using System;
using System.Security.Cryptography;

namespace Insert_Creative_Name
{
    internal sealed class ClientPacket : Packet
    {
        internal bool IsDialog
        {
            get
            {
                return opCode == 57 || opCode == 58;
            }
        }
        internal override EncryptMethod EncryptMethod
        {
            get
            {
                switch(opCode)
                {

                    case 2:
                    case 3:
                    case 4:
                    case 11:
                    case 38:
                    case 45:
                    case 58:
                    case 66:
                    case 67:
                    case 75:
                    case 87:
                    case 98:
                    case 104:
                    case 113:
                    case 115:
                    case 123:
                        return EncryptMethod.Normal;
                    case 0:
                    case 16:
                    case 72:
                        return EncryptMethod.None;
                    default:
                        return EncryptMethod.MD5Key;
                }
            }
        }
        internal ClientPacket(byte opcode) : base(opcode)
        {
        }
        internal ClientPacket(byte[] buffer) : base(buffer)
        {
        }
        internal override void Encrypt(Crypto crypto)
        {
            byte[] cryptoKey;
            position = data.Length;

            ushort a = (ushort)(Utility.Random(65277) + 256);
            byte b = (byte)(Utility.Random(155) + 100);

            switch (EncryptMethod)
            {
                case EncryptMethod.Normal:
                    WriteByte(0);
                    cryptoKey = crypto.Key;
                    break;
                case EncryptMethod.MD5Key:
                    WriteByte(0);
                    WriteByte(opCode);
                    cryptoKey = crypto.GenerateKey(a, b);
                    break;
                default:
                    return;
            }

            for (int i = 0; i < data.Length; ++i)
            {
                int saltIndex = i / crypto.Key.Length % 256;

                data[i] ^= (byte)(crypto.Salt[saltIndex] ^ (uint)cryptoKey[i % cryptoKey.Length]);

                if (saltIndex != sequence)
                {
                    data[i] ^= crypto.Salt[sequence];
                }
            }

            byte[] bytesToHash = new byte[data.Length + 2];
            bytesToHash[0] = opCode;
            bytesToHash[1] = sequence;
            Buffer.BlockCopy(data, 0, bytesToHash, 2, data.Length);

            var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(bytesToHash);

            WriteByte(hash[13]);
            WriteByte(hash[3]);
            WriteByte(hash[11]);
            WriteByte(hash[7]);

            WriteByte((byte)(a % 256 ^ 0x70));
            WriteByte((byte)(b ^ 0x23));
            WriteByte((byte)((a >> 8) % 256 ^ 0x74));
        }
        internal override void Decrypt(Crypto crypto)
        {
            int length = data.Length - 7;

            ushort a = (ushort)((data[length + 6] << 8 | data[length + 4]) ^ 0x7470);
            byte b = (byte)(data[length + 5] ^ 0x23);

            byte[] key;

            switch (EncryptMethod)
            {
                case EncryptMethod.Normal:
                    length--;
                    key = crypto.Key;
                    break;
                case EncryptMethod.MD5Key:
                    length -= 2;
                    key = crypto.GenerateKey(a, b);
                    break;
                default:
                    return;
            }

            for (int i = 0; i < length; ++i)
            {
                int saltIndex = (i / crypto.Key.Length) % 256;

                data[i] ^= (byte)(crypto.Salt[saltIndex] ^ key[i % key.Length]);

                if (saltIndex != sequence)
                {
                    data[i] ^= crypto.Salt[sequence];
                }
            }

            Array.Resize(ref data, length);
        }
        internal void GenerateDialogHeader()
        {
            ushort num = CRC16.Calculate(data, 6, data.Length - 6);
            data[0] = (byte)Utility.Random();
            data[1] = (byte)Utility.Random();
            data[2] = (byte)((data.Length - 4) / 256);
            data[3] = (byte)((data.Length - 4) % 256);
            data[4] = (byte)(num / 256);
            data[5] = (byte)(num % 256);
        }
        internal void EncryptDialog()
        {
            Array.Resize<byte>(ref data, data.Length + 6);
            Buffer.BlockCopy(data, 0, data, 6, data.Length - 6);
            GenerateDialogHeader();
            int num1 = data[2] << 8 | data[3];
            byte num2 = (byte)(data[1] ^ (uint)(byte)(data[0] - 45U));
            byte num3 = (byte)(num2 + 114U);
            byte num4 = (byte)(num2 + 40U);
            data[2] ^= num3;
            data[3] ^= (byte)((num3 + 1) % 256);
            for (int index = 0; index < num1; ++index)
                data[4 + index] ^= (byte)((num4 + index) % 256);
        }
        internal void DecryptDialog()
        {
            byte num1 = (byte)(data[1] ^ (uint)(byte)(data[0] - 45U));
            byte num2 = (byte)(num1 + 114U);
            byte num3 = (byte)(num1 + 40U);
            data[2] ^= num2;
            data[3] ^= (byte)((num2 + 1) % 256);
            int num4 = data[2] << 8 | data[3];
            for (int index = 0; index < num4; ++index)
                data[4 + index] ^= (byte)((num3 + index) % 256);
            Buffer.BlockCopy(data, 6, data, 0, data.Length - 6);
            Array.Resize<byte>(ref data, data.Length - 6);
        }
        internal ClientPacket Copy()
        {
            ClientPacket clientPacket = new ClientPacket(opCode);
            clientPacket.Write(data);
            clientPacket.timeStamp = timeStamp;
            return clientPacket;
        }
        public override string ToString()
        {
            if (GetHexString().StartsWith("06"))
                return $"[Walk] Send> {GetHexString()}";
            if (GetHexString().StartsWith("11"))
                return $"[Turn] Send> {GetHexString()}";
            if (GetHexString().StartsWith("07"))
                return $"[Pickup] Send> {GetHexString()}";
            if (GetHexString().StartsWith("0E"))
                return $"[Chat] Send> {GetHexString()}";
            if (GetHexString().StartsWith("19"))
                return $"[Whisper] Send> {GetHexString()}";
            if (GetHexString().StartsWith("0F"))
                return $"[Spell] Send> {GetHexString()}";
            if (GetHexString().StartsWith("18"))
                return $"[Worldlist] Send> {GetHexString()}";
            if (GetHexString().StartsWith("1C"))
                return $"[Item] Send> {GetHexString()}";
            if (GetHexString().StartsWith("30"))
                return $"[Slot] Send> {GetHexString()}";
            if (GetHexString().StartsWith("39"))
                return $"[Gossip] Send> {GetHexString()}";
            if (GetHexString().StartsWith("3A"))
                return $"[Pursuit] Send> {GetHexString()}";
            if (GetHexString().StartsWith("3B"))
                return $"[Board] Send> {GetHexString()}";
            if (GetHexString().StartsWith("43"))
                return $"[ObjClick] Send> {GetHexString()}";
            if (GetHexString().StartsWith("47"))
                return $"[Stat] Send> {GetHexString()}";
            if (GetHexString().StartsWith("4A"))
                return $"[Exchange] Send> {GetHexString()}";
            if (GetHexString().StartsWith("1B"))
                return $"[UOptions] Send> {GetHexString()}";
            if (GetHexString().StartsWith("1D"))
                return $"[SendEmote] Send> {GetHexString()}";
            return GetHexString().StartsWith("3F") ? $"[ClickMap] Send> {GetHexString()}" : $"[Unknown] Send> {GetHexString()}";
        }
    }
}
