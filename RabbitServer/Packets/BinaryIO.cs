using System;
using System.IO;
using System.Text;
using Box2DX.Common;

namespace RabbitServer.Packets
{
    public sealed class PacketBinaryReader : BinaryReader
    {
        public int PacketLength { get; }
        public byte PacketId { get; }

        public PacketBinaryReader(int size,Stream input) : base(new BufferedStream(input),Encoding.Unicode,true)
        {
            PacketLength = ReadInt16();
            PacketId = ReadByte();
        }

        public Vec2 ReadVec2()
        {
            return new Vec2(ReadSingle(), ReadSingle());
        }
        public override string ReadString()
        {
            string str = "";
            char c;
            do
            {
                c = ReadChar();
                str += c;
            } while (c != '\x00');

            return str;
        }
    }

    public sealed class PacketBinaryWriter : BinaryWriter
    {
        private byte PacketId;

        public PacketBinaryWriter(byte packet) : base(new MemoryStream(256), Encoding.Unicode, true)
        {
            PacketId = packet;
            Seek(3, SeekOrigin.Begin);
        }

        public void AddBeginningBytes()
        {
            MemoryStream stream = (MemoryStream) BaseStream;
            Seek(0, SeekOrigin.Begin);
            Write((short) (stream.Length - 2));
            Write(PacketId);
            stream.Flush();
        }

        public void Write(Vec2 vec)
        {
            Write(vec.X);
            Write(vec.Y);
        }
        public override void Write(string str)
        {
            Write(Encoding.Unicode.GetBytes(str));
            
        }
    }
}