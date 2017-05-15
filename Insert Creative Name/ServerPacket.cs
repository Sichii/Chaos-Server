using System;
namespace Insert_Creative_Name
{
    internal sealed class ServerPacket : Packet
    {
        internal override EncryptMethod EncryptMethod
        {
            get
            {
                switch (opCode)
                {
                    case 0:
                    case 3:
                    case 64:
                    case 126:
                        return EncryptMethod.None;
                    case 1:
                    case 2:
                    case 10:
                    case 86:
                    case 96:
                    case 98:
                    case 102:
                    case 111:
                        return EncryptMethod.Normal;
                    default:
                        return EncryptMethod.MD5Key;
                }
            }
        }
        internal ServerPacket(byte opcode) : base(opcode) { }
        internal ServerPacket(byte[] buffer) : base(buffer) { }
        internal override void Encrypt(Crypto crypto)
        {
            position = data.Length;
            ushort a = (ushort)(Utility.Random(65277) + 256);
            byte b = (byte)(Utility.Random(155) + 100);
            byte[] numArray;
            switch (EncryptMethod)
            {
                case EncryptMethod.Normal:
                    numArray = crypto.Key;
                    break;
                case EncryptMethod.MD5Key:
                    numArray = crypto.GenerateKey(a, b);
                    break;
                default:
                    return;
            }
            for (int index1 = 0; index1 < data.Length; ++index1)
            {
                int index2 = index1 / crypto.Key.Length % 256;
                data[index1] ^= (byte)(crypto.Salt[index2] ^ (uint)numArray[index1 % numArray.Length]);
                if (index2 != sequence)
                    data[index1] ^= crypto.Salt[sequence];
            }
            WriteByte((byte)(a % 256 ^ 116));
            WriteByte((byte)(b ^ 36U));
            WriteByte((byte)((a >> 8) % 256 ^ 100));
        }
        internal override void Decrypt(Crypto crypto)
        {
            int newSize = data.Length - 3;
            ushort a = (ushort)((data[newSize + 2] << 8 | data[newSize]) ^ 25716);
            byte b = (byte)(data[newSize + 1] ^ 36U);
            byte[] numArray;
            switch (EncryptMethod)
            {
                case EncryptMethod.Normal:
                    numArray = crypto.Key;
                    break;
                case EncryptMethod.MD5Key:
                    numArray = crypto.GenerateKey(a, b);
                    break;
                default:
                    return;
            }
            for (int index1 = 0; index1 < newSize; ++index1)
            {
                int index2 = index1 / crypto.Key.Length % 256;
                data[index1] ^= (byte)(crypto.Salt[index2] ^ (uint)numArray[index1 % numArray.Length]);
                if (index2 != sequence)
                    data[index1] ^= crypto.Salt[sequence];
            }
            Array.Resize<byte>(ref data, newSize);
        }
        internal ServerPacket Copy()
        {
            ServerPacket serverPacket = new ServerPacket(opCode);
            serverPacket.Write(data);
            serverPacket.timeStamp = timeStamp;
            return serverPacket;
        }
        public override string ToString()
        {
            if (base.GetHexString().StartsWith("03"))
                return string.Format("[Redirect] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("05"))
                return string.Format("[UserID] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("07"))
                return string.Format("[Display] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("08"))
                return string.Format("[Attrib] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("0A"))
                return string.Format("[SysMsg] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("0B"))
                return string.Format("[ClientWalk] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("0C"))
                return string.Format("[CreatureWalk] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("0D"))
                return string.Format("[Chat] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("0E"))
                return string.Format("[RemoveObj] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("0F"))
                return string.Format("[AddItem] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("10"))
                return string.Format("[RemItem] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("11"))
                return string.Format("[CreatureTurn] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("13"))
                return string.Format("[HPBar] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("15"))
                return string.Format("[MapInfo] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("17"))
                return string.Format("[AddSpell] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("18"))
                return string.Format("[RemSpell] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("19"))
                return string.Format("[Sound] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("1F"))
                return string.Format("[MapChange] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("1A"))
                return string.Format("[RecvEmote] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("22"))
                return string.Format("[RefreshR] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("29"))
                return string.Format("[SpellAni] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("2C"))
                return string.Format("[AddSkill] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("2D"))
                return string.Format("[RemSkill] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("2E"))
                return string.Format("[WorldMap] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("2F"))
                return string.Format("[MerchMenu] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("30"))
                return string.Format("[Pursuit] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("31"))
                return string.Format("[Board] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("32"))
                return string.Format("[ClientWalkF] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("33"))
                return string.Format("[DisplayUser] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("34"))
                return string.Format("[ProfileUser] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("36"))
                return string.Format("[Userlist] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("37"))
                return string.Format("[AddEquip] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("38"))
                return string.Format("[RemEquip] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("39"))
                return string.Format("[ProfileSelf] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("3A"))
                return string.Format("[SpellBar] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("3B"))
                return string.Format("[Ping(a)] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("3C"))
                return string.Format("[MapData] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("3F"))
                return string.Format("[Cooldown] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("42"))
                return string.Format("[Exchange] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("48"))
                return string.Format("[CancelCast] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("58"))
                return string.Format("[MapLoad] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("60"))
                return string.Format("[Notif] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("67"))
                return string.Format("[MapChangePend] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("68"))
                return string.Format("[Ping(b)] Recv> {0}", base.GetHexString());
            else if (base.GetHexString().StartsWith("6F"))
                return string.Format("[Metafile] Recv> {0}", base.GetHexString());
            else
                return string.Format("[**Unknown**] Recv> {0}", base.GetHexString());
        }
    }
}
