using System;
using System.Text;

namespace RabbitServer.Packets
{
    public abstract class IPacket
    {
        public abstract void Read(PacketBinaryReader reader);
        public abstract void Write(PacketBinaryWriter writer);

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.GetType().Name + "(");
            var props = GetType().GetProperties();
            for (var i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                builder.Append(prop.Name).Append(" = ").Append(prop.GetValue(this));
                if (i != props.Length - 1) builder.Append(", ");
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class Packet : Attribute
    {
        public byte packetId;

        public Packet(byte packetId)
        {
            this.packetId = packetId;
        }
    }
}