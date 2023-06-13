using System;
using System.Collections.Generic;
using System.Text;

namespace beepe0.UNetwork
{
    public abstract class UNetworkPacket
    {
        public List<byte> BufferBytes;
        public ushort ReadPointer;

        public ushort GetLength() => (ushort)BufferBytes.Count;
        public byte[] ToArray() => BufferBytes.ToArray();
        
        public void Insert(ushort value) => BufferBytes.InsertRange(0, BitConverter.GetBytes(value));
        
        public void Write(byte value) => BufferBytes.Add(value);
        public void Write(byte[] value) => BufferBytes.AddRange(value);
        public void Write(bool value) => BufferBytes.AddRange(BitConverter.GetBytes(value));
        public void Write(short value) => BufferBytes.AddRange(BitConverter.GetBytes(value));
        public void Write(ushort value) => BufferBytes.AddRange(BitConverter.GetBytes(value));
        public void Write(int value) => BufferBytes.AddRange(BitConverter.GetBytes(value));
        public void Write(long value) => BufferBytes.AddRange(BitConverter.GetBytes(value));
        public void Write(float value) => BufferBytes.AddRange(BitConverter.GetBytes(value));
        public void Write(double value) => BufferBytes.AddRange(BitConverter.GetBytes(value));
        public void Write(string value)
        {
            Write((ushort)value.Length);
            BufferBytes.AddRange(Encoding.ASCII.GetBytes(value));
        }
        
        public byte ReadByte(bool movePointer = true)
        {
            byte res = BufferBytes[ReadPointer];
            if (movePointer)
            {
                ReadPointer += 1;
            }
            return res;
        }
        public List<byte> ReadBytes(ushort count, bool movePointer = true)
        {
            List<byte> res = BufferBytes.GetRange(ReadPointer, count);
            if (movePointer)
            {
                ReadPointer += count;
            }
            return res;
        }
        public bool ReadBool(bool movePointer = true)
        {
            bool res = BitConverter.ToBoolean(BufferBytes.GetRange(ReadPointer, 1).ToArray());
            if (movePointer)
            {
                ReadPointer += 1;
            }
            return res;
        }
        public short ReadShort(bool movePointer = true)
        {
            short res = BitConverter.ToInt16(BufferBytes.GetRange(ReadPointer, 2).ToArray());
            if (movePointer)
            {
                ReadPointer += 2;
            }
            return res;
        }
        public ushort ReadUShort(bool movePointer = true)
        {
            ushort res = BitConverter.ToUInt16(BufferBytes.GetRange(ReadPointer, 2).ToArray());
            if (movePointer)
            {
                ReadPointer += 2;
            }
            return res;
        }
        public int ReadInt(bool movePointer = true)
        {
            int res = BitConverter.ToInt32(BufferBytes.GetRange(ReadPointer, 4).ToArray());
            if (movePointer)
            {
                ReadPointer += 4;
            }
            return res;
        }
        public long ReadLong(bool movePointer = true)
        {
            long res = BitConverter.ToInt64(BufferBytes.GetRange(ReadPointer, 8).ToArray());
            if (movePointer)
            {
                ReadPointer += 8;
            }
            return res;
        }
        public float ReadFloat(bool movePointer = true)
        {
            float res = BitConverter.ToSingle(BufferBytes.GetRange(ReadPointer, 4).ToArray());
            if (movePointer)
            {
                ReadPointer += 4;
            }
            return res;
        }
        public double ReadDouble(bool movePointer = true)
        {
            double res = BitConverter.ToDouble(BufferBytes.GetRange(ReadPointer, 8).ToArray());
            if (movePointer)
            {
                ReadPointer += 8;
            }
            return res;
        }
        public string ReadString(bool movePointer = true)
        {
            ushort length = ReadUShort();
            string res = Encoding.ASCII.GetString(BufferBytes.GetRange(ReadPointer, length).ToArray());
            if (movePointer)
            {
                ReadPointer += length;
            }
            return res;
        }
    }
    public sealed class UNetworkIOPacket : UNetworkPacket
    {
        public UNetworkIOPacket()
        {
            ReadPointer = 0;
            BufferBytes = new List<byte>();
        }
        public UNetworkIOPacket(ushort packetNumber)
        {
            ReadPointer = 0;
            BufferBytes = new List<byte>();

            Write(packetNumber);
        }
        public UNetworkIOPacket(byte[] data)
        {
            ReadPointer = 0;
            BufferBytes = new List<byte>();

            Write(data);
        }
    }
    public sealed class UNetworkReadablePacket : UNetworkPacket
    {
        public ushort Length;
        public ushort Index;
        public ushort PacketNumber;
    }
}
