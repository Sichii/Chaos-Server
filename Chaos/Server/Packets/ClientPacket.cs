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

namespace Chaos
{
    /// <summary>
    /// Represents a packet sent by the client to the server. Contains methods used to analyze and decrypt them.
    /// </summary>
    internal sealed class ClientPacket : Packet
    {
        internal override bool ShouldEncrypt => OpCode != 0 && OpCode != 16 && OpCode != 72;
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
            ushort a = (ushort)(((Data[length + 6] << 8) | Data[length + 4]) ^ 29808);
            byte b = (byte)(Data[length + 5] ^ 35);
            byte[] key = (method == EncryptionType.Normal) ? crypto.Key 
                : (method == EncryptionType.MD5) ? crypto.GenerateKey(a, b) 
                : new byte[0];
            length -= (method == EncryptionType.Normal) ? 1 
                : (method == EncryptionType.MD5) ? 2 
                : 0;

            for (int i = 0; i < length; ++i)
            {
                byte saltI = (byte)(i / crypto.Key.Length);
                Data[i] ^= (byte)(crypto.Salts[saltI] ^ key[i % key.Length]);
                if (saltI != Sequence)
                    Data[i] ^= crypto.Salts[Sequence];
            }

            Array.Resize(ref Data, length);
        }

        internal void DecryptDialog()
        {
            byte num1 = (byte)(Data[1] ^ (uint)(byte)(Data[0] - 45U));
            byte num2 = (byte)(num1 + 114U);
            byte num3 = (byte)(num1 + 40U);

            Data[2] ^= num2;
            Data[3] ^= (byte)(num2 + 1);
            int num4 = (Data[2] << 8) | Data[3];
            for (int index = 0; index < num4; index++)
                Data[4 + index] ^= (byte)(num3 + index);

            Buffer.BlockCopy(Data, 6, Data, 0, Data.Length - 6);
            Array.Resize(ref Data, Data.Length - 6);
        }
        public override string ToString() => $@"Recv [{((ClientOpCodes)OpCode).ToString() ?? "**Unknown**"}] {GetHexString()}";
    }
}
