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
                return OpCode == 57 || OpCode == 58;
            }
        }
        internal override EncryptMethod EncryptMethod
        {
            get
            {
                switch(OpCode)
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

        internal void Decrypt(Crypto crypto)
        {
            int length = Data.Length - 7;

            ushort a = (ushort)((Data[length + 6] << 8 | Data[length + 4]) ^ 0x7470);
            byte b = (byte)(Data[length + 5] ^ 0x23);

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

                Data[i] ^= (byte)(crypto.Salt[saltIndex] ^ key[i % key.Length]);

                if (saltIndex != Sequence)
                {
                    Data[i] ^= crypto.Salt[Sequence];
                }
            }

            ResizeArray(length);
        }
        internal void GenerateDialogHeader()
        {
            ushort num = CRC16.Calculate(Data, 6, Data.Length - 6);
            Data[0] = (byte)Utility.Random();
            Data[1] = (byte)Utility.Random();
            Data[2] = (byte)((Data.Length - 4) / 256);
            Data[3] = (byte)((Data.Length - 4) % 256);
            Data[4] = (byte)(num / 256);
            Data[5] = (byte)(num % 256);
        }
        internal void EncryptDialog()
        {
            ResizeArray(Data.Length + 6);
            Buffer.BlockCopy(Data, 0, Data, 6, Data.Length - 6);
            GenerateDialogHeader();
            int num1 = Data[2] << 8 | Data[3];
            byte num2 = (byte)(Data[1] ^ (uint)(byte)(Data[0] - 45U));
            byte num3 = (byte)(num2 + 114U);
            byte num4 = (byte)(num2 + 40U);
            Data[2] ^= num3;
            Data[3] ^= (byte)((num3 + 1) % 256);
            for (int index = 0; index < num1; ++index)
                Data[4 + index] ^= (byte)((num4 + index) % 256);
        }
        internal void DecryptDialog()
        {
            byte num1 = (byte)(Data[1] ^ (uint)(byte)(Data[0] - 45U));
            byte num2 = (byte)(num1 + 114U);
            byte num3 = (byte)(num1 + 40U);
            Data[2] ^= num2;
            Data[3] ^= (byte)((num2 + 1) % 256);
            int num4 = Data[2] << 8 | Data[3];
            for (int index = 0; index < num4; ++index)
                Data[4 + index] ^= (byte)((num3 + index) % 256);
            Buffer.BlockCopy(Data, 6, Data, 0, Data.Length - 6);
            ResizeArray(Data.Length - 6);
        }
        internal ClientPacket Copy()
        {
            ClientPacket clientPacket = new ClientPacket(OpCode);
            clientPacket.Write(Data);
            clientPacket.TimeStamp = TimeStamp;
            return clientPacket;
        }
        public override string ToString()
        {
            switch (GetHexString().Substring(0, 2))
            {
                case "06":
                    return $"[Walk] Recv> {GetHexString()}";
                case "11":
                    return $"[Turn] Recv> {GetHexString()}";
                case "07":
                    return $"[Pickup] Recv> {GetHexString()}";
                case "0E":
                    return $"[Chat] Recv> {GetHexString()}";
                case "19":
                    return $"[Whisper] Recv> {GetHexString()}";
                case "0F":
                    return $"[Spell] Recv> {GetHexString()}";
                case "18":
                    return $"[Worldlist] Recv> {GetHexString()}";
                case "1C":
                    return $"[Item] Recv> {GetHexString()}";
                case "30":
                    return $"[Slot] Recv> {GetHexString()}";
                case "39":
                    return $"[Gossip] Recv> {GetHexString()}";
                case "3A":
                    return $"[Pursuit] Recv> {GetHexString()}";
                case "3B":
                    return $"[Board] Recv> {GetHexString()}";
                case "43":
                    return $"[ObjClick] Recv> {GetHexString()}";
                case "47":
                    return $"[Stat] Recv> {GetHexString()}";
                case "4A":
                    return $"[Exchange] Recv> {GetHexString()}";
                case "1B":
                    return $"[UOptions] Recv> {GetHexString()}";
                case "1D":
                    return $"[SendEmote] Recv> {GetHexString()}";
                case "3F":
                    return $"[ClickMap] Recv> {GetHexString()}";
                default:
                    return $"[**Unknown**] Recv> {GetHexString()}";
            }
        }
    }
}
