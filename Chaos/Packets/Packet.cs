// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using System;
using System.IO;
using System.Text;
namespace Chaos
{
    internal abstract class Packet
    {
        protected internal DateTime Creation { internal get; set; }
        protected internal byte Identifier { internal get; set; }
        protected internal byte OpCode { internal get; set; }
        internal bool ShouldEncrypt => EncryptionType != EncryptionType.None;
        internal abstract EncryptionType EncryptionType { get; }
        internal byte Counter;
        protected internal byte[] Data;
        internal int Position;

        internal Packet(byte opcode)
        {
            Identifier = 170;
            OpCode = opcode;
            Data = new byte[0];
        }
        internal Packet(byte[] buffer)
        {
            Identifier = buffer[0];
            OpCode = buffer[3];
            Counter = buffer[4];
            Creation = DateTime.UtcNow;

            int resultLength = buffer.Length - (ShouldEncrypt ? 5 : 4);
            Data = new byte[resultLength];
            Buffer.BlockCopy(buffer, buffer.Length - resultLength, Data, 0, resultLength);
        }
        internal byte[] ReadBytes(int length)
        {
            if (Position + length > Data.Length)
                throw new EndOfStreamException();

            byte[] readData = new byte[length];
            Buffer.BlockCopy(Data, Position, readData, 0, length);
            Position += length;
            return readData;
        }
        internal byte ReadByte()
        {
            if (Position >= Data.Length)
                throw new EndOfStreamException();

            return Data[Position++];
        }
        internal sbyte ReadSByte() => (sbyte)ReadByte();
        internal bool ReadBoolean() => ReadByte() != 0;
        internal short ReadInt16() => (short)(ReadByte() << 8 | ReadByte());
        internal ushort ReadUInt16() => (ushort)(ReadByte() << 8 | ReadByte());
        internal int ReadInt32() => ReadByte() << 24 | ReadByte() << 16 | ReadByte() << 8 | ReadByte();
        internal uint ReadUInt32() => (uint)(ReadByte() << 24 | ReadByte() << 16 | ReadByte() << 8 | ReadByte());

        internal string ReadString()
        {
            int length = Data.Length;
            if (Position + length > Data.Length)
            {
                Position--;
                throw new EndOfStreamException();
            }
            for (int i = 0; i < Data.Length; i++)
                if (Data[i] == 0)
                {
                    length = i;
                    break;
                }

            byte[] readData = new byte[length - Position];
            Buffer.BlockCopy(Data, Position, readData, 0, readData.Length);
            Position = length + 1;
            if (Position > Data.Length)
                Position = Data.Length;

            return Encoding.GetEncoding(949).GetString(readData);
        }
        internal string ReadString8()
        {
            if (Position >= Data.Length)
                throw new EndOfStreamException();

            int lengthPrefix = ReadByte();
            if (Position + lengthPrefix > Data.Length)
            {
                Position--;
                throw new EndOfStreamException();
            }

            byte[] readData = ReadBytes(lengthPrefix);
            return Encoding.GetEncoding(949).GetString(readData);
        }
        internal string ReadString16()
        {
            if (Position + 1 > Data.Length)
                throw new EndOfStreamException();

            int lengthPrefix = ReadUInt16();
            if (Position + lengthPrefix > Data.Length)
            {
                Position -= 2;
                throw new EndOfStreamException();
            }
            byte[] readData = ReadBytes(lengthPrefix);
            return Encoding.GetEncoding(949).GetString(readData);
        }
        internal byte[] ReadData8()
        {
            if (Position >= Data.Length)
                throw new EndOfStreamException();

            int lengthPrefix = ReadByte();
            if (Position + lengthPrefix > Data.Length)
            {
                Position--;
                throw new EndOfStreamException();
            }
            return ReadBytes(lengthPrefix);
        }
        internal byte[] ReadData16()
        {
            if (Position + 1 > Data.Length)
                throw new EndOfStreamException();

            int lengthPrefix = ReadUInt16();
            if (Position + lengthPrefix > Data.Length)
            {
                Position -= 2;
                throw new EndOfStreamException();
            }
            return ReadBytes(lengthPrefix);
        }
        internal Point ReadPoint()
        {
            if (Position + 4 > Data.Length)
                throw new EndOfStreamException();

            return new Point(ReadUInt16(), ReadUInt16());
        }
        internal void Write(byte[] buffer)
        {
            int resultLength = Position + buffer.Length;
            if (resultLength > Data.Length)
                Array.Resize(ref Data, resultLength);

            Buffer.BlockCopy(buffer, 0, Data, Position, buffer.Length);
            Position += buffer.Length;
        }

        internal void WriteByte(byte value) => Write(new byte[] { value });
        internal void WriteSByte(sbyte value) => Write(new byte[] { (byte)value });
        internal void WriteBoolean(bool value) => WriteByte((byte)(value ? 1 : 0));
        internal void WriteInt16(short value) => Write(new byte[] { (byte)(value >> 8), (byte)value });
        internal void WriteUInt16(ushort value) => Write(new byte[] { (byte)((uint)value >> 8), (byte)value });
        internal void WriteInt32(int value) => Write(new byte[] { (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value });
        internal void WriteUInt32(uint value) => Write(new byte[] { (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value });

        internal void WriteString(string value, bool terminate = false)
        {
            Write(Encoding.GetEncoding(949).GetBytes(value));
            if (terminate)
                WriteByte(10);
        }
        internal void WriteString8(string value)
        {
            byte[] writeData = Encoding.GetEncoding(949).GetBytes(value);
            if (writeData.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException("value", value, $@"String must be {byte.MaxValue} chars or less");

            WriteByte((byte)writeData.Length);
            Write(writeData);
        }
        internal void WriteString16(string value)
        {
            byte[] writeData = Encoding.GetEncoding(949).GetBytes(value);
            if (writeData.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("value", value, $@"String must be {ushort.MaxValue} chars or less");

            WriteUInt16((ushort)writeData.Length);
            Write(writeData);
        }
        internal void WritePoint8(Point value)
        {
            WriteByte((byte)value.X);
            WriteByte((byte)value.Y);
        }
        internal void WritePoint16(Point value)
        {
            WriteUInt16(value.X);
            WriteUInt16(value.Y);
        }
        internal void WriteData(byte[] value, bool terminate = false)
        {
            Write(value);
            if (terminate)
                WriteByte(0);
        }
        internal void WriteData8(byte[] value)
        {
            WriteByte((byte)value.Length);
            WriteData(value);
        }

        internal void WriteData16(byte[] value)
        {
            WriteUInt16((ushort)value.Length);
            WriteData(value);
        }
        internal byte[] ToArray()
        {
            int resultLength = Data.Length + (ShouldEncrypt ? 5 : 4) - 3;
            byte[] resultData = new byte[resultLength + 3];
            resultData[0] = Identifier;
            resultData[1] = (byte)(resultLength / 256);
            resultData[2] = (byte)(resultLength % 256);
            resultData[3] = OpCode;
            resultData[4] = Counter;
            Buffer.BlockCopy(Data, 0, resultData, resultData.Length - Data.Length, Data.Length);
            return resultData;
        }
        internal string GetHexString()
        {
            int resultLength = Data.Length + 1;
            byte[] resultData = new byte[resultLength];
            resultData[0] = OpCode;
            Buffer.BlockCopy(Data, 0, resultData, 1, Data.Length);
            return BitConverter.ToString(resultData).Replace('-', ' ');
        }
        internal string GetAsciiString(bool replaceNewline = true)
        {
            char[] resultString = new char[Data.Length + 1];
            byte[] buffer = new byte[Data.Length + 1];
            buffer[0] = OpCode;
            Buffer.BlockCopy(Data, 0, buffer, 1, Data.Length);
            for (int i = 0; i < buffer.Length; i++)
            {
                byte charCode = buffer[i];
                if ((charCode == 10 || charCode == 13) && !replaceNewline)
                    resultString[i] = (char)charCode;
                else if (charCode < 32 || charCode > 126)
                    resultString[i] = '.';
                else
                    resultString[i] = (char)charCode;
            }
            return new string(resultString);
        }
    }
}
