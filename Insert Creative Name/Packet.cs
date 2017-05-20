using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
namespace Insert_Creative_Name
{
    internal abstract class Packet
    {
        protected internal byte Signature { internal get; set; }
        protected internal byte OpCode { internal get; set; }
        internal byte Sequence { get; set; }
        protected internal byte[] Data { internal get; set; }
        internal int Position { get; set; }
        protected internal byte[] Original { internal get; set; }
        protected internal DateTime TimeStamp { internal get; set; }
        internal bool ShouldEncrypt => EncryptMethod != EncryptMethod.None;
        internal abstract EncryptMethod EncryptMethod { get; }

        internal Packet(byte opcode)
        {
            Signature = 170;
            OpCode = opcode;
            Data = new byte[0];
        }
        internal Packet(byte[] buffer)
        {
            Original = buffer;
            Signature = buffer[0];
            OpCode = buffer[3];
            Sequence = buffer[4];
            TimeStamp = DateTime.UtcNow;
            int num = buffer.Length - (ShouldEncrypt ? 5 : 4);
            Data = new byte[num];
            Array.Copy(buffer, buffer.Length - num, Data, 0, num);
        }
        internal void Clear()
        {
            Position = 0;
            Data = new byte[0];
        }
        internal byte[] ReadBytes(int length)
        {
            if (Position + length > Data.Length)
                throw new EndOfStreamException();

            byte[] array = new byte[length];
            Array.Copy(Data, Position, array, 0, length);
            Position += length;
            return array;
        }
        internal byte ReadByte()
        {
            if (Position >= Data.Length)
                throw new EndOfStreamException();

            return Data[Position++];
        }
        internal sbyte ReadSByte()
        {
            if (Position >= Data.Length)
                throw new EndOfStreamException();

            return (sbyte)Data[Position++];
        }
        internal bool ReadBoolean()
        {
            if (Position >= Data.Length)
                throw new EndOfStreamException();

            return Data[Position++] != 0;
        }
        internal short ReadInt16()
        {
            byte[] array = ReadBytes(2);
            return (short)(array[0] << 8 | array[1]);
        }
        internal ushort ReadUInt16()
        {
            byte[] array = ReadBytes(2);
            return (ushort)(array[0] << 8 | array[1]);
        }
        internal int ReadInt32()
        {
            byte[] array = ReadBytes(4);
            return array[0] << 24 | array[1] << 16 | array[2] << 8 | array[3];
        }
        internal uint ReadUInt32()
        {
            byte[] array = ReadBytes(4);
            return (uint)(array[0] << 24 | array[1] << 16 | array[2] << 8 | array[3]);
        }
        internal string ReadString()
        {
            int num = Data.Length;
            for (int i = 0; i < Data.Length; i++)
                if (Data[i] == 0)
                {
                    num = i;
                    break;
                }

            byte[] array = new byte[num - Position];
            Buffer.BlockCopy(Data, Position, array, 0, array.Length);
            Position = num + 1;
            if (Position > Data.Length)
                Position = Data.Length;

            return Encoding.GetEncoding(949).GetString(array);
        }
        internal string ReadString8()
        {
            if (Position >= Data.Length)
                throw new EndOfStreamException();

            int num = ReadByte();
            if (Position + num > Data.Length)
            {
                Position--;
                throw new EndOfStreamException();
            }

            byte[] bytes = ReadBytes(num);
            return Encoding.GetEncoding(949).GetString(bytes);
        }
        internal string ReadString16()
        {
            if (Position + 1 > Data.Length)
                throw new EndOfStreamException();

            int num = ReadUInt16();
            if (Position + num > Data.Length)
            {
                Position -= 2;
                throw new EndOfStreamException();
            }
            byte[] bytes = ReadBytes(num);
            return Encoding.GetEncoding(949).GetString(bytes);
        }
        internal byte[] ReadData8()
        {
            if (Position >= Data.Length)
                throw new EndOfStreamException();

            int num = ReadByte();
            if (Position + num > Data.Length)
            {
                Position--;
                throw new EndOfStreamException();
            }
            return ReadBytes(num);
        }
        internal byte[] ReadData16()
        {
            if (Position + 1 > Data.Length)
                throw new EndOfStreamException();

            int num = ReadUInt16();
            if (Position + num > Data.Length)
            {
                Position -= 2;
                throw new EndOfStreamException();
            }
            return ReadBytes(num);
        }
        internal Point ReadPoint()
        {
            if (Position + 4 > Data.Length)
                throw new EndOfStreamException();

            return new Point((short)ReadUInt16(), (short)ReadUInt16());
        }
        internal void Write(byte[] buffer)
        {
            int num = Position + buffer.Length;
            if (num > Data.Length)
                ResizeArray(num);

            Array.Copy(buffer, 0, Data, Position, buffer.Length);
            Position += buffer.Length;
        }
        internal void WriteByte(byte value)
        {
            Write(new byte[]
            {
                value
            });
        }
        internal void WriteSByte(sbyte value)
        {
            Write(new byte[]
            {
                (byte)value
            });
        }
        internal void WriteBoolean(bool value)
        {
            WriteByte((byte)(value ? 1 : 0));
        }
        internal void WriteInt16(short value)
        {
            byte[] buffer = new byte[]
            {
                (byte)(value >> 8),
                (byte)value
            };
            Write(buffer);
        }
        internal void WriteUInt16(ushort value)
        {
            Write(new byte[2]
            {
                (byte) ((uint) value >> 8),
                (byte) value
            });
        }
        internal void WriteInt32(int value)
        {
            byte[] buffer = new byte[]
            {
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value
            };
            Write(buffer);
        }
        internal void WriteUInt32(uint value)
        {
            byte[] buffer = new byte[]
            {
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value
            };
            Write(buffer);
        }
        internal void WriteString(string value, bool terminate = false)
        {
            Write(Encoding.GetEncoding(949).GetBytes(value));
            if (terminate)
                WriteByte(0);
        }
        internal void WriteString8(string value)
        {
            byte[] bytes = Encoding.GetEncoding(949).GetBytes(value);
            if (bytes.Length > 255)
                throw new ArgumentOutOfRangeException("value", value, "Length of string must not exceed 255 characters");

            WriteByte((byte)bytes.Length);
            Write(bytes);
        }
        internal void WriteString16(string value)
        {
            byte[] bytes = Encoding.GetEncoding(949).GetBytes(value);
            if (bytes.Length > 65535)
                throw new ArgumentOutOfRangeException("value", value, "Length of string must not exceed 65535 characters");

            WriteUInt16((ushort)bytes.Length);
            Write(bytes);
        }
        internal void WritePoint(Point value)
        {
            WriteInt16(value.X);
            WriteInt16(value.Y);
        }
        internal void WriteArray(Array value)
        {
            foreach (object current in value)
            {
                if (current is char)
                    WriteByte((byte)current);
                else if (current is byte)
                    WriteByte((byte)current);
                else if (current is sbyte)
                    WriteSByte((sbyte)current);
                else if (current is bool)
                    WriteBoolean((bool)current);
                else if (current is short)
                    WriteInt16((short)current);
                else if (current is ushort)
                    WriteUInt16((ushort)current);
                else if (current is int)
                    WriteInt32((int)current);
                else if (current is uint)
                    WriteUInt32((uint)current);
                else if (current is string)
                    WriteString8((string)current);
                else if (current is Point)
                    WritePoint((Point)current);
                else if (current is Array)
                    WriteArray((Array)current);
            }
        }
        internal byte[] ToArray()
        {
            int num = Data.Length + (ShouldEncrypt ? 5 : 4) - 3;
            byte[] array = new byte[num + 3];
            array[0] = Signature;
            array[1] = (byte)(num / 256);
            array[2] = (byte)(num % 256);
            array[3] = OpCode;
            array[4] = Sequence;
            Array.Copy(Data, 0, array, array.Length - Data.Length, Data.Length);
            return array;
        }
        internal string GetHexString()
        {
            int num = Data.Length + 1;
            byte[] array = new byte[num];
            array[0] = OpCode;
            Array.Copy(Data, 0, array, 1, Data.Length);
            return BitConverter.ToString(array).Replace('-', ' ');
        }
        internal string GetAsciiString(bool replaceNewline = true)
        {
            char[] array = new char[Data.Length + 1];
            byte[] array2 = new byte[Data.Length + 1];
            array2[0] = OpCode;
            Array.Copy(Data, 0, array2, 1, Data.Length);
            for (int i = 0; i < array2.Length; i++)
            {
                byte b = array2[i];
                if ((b == 10 || b == 13) && !replaceNewline)
                    array[i] = (char)b;
                else if (b < 32 || b > 126)
                    array[i] = '.';
                else
                    array[i] = (char)b;
            }
            return new string(array);
        }

        protected void ResizeArray(int newLength)
        {
            var data = Data;
            Array.Resize(ref data, newLength);
            Data = data;
        }
    }
}
