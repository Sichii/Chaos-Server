using System;
namespace Insert_Creative_Name
{
    internal sealed class ServerPacket : Packet
    {
        internal override EncryptMethod EncryptMethod
        {
            get
            {
                switch (OpCode)
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
        internal void Encrypt(Crypto crypto)
        {
            Position = Data.Length;
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
            for (int index1 = 0; index1 < Data.Length; ++index1)
            {
                int index2 = index1 / crypto.Key.Length % 256;
                Data[index1] ^= (byte)(crypto.Salt[index2] ^ (uint)numArray[index1 % numArray.Length]);
                if (index2 != Sequence)
                    Data[index1] ^= crypto.Salt[Sequence];
            }
            WriteByte((byte)(a % 256 ^ 116));
            WriteByte((byte)(b ^ 36U));
            WriteByte((byte)((a >> 8) % 256 ^ 100));
        }

        internal ServerPacket Copy()
        {
            ServerPacket serverPacket = new ServerPacket(OpCode);
            serverPacket.Write(Data);
            serverPacket.TimeStamp = TimeStamp;
            return serverPacket;
        }
        public override string ToString()
        {
            switch (GetHexString().Substring(0, 2))
            {
                case "03":
                    return $"[Redirect] Send> {GetHexString()}";
                case "05":
                    return $"[UserID] Send> {GetHexString()}";
                case "07":
                    return $"[Display] Send> {GetHexString()}";
                case "08":
                    return $"[Attrib] Send> {GetHexString()}";
                case "0A":
                    return $"[SysMsg] Send> {GetHexString()}";
                case "0B":
                    return $"[ClientWalk] Send> {GetHexString()}";
                case "0C":
                    return $"[CreatureWalk] Send> {GetHexString()}";
                case "0D":
                    return $"[Chat] Send> {GetHexString()}";
                case "0E":
                    return $"[RemoveObj] Send> {GetHexString()}";
                case "0F":
                    return $"[AddItem] Send> {GetHexString()}";
                case "10":
                    return $"[RemItem] Send> {GetHexString()}";
                case "11":
                    return $"[CreatureTurn] Send> {GetHexString()}";
                case "13":
                    return $"[HPBar] Send> {GetHexString()}";
                case "15":
                    return $"[MapInfo] Send> {GetHexString()}";
                case "17":
                    return $"[AddSpell] Send> {GetHexString()}";
                case "18":
                    return $"[RemSpell] Send> {GetHexString()}";
                case "19":
                    return $"[Sound] Send> {GetHexString()}";
                case "1F":
                    return $"[MapChange] Send> {GetHexString()}";
                case "1A":
                    return $"[CreatureAnim] Send> {GetHexString()}";
                case "22":
                    return $"[RefreshR] Send> {GetHexString()}";
                case "29":
                    return $"[SpellAni] Send> {GetHexString()}";
                case "2C":
                    return $"[AddSkill] Send> {GetHexString()}";
                case "2D":
                    return $"[RemSkill] Send> {GetHexString()}";
                case "2E":
                    return $"[WorldMap] Send> {GetHexString()}";
                case "2F":
                    return $"[MerchMenu] Send> {GetHexString()}";
                case "30":
                    return $"[Pursuit] Send> {GetHexString()}";
                case "31":
                    return $"[Board] Send> {GetHexString()}";
                case "32":
                    return $"[ClientWalkF] Send> {GetHexString()}";
                case "33":
                    return $"[DisplayUser] Send> {GetHexString()}";
                case "34":
                    return $"[ProfileUser] Send> {GetHexString()}";
                case "36":
                    return $"[Userlist] Send> {GetHexString()}";
                case "37":
                    return $"[AddEquip] Send> {GetHexString()}";
                case "38":
                    return $"[RemEquip] Send> {GetHexString()}";
                case "39":
                    return $"[ProfileSelf] Send> {GetHexString()}";
                case "3A":
                    return $"[SpellBar] Send> {GetHexString()}";
                case "3B":
                    return $"[HeartBeatA] Send> {GetHexString()}";
                case "3C":
                    return $"[MapData] Send> {GetHexString()}";
                case "3F":
                    return $"[Cooldown] Send> {GetHexString()}";
                case "42":
                    return $"[Exchange] Send> {GetHexString()}";
                case "48":
                    return $"[CancelCast] Send> {GetHexString()}";
                case "58":
                    return $"[MapLoad] Send> {GetHexString()}";
                case "60":
                    return $"[Notif] Send> {GetHexString()}";
                case "67":
                    return $"[MapChangePend] Send> {GetHexString()}";
                case "68":
                    return $"[HeartBeatB] Send> {GetHexString()}";
                case "6F":
                    return $"[Metafile] Send> {GetHexString()}";
                default:
                    return $"[**Unknown**] Send> {GetHexString()}";
            }
        }
    }
}
