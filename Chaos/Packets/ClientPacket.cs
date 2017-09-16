using System;

namespace Chaos
{
    internal sealed class ClientPacket : Packet
    {
        internal bool IsDialog => OpCode == 57 || OpCode == 58;

        internal override EncryptionType EncryptionType
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
                        return EncryptionType.Normal;
                    case 0:
                    case 16:
                    case 72:
                        return EncryptionType.None;
                    default:
                        return EncryptionType.MD5;
                }
            }
        }
        internal ClientPacket(byte[] buffer) : base(buffer) { }

        internal void Decrypt(Crypto crypto)
        {
            EncryptionType method = EncryptionType;
            int length = Data.Length - 7;
            ushort a = (ushort)((Data[length + 6] << 8 | Data[length + 4]) ^ 29808);
            byte b = (byte)(Data[length + 5] ^ 35);
            byte[] key = method == EncryptionType.Normal ? crypto.Key : method == EncryptionType.MD5 ? crypto.GenerateKey(a, b) : new byte[0];
            length -= method == EncryptionType.Normal ? 1 : method == EncryptionType.MD5 ? 2 : 0;

            for (int i = 0; i < length; ++i)
            {
                int x = (i / crypto.Key.Length) % 256;
                Data[i] ^= (byte)(crypto.Salts[x] ^ key[i % key.Length]);
                if (x != Counter)
                    Data[i] ^= crypto.Salts[Counter];
            }

            Array.Resize(ref Data, length);
        }
        internal void GenerateDialogHeader()
        {
            ushort CheckSum = Crypto.Generate16(Data, 6, Data.Length - 6);
            Data[0] = (byte)Utility.Random(0, 255);
            Data[1] = (byte)Utility.Random(0, 255);
            Data[2] = (byte)((Data.Length - 4) / 256);
            Data[3] = (byte)((Data.Length - 4) % 256);
            Data[4] = (byte)(CheckSum / 256);
            Data[5] = (byte)(CheckSum % 256);
        }
        internal void DecryptDialog()
        {
            byte num1 = (byte)(Data[1] ^ (uint)(byte)(Data[0] - 45));
            byte num2 = (byte)(num1 + 114);
            byte num3 = (byte)(num1 + 40);
            Data[2] ^= num2;
            Data[3] ^= (byte)((num2 + 1) % 256);
            int num4 = Data[2] << 8 | Data[3];
            for (int index = 0; index < num4; ++index)
                Data[4 + index] ^= (byte)((num3 + index) % 256);

            Buffer.BlockCopy(Data, 6, Data, 0, Data.Length - 6);
            Array.Resize(ref Data, Data.Length - 6);
        }
        public override string ToString() => $@"Recv [{Enum.GetName(typeof(ClientOpCodes), OpCode) ?? "**Unknown**"}] {GetHexString()}";
    }
}
