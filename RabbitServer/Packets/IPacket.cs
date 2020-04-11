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
            var fields = GetType().GetFields();
            for (var i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                builder.Append(field.Name).Append(" = ").Append(field.GetValue(this));
                if (i != fields.Length - 1) builder.Append(", ");
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