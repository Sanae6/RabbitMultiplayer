using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Box2DX.Common;
using RabbitServer.Packets;

namespace RabbitServer.Logic
{
    public class ServerLogic
    {
        public List<ServerPlayer> Players = new List<ServerPlayer>();
        private Server server;
        public ServerLogic(Server server)
        {
            this.server = server;
        }

        private int GetNewPlayerSlot()
        {
            if (Players.Count == 0) return 0;
            for (int i = 0;i<UInt16.MaxValue;i++)
            {
                if (Players.ElementAtOrDefault(i)==null) return i;
            };
            return -1;
        }
        
        private ServerPlayer GetPlayer(Server.ClientInstance client)
        {
            return Players.First(x => x.client == client);
        }
        
        public void UserJoined(Server.ClientInstance ci)
        {
            Console.WriteLine("Someone joined!");
            var sp = new ServerPlayer(server,ci);
            if (Players.Count > UInt16.MaxValue) sp.SendPacket(new Disconnect("This server is somehow full."));
            Players.Add(sp);
        }

        public void UserLeft(Server.ClientInstance ci)
        {
            
            try
            {
                //Players.First(s => { return s.client == ci; });
                var sp = GetPlayer(ci);
                Console.WriteLine($"{sp.PlayerName} left. ({sp.PlayerSlot})");
                Broadcast(new PlayerConnect{PlayerId = sp.PlayerSlot, Quitting = false},false,sp);
                Players.Remove(sp);
            }
            catch
            {
                Console.WriteLine($"Someone left. ({ci.InstanceId})");
            }
        }

        public void Broadcast(IPacket packet, bool toSelf, ServerPlayer player)
        {
            foreach (var p in Players)
                if (!toSelf && p != player)
                    p.SendPacket(packet);
        }

        public void PacketReceived(Server.ClientInstance client, IPacket packet)
        {
            Console.WriteLine(packet.ToString());
            ServerPlayer player = GetPlayer(client);
            switch (packet.GetType().Name)
            {
                case "Connect":
                    Connect cn = (Connect) packet;
                    if (cn.Version != Server.Version)
                    {
                        player.Disconnect(
                            $"Your version {cn.Version} is different than the server's version. {Server.Version}");
                        return;
                    }
                    var slot = GetNewPlayerSlot();
                    if (slot == -1)
                    {
                        player.Disconnect("Too many people are online.");
                        return;
                    }
                    cn.PlayerId = player.PlayerSlot = (ushort) slot;
                    player.SendPacket(cn);//kirby right back at ya
                    break;
                case "Disconnect":
                    Disconnect dc = (Disconnect) packet;
                    Console.WriteLine(dc.Reason);
                    //player.client.client.Close();
                    Broadcast(new PlayerConnect{PlayerId = player.PlayerSlot, Quitting = true, Reason = dc.Reason},false,player);
                    break;
                case "Movement":
                    Movement mv = (Movement) packet;
                    player.Position = mv.Pos;
                    mv.PlayerId = player.PlayerSlot;
                    Broadcast(mv,false,player);
                    break;
                case "PlayerConnect":
                    PlayerConnect pcn = (PlayerConnect) packet;
                    if (!pcn.Quitting)
                    {
                        player.PlayerName = pcn.Name;
                        player.Position = pcn.Pos;
                        player.CurrentRoom = pcn.RoomNumber;
                    }
                    Broadcast(pcn, false, player);
                    break;
            }
        }
    }
    public class ServerPlayer
    {
        public ushort PlayerSlot;
        public string PlayerName;
        public Vec2 Position;
        public /*Room*/ uint CurrentRoom;
        //potential to implement server side object management ;) and holy fuck that would be insane
        //multiplayer alone is absurd, but the idea of co-op is just crazy
        public Server server;
        public Server.ClientInstance client;
        
        public ServerPlayer(Server server, Server.ClientInstance client)
        {
            this.server = server;
            this.client = client;
        }

        public void SendPacket(IPacket packet)
        {
            if (!client.client.Connected||!server.WriteQueues.ContainsKey(client.InstanceId)) return;
            server.WriteQueues[client.InstanceId].Add(packet);
        }

        public void Disconnect(string reason)
        {
            SendPacket(new Disconnect(reason));
        }
    }
}