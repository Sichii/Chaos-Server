using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
namespace Insert_Creative_Name
{
    internal abstract class Packet
    {
        protected byte signature;
        protected byte opCode;
        protected byte sequence;
        protected byte[] data;
        protected int position;
        protected byte[] original;
        protected DateTime timeStamp;
        internal byte Signature
        {
            get
            {
                return signature;
            }
        }
        internal byte Opcode
        {
            get
            {
                return opCode;
            }
        }
        internal byte Sequence
        {
            get
            {
                return sequence;
            }
            set
            {
                sequence = value;
            }
        }
        internal byte[] Data
        {
            get
            {
                return data;
            }
        }
        internal int Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        internal byte[] Original
        {
            get
            {
                return original;
            }
        }
        internal DateTime TimeStamp
        {
            get
            {
                return timeStamp;
            }
        }
        internal bool ShouldEncrypt
        {
            get
            {
                return EncryptMethod != EncryptMethod.None;
            }
        }
        internal abstract EncryptMethod EncryptMethod
        {
            get;
        }
        internal Packet(byte opcode)
        {
            signature = 170;
            opCode = opcode;
            data = new byte[0];
        }
        internal Packet(byte[] buffer)
        {
            original = buffer;
            signature = buffer[0];
            opCode = buffer[3];
            sequence = buffer[4];
            timeStamp = DateTime.UtcNow;
            int num = buffer.Length - (ShouldEncrypt ? 5 : 4);
            data = new byte[num];
            Array.Copy(buffer, buffer.Length - num, data, 0, num);
        }
        internal abstract void Encrypt(Crypto crypto);
        internal abstract void Decrypt(Crypto crypto);
        internal void Clear()
        {
            position = 0;
            data = new byte[0];
        }
        internal byte[] ReadBytes(int length)
        {
            if (position + length > data.Length)
            {
                throw new EndOfStreamException();
            }
            byte[] array = new byte[length];
            Array.Copy(data, position, array, 0, length);
            position += length;
            return array;
        }
        internal byte ReadByte()
        {
            if (position >= data.Length)
            {
                throw new EndOfStreamException();
            }
            return data[position++];
        }
        internal sbyte ReadSByte()
        {
            if (position >= data.Length)
            {
                throw new EndOfStreamException();
            }
            return (sbyte)data[position++];
        }
        internal bool ReadBoolean()
        {
            if (position >= data.Length)
            {
                throw new EndOfStreamException();
            }
            return data[position++] != 0;
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
            int num = data.Length;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0)
                {
                    num = i;
                    break;
                }
            }
            byte[] array = new byte[num - position];
            Buffer.BlockCopy(data, position, array, 0, array.Length);
            position = num + 1;
            if (position > data.Length)
            {
                position = data.Length;
            }
            return Encoding.GetEncoding(949).GetString(array);
        }
        internal string ReadString8()
        {
            if (position >= data.Length)
            {
                throw new EndOfStreamException();
            }
            int num = ReadByte();
            if (position + num > data.Length)
            {
                position--;
                throw new EndOfStreamException();
            }
            byte[] bytes = ReadBytes(num);
            return Encoding.GetEncoding(949).GetString(bytes);
        }
        internal string ReadString16()
        {
            if (position + 1 > data.Length)
            {
                throw new EndOfStreamException();
            }
            int num = ReadUInt16();
            if (position + num > data.Length)
            {
                position -= 2;
                throw new EndOfStreamException();
            }
            byte[] bytes = ReadBytes(num);
            return Encoding.GetEncoding(949).GetString(bytes);
        }
        internal Point ReadPoint()
        {
            if (position + 4 > data.Length)
            {
                throw new EndOfStreamException();
            }
            return new Point((short)ReadUInt16(), (short)ReadUInt16());
        }
        internal void Write(byte[] buffer)
        {
            int num = position + buffer.Length;
            if (num > data.Length)
            {
                Array.Resize(ref data, num);
            }
            Array.Copy(buffer, 0, data, position, buffer.Length);
            position += buffer.Length;
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
            WriteByte(value ? (byte)1 : (byte)0);
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
            byte[] bytes = Encoding.GetEncoding(949).GetBytes(value);
            Write(bytes);
            if (terminate)
            {
                WriteByte(0);
            }
        }
        internal void WriteString8(string value)
        {
            byte[] bytes = Encoding.GetEncoding(949).GetBytes(value);
            if (bytes.Length > 255)
            {
                throw new ArgumentOutOfRangeException("value", value, "Length of string must not exceed 255 characters");
            }
            WriteByte((byte)bytes.Length);
            Write(bytes);
        }
        internal void WriteString16(string value)
        {
            byte[] bytes = Encoding.GetEncoding(949).GetBytes(value);
            if (bytes.Length > 65535)
            {
                throw new ArgumentOutOfRangeException("value", value, "Length of string must not exceed 65535 characters");
            }
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
                {
                    WriteByte((byte)current);
                }
                if (current is byte)
                {
                    WriteByte((byte)current);
                }
                if (current is sbyte)
                {
                    WriteSByte((sbyte)current);
                }
                if (current is bool)
                {
                    WriteBoolean((bool)current);
                }
                if (current is short)
                {
                    WriteInt16((short)current);
                }
                if (current is ushort)
                {
                    WriteUInt16((ushort)current);
                }
                if (current is int)
                {
                    WriteInt32((int)current);
                }
                if (current is uint)
                {
                    WriteUInt32((uint)current);
                }
                if (current is string)
                {
                    WriteString8((string)current);
                }
                if (current is Point)
                {
                    WritePoint((Point)current);
                }
                if (current is Array)
                {
                    WriteArray((Array)current);
                }
            }
        }
        internal byte[] ToArray()
        {
            int num = data.Length + (ShouldEncrypt ? 5 : 4) - 3;
            byte[] array = new byte[num + 3];
            array[0] = signature;
            array[1] = (byte)(num / 256);
            array[2] = (byte)(num % 256);
            array[3] = opCode;
            array[4] = sequence;
            Array.Copy(data, 0, array, array.Length - data.Length, data.Length);
            return array;
        }
        internal string GetHexString()
        {
            int num = data.Length + 1;
            byte[] array = new byte[num];
            array[0] = opCode;
            Array.Copy(data, 0, array, 1, data.Length);
            return BitConverter.ToString(array).Replace('-', ' ');
        }
        internal string GetAsciiString(bool replaceNewline = true)
        {
            char[] array = new char[data.Length + 1];
            byte[] array2 = new byte[data.Length + 1];
            array2[0] = opCode;
            Array.Copy(data, 0, array2, 1, data.Length);
            for (int i = 0; i < array2.Length; i++)
            {
                byte b = array2[i];
                if ((b == 10 || b == 13) && !replaceNewline)
                {
                    array[i] = (char)b;
                }
                else if (b < 32 || b > 126)
                {
                    array[i] = '.';
                }
                else
                {
                    array[i] = (char)b;
                }
            }
            return new string(array);
        }
    }
}
