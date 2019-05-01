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
    /// Represents a packet sent by the server, to the client. Contains methods used to analyze and encrypt them.
    /// </summary>
    internal sealed class ServerPacket : Packet
    {
        internal string LogString { get; set; }
        internal override bool ShouldEncrypt => OpCode != 0 && OpCode != 3 && OpCode != 64 && OpCode != 126;

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
        internal void Encrypt(Crypto crypto)
        {
            EncryptionType method = EncryptionType;
            int pos = Data.Length;
            ushort a = (ushort)Utilities.Random(257, ushort.MaxValue);
            byte b = (byte)Utilities.Random(101, byte.MaxValue);
            byte[] key = (method == EncryptionType.Normal) ? crypto.Key 
                : (method == EncryptionType.MD5) ? crypto.GenerateKey(a, b) 
                : new byte[1024];

            for (int i = 0; i < Data.Length; ++i)
            {
                byte saltI = (byte)(i / crypto.Key.Length);
                Data[i] ^= (byte)(crypto.Salts[saltI] ^ (uint)key[i % key.Length]);
                if (saltI != Sequence)
                    Data[i] ^= crypto.Salts[Sequence];
            }

            Array.Resize(ref Data, pos + 3);
            Data[pos++] = (byte)((a % 256) ^ 116);
            Data[pos++] = (byte)(b ^ 36U);
            Data[pos++] = (byte)(((a >> 8) % 256) ^ 100);
        }
        public override string ToString() => $@"Send [{((ServerOpCodes)OpCode).ToString() ?? "**Unknown**"}] {GetHexString()}";
    }
}
