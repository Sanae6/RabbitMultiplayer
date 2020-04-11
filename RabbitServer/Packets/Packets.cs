using Box2DX.Common;

namespace RabbitServer.Packets
{
    [Packet(0x00)]
    public class Connect : IPacket
    {
        public string Name { get; private set; }
        public ushort PlayerId { get; set; }
        public ushort Version { get; set; }
        public override void Read(PacketBinaryReader reader)
        {
            Name = reader.ReadString();
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
            //for the server to automatically generate th epacket
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
        public ushort PlayerId { get; set; }
        public Vec2 Pos { get; set; }
        public uint RoomNumber { get; set; }

        public override void Read(PacketBinaryReader reader)
        {
            Pos = reader.ReadVec2();
            RoomNumber = reader.ReadUInt32();
        }

        public override void Write(PacketBinaryWriter writer)
        {
            writer.Write(Pos);
            writer.Write(RoomNumber);
        }
    }

    [Packet(0x03)]
    public class PlayerConnect : IPacket
    {
        public ushort PlayerId { get; set; }
        public bool Quitting;
        public override void Read(PacketBinaryReader reader)
        {
            Quitting = reader.ReadBoolean();
        }

        public override void Write(PacketBinaryWriter writer)
        {
            writer.Write(Quitting);
        }
    }
}