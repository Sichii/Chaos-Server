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
using System.Security.Cryptography;

namespace Chaos
{
    internal sealed class ServerPacket : Packet
    {
        internal override EncryptionType EncryptionType
        {
            get
            {
                switch (OpCode)
                {
                    case 0:
                    case 3:
                    case 64:
                    case 126:
                        return EncryptionType.None;
                    case 1:
                    case 2:
                    case 10:
                    case 86:
                    case 96:
                    case 98:
                    case 102:
                    case 111:
                        return EncryptionType.Normal;
                    default:
                        return EncryptionType.MD5;
                }
            }
        }
        internal ServerPacket(ServerOpCodes opcode) : base((byte)opcode) { }
        internal ServerPacket(byte[] buffer) : base(buffer) { }
        internal void Encrypt(Crypto crypto)
        {
            EncryptionType method = EncryptionType;
            int pos = Data.Length;
            ushort a = (ushort)(Utility.Random(0, 65277) + 256);
            byte b = (byte)(Utility.Random(0, 155) + 100);
            byte[] key = method == EncryptionType.Normal ? crypto.Key : method == EncryptionType.MD5 ? crypto.GenerateKey(a, b) : new byte[0];

            for (int i = 0; i < Data.Length; ++i)
            {
                int saltI = i / crypto.Key.Length % 256;
                Data[i] ^= (byte)(crypto.Salts[saltI] ^ (uint)key[i % key.Length]);
                if (saltI != Counter)
                    Data[i] ^= crypto.Salts[Counter];
            }

            Array.Resize(ref Data, Data.Length + 3);
            Data[pos++] = (byte)(a % 256 ^ 116);
            Data[pos++] = (byte)(b ^ 36U);
            Data[pos++] = (byte)((a >> 8) % 256 ^ 100);
        }
        public override string ToString() => $@"Send [{Enum.GetName(typeof(ServerOpCodes), OpCode) ?? "**Unknown**"}] {GetHexString()}";
    }
}
