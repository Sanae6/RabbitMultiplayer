using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Jint;
using RabbitServer.Logic;
using RabbitServer.Packets;

namespace RabbitServer
{
    public class Server
    {
        private TcpListener _listener;
        public static ushort Version => 1;

        internal readonly Dictionary<int, BlockingCollection<IPacket>> WriteQueues =
            new Dictionary<int, BlockingCollection<IPacket>>();

        private static readonly ManualResetEvent TcpClientConnected =
            new ManualResetEvent(false);
        private readonly Dictionary<byte,Type> _packetTypes = new Dictionary<byte, Type>();
        private ServerLogic logic;
        static void Main(String[] args)
        {
            Server server = new Server();
            server.Start(7773);
            foreach (var type in Assembly.GetAssembly(typeof(IPacket)).GetTypes()
                .Where(t => typeof(IPacket).IsAssignableFrom(t)))
            {
                if (type == typeof(IPacket)) continue;
                var attribute = (Packet) type.GetCustomAttributes(typeof(Packet)).First();
                server._packetTypes.Add(attribute.packetId, type);
            }
            
            server.Listen();
            Console.WriteLine("closed");
        }

        void Start(int port)
        {
            logic = new ServerLogic(this);
            _listener = TcpListener.Create(port);
            _listener.Start();
            Console.WriteLine("Bound to port {0}", port);
        }

        void Listen()
        {
            while (true)
            {
                TcpClientConnected.Reset();
                object obj = new object();
                var clientTask = _listener.BeginAcceptTcpClient(ClientHandler, obj);
                TcpClientConnected.WaitOne();
            }
        }

        private void ClientHandler(IAsyncResult ar)
        {
            var client = _listener.EndAcceptTcpClient(ar);
            TcpClientConnected.Set();
            var ci = new ClientInstance(client);
            //GameLogic.CallEvent("joined",ci.InstanceId);
            
            WriteQueues.Add(ci.InstanceId, new BlockingCollection<IPacket>());
            try
            {
                Read(ci);
                Task.Run(() => WriteAction(ci));
            }
            catch (StackOverflowException e)
            {
                Console.Error.WriteLine(e.ToString());
            }
        }

        private void WriteAction(ClientInstance client)
        {
            while (client.client.Connected)
            {
                var packet = WriteQueues[client.InstanceId].Take();
                byte packetId = _packetTypes.First(x => x.Value == packet.GetType()).Key;
                var writer = new PacketBinaryWriter(packetId);
                if (!client.client.Connected) return;
                packet.Write(writer);
                writer.AddBeginningBytes();
                var ma = ((MemoryStream) writer.BaseStream).ToArray();
                client.client.GetStream().Write(ma,0,ma.Length);
                //writer.Close();
                if (packetId == 0x01)
                {
                    client.client.Close();
                    client.client.Dispose();
                }
            }
        }

        private void Read(ClientInstance client)
        {
            byte[] buffer = new byte[client.client.ReceiveBufferSize];
            client.client.GetStream()
                .BeginRead(buffer, 0, buffer.Length, ReadCallback, new ClientBufferObject(client, buffer));
        }

        private void ReadCallback(IAsyncResult ar)
        {
            ClientBufferObject o = ar.AsyncState as ClientBufferObject;
            if (o != null && o.client.client.Connected)
            {
                Stream stream = o.client.client.GetStream();
                try
                {
                    int read = stream.EndRead(ar);
                    if (read == 0 || !o.client.client.Connected)
                    {
                        stream.Close();
                        o.client.client.Close();
                        return;
                    }
                    PacketBinaryReader reader = new PacketBinaryReader(read, new MemoryStream(o.buffer));
                    if (!_packetTypes.ContainsKey(reader.PacketId))
                    {
                        Console.WriteLine("Packet with ID "+ reader.PacketId +" isn't defined!!!");
                    }
                    else
                    {
                        IPacket packet = (IPacket) _packetTypes[reader.PacketId].GetConstructor(new Type[] { })
                            ?.Invoke(new object[] { });
                        packet.Read(reader);
                        reader.Close();
                        logic.PacketReceived(o.client, packet);
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                }
            }
            else return;
            Read(o?.client);
        }

        
        class ClientBufferObject
        {
            internal ClientInstance client;
            public byte[] buffer;

            public ClientBufferObject(ClientInstance client, byte[] buffer)
            {
                this.client = client;
                this.buffer = buffer;
            }
        }

        public class ClientInstance
        {
            private static int nextInstance = 1;
            public int InstanceId { get; private set; }
            public TcpClient client;

            public ClientInstance(TcpClient client)
            {
                this.client = client;
                InstanceId = nextInstance++;
            }
        }
    }
}