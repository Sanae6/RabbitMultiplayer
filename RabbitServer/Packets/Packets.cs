using System;
using Box2DX.Common;

namespace RabbitServer.Packets
{
    [Packet(0x00)]
    public class Connect : IPacket
    {
        public ushort PlayerId { get; set; }
        public ushort Version { get; set; }
        public override void Read(PacketBinaryReader reader)
        {
            Version = reader.ReadUInt16();
        }

        public override void Write(PacketBinaryWriter writer)
        {
            writer.Write(PlayerId);
        }
    }

    [Packet(0x01)]
    public class Disconnect : IPacket
    {
        public Disconnect(string reason)
        {
            Reason = reason;
        }

        public Disconnect()
        {
            //for the server to automatically generate the packet
        }

        public string Reason { get; set; }
        
        public override void Read(PacketBinaryReader reader)
        {
            Reason = reader.ReadString();
        }

        public override void Write(PacketBinaryWriter writer)
        {
            writer.Write(Reason);
        }
    }

    [Packet(0x02)]//does nothing at all
    public class Movement : IPacket
    {
        public ushort PlayerId { private get; set; }
        public Vec2 Pos { get; set; }
        public uint RoomNumber { get; set; }

        public override void Read(PacketBinaryReader reader)
        {
            Pos = reader.ReadVec2();
            RoomNumber = reader.ReadUInt32();
        }

        public override void Write(PacketBinaryWriter writer)
        {
            writer.Write(PlayerId);
            writer.Write(Pos);
            writer.Write(RoomNumber);
        }
    }

    [Packet(0x03)]
    public class PlayerConnect : IPacket
    {
        public ushort PlayerId { private get; set; }
        public bool Quitting { get; set; }
        public string Name { get; set; }
        public Vec2 Pos { get; set; }
        public uint RoomNumber { get; set; }
        public string Reason { get; set; }
        public override void Read(PacketBinaryReader reader)
        {
            Quitting = reader.ReadByte() == 1;
            if (Quitting)
            {
                Name = reader.ReadString();
                Pos = reader.ReadVec2();
                RoomNumber = reader.ReadUInt32();
            }
            else
            {
                Reason = reader.ReadString();
            }
        }

        public override void Write(PacketBinaryWriter writer)
        {
            writer.Write(PlayerId);
            writer.Write(Quitting);
            if (!Quitting)
            {
                writer.Write(Reason);
            }
            else
            {
                writer.Write(Name);
                writer.Write(Pos);
                writer.Write(RoomNumber);
            }
        }
    }

    [Packet(0x04)]
    public class Ping : IPacket
    {
        private ulong PID { get; set; }

        public override void Read(PacketBinaryReader reader)
        {
            PID = reader.ReadUInt64();
        }

        public override void Write(PacketBinaryWriter writer)
        {
            if (PID >= UInt64.MaxValue - 100) PID = 0;
            writer.Write(PID+1);
        }
    }
}