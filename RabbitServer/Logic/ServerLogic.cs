using System;
using System.Collections.Generic;
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

        private byte GetNewPlayerSlot()
        {
            if (Players.Count == 0) return 0;
            return (byte) Players.FindIndex(p=>Players[Players.IndexOf(p)+1]==null);
        }
        
        private ServerPlayer GetPlayer(Server.ClientInstance client)
        {
            return Players.First(x => x.client == client);
        }
        
        public void UserJoined(Server.ClientInstance ci)
        {
            var sp = new ServerPlayer(server,ci);
            if (Players.Count > UInt16.MaxValue) sp.SendPacket(new Disconnect("This server is somehow full."));
        }

        public void UserLeft(Server.ClientInstance ci)
        {
            var sp = GetPlayer(ci);
            Broadcast(new PlayerConnect{PlayerId = sp.PlayerSlot, Quitting = true},false,sp);
            Players.Remove(sp);
        }

        public void Broadcast(IPacket packet, bool toSelf, ServerPlayer player)
        {
            foreach (var p in Players)
                if (!toSelf && p != player)
                    p.SendPacket(packet);
        }

        public void PacketReceived(Server.ClientInstance client, IPacket packet)
        {
            Console.WriteLine(packet);
            ServerPlayer player = GetPlayer(client);
            Console.WriteLine(packet.GetType().Name);
            switch (packet.GetType().Name)
            {
                case "Connect":
                    Connect cn = (Connect) packet;
                    if (cn.Version != Server.Version)
                    {
                        player.SendPacket(
                            new Disconnect($"Your version {cn.Version} is different than the server's version. {Server.Version}"));
                        return;
                    }
                    player.PlayerName = cn.Name;
                    cn.PlayerId = player.PlayerSlot = GetNewPlayerSlot();
                    break;
                case "Disconnect":
                    Disconnect dc = (Disconnect) packet;
                    Console.WriteLine(dc.Reason);
                    break;
                case "Movement":
                    Movement mv = (Movement) packet;
                    player.Position = mv.Pos;
                    Broadcast(mv,false,player);
                    break;
                case "PlayerConnect":
                    PlayerConnect pcn = (PlayerConnect) packet;
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
        public /*Room*/ int CurrentRoom;
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
            server.WriteQueues[client.InstanceId].Add(packet);
        }
    }
}