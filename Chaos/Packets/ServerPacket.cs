using System;

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
        internal ServerPacket(byte opcode) : base(opcode) { }
        internal ServerPacket(byte[] buffer) : base(buffer) { }
        internal void Encrypt(Crypto crypto)
        {
            EncryptionType type = EncryptionType;
            int pos = Data.Length;

            Array.Resize(ref Data, Data.Length + (type == EncryptionType.MD5 ? 5 : 4));

            Data[pos++] = 0;
            if (type == EncryptionType.MD5)
                Data[pos++] = OpCode;

            ushort a = (ushort)(Utility.Random(0, 65277) + 256);
            byte b = (byte)(Utility.Random(0, 155) + 100);
            byte[] key = type == EncryptionType.Normal ? crypto.Key : type == EncryptionType.MD5 ? crypto.GenerateKey(a, b) : new byte[0];

            for (int i = 0; i < Data.Length - 3; i++)
            {
                int saltI = i / crypto.Key.Length % 256;
                Data[i] ^= (byte)(crypto.Salts[saltI] ^ (uint)key[i % key.Length]);
                if (saltI != Counter)
                    Data[i] ^= crypto.Salts[Counter];
            }

            Data[pos++] = (byte)(a % 256 ^ 116);
            Data[pos++] = (byte)(b ^ 36U);
            Data[pos++] = (byte)((a >> 8) % 256 ^ 100);
        }

        internal ServerPacket Copy()
        {
            ServerPacket serverPacket = new ServerPacket(OpCode);
            serverPacket.Write(Data);
            serverPacket.Creation = Creation;
            return serverPacket;
        }
        public override string ToString()
        {
            switch (GetHexString().Substring(0, 2))
            {
                case "00":
                    return $"[ConnectionInfo] Send> {GetHexString()}";
                case "02":
                    return $"[LobbyMessage] Send> {GetHexString()}";
                case "03":
                    return $"[Redirect] Send> {GetHexString()}";
                case "04":
                    return $"[Location] Send> {GetHexString()}";
                case "05":
                    return $"[UserID] Send> {GetHexString()}";
                case "07":
                    return $"[DisplayItemMonster] Send> {GetHexString()}";
                case "08":
                    return $"[Attributes] Send> {GetHexString()}";
                case "0A":
                    return $"[SysMsg] Send> {GetHexString()}";
                case "0B":
                    return $"[ClientWalk] Send> {GetHexString()}";
                case "0C":
                    return $"[CreatureWalk] Send> {GetHexString()}";
                case "0D":
                    return $"[PublicChat] Send> {GetHexString()}";
                case "0E":
                    return $"[RemoveObject] Send> {GetHexString()}";
                case "0F":
                    return $"[AddItem] Send> {GetHexString()}";
                case "10":
                    return $"[RemoveItem] Send> {GetHexString()}";
                case "11":
                    return $"[CreatureTurn] Send> {GetHexString()}";
                case "13":
                    return $"[HealthBar] Send> {GetHexString()}";
                case "15":
                    return $"[MapInfo] Send> {GetHexString()}";
                case "17":
                    return $"[AddSpell] Send> {GetHexString()}";
                case "18":
                    return $"[RemSpell] Send> {GetHexString()}";
                case "19":
                    return $"[Sound] Send> {GetHexString()}";
                case "1A":
                    return $"[CreatureAnimation] Send> {GetHexString()}";
                case "1F":
                    return $"[MapChangeComplete] Send> {GetHexString()}";
                case "20":
                    return $"[LightLevel] Send> {GetHexString()}";
                case "22":
                    return $"[RefreshResponse] Send> {GetHexString()}";
                case "29":
                    return $"[Animation] Send> {GetHexString()}";
                case "2C":
                    return $"[AddSkill] Send> {GetHexString()}";
                case "2D":
                    return $"[RemoveSkill] Send> {GetHexString()}";
                case "2E":
                    return $"[WorldMap] Send> {GetHexString()}";
                case "2F":
                    return $"[MerchantMenu] Send> {GetHexString()}";
                case "30":
                    return $"[Dialog] Send> {GetHexString()}";
                case "31":
                    return $"[Board] Send> {GetHexString()}";
                case "32":
                    return $"[Door] Send> {GetHexString()}";
                case "33":
                    return $"[DisplayUser] Send> {GetHexString()}";
                case "34":
                    return $"[Profile] Send> {GetHexString()}";
                case "36":
                    return $"[WorldList] Send> {GetHexString()}";
                case "37":
                    return $"[AddEquipment] Send> {GetHexString()}";
                case "38":
                    return $"[RemoveEquipment] Send> {GetHexString()}";
                case "39":
                    return $"[ProfileSelf] Send> {GetHexString()}";
                case "3A":
                    return $"[EffectsBar] Send> {GetHexString()}";
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
                case "56":
                    return $"[ServerTable] Send> {GetHexString()}";
                case "58":
                    return $"[MapLoadComplete] Send> {GetHexString()}";
                case "60":
                    return $"[LobbyNotification] Send> {GetHexString()}";
                case "63":
                    return $"[GroupRequest] Send> {GetHexString()}";
                case "66":
                    return $"[LobbyControls] Send> {GetHexString()}";
                case "67":
                    return $"[MapChangePending] Send> {GetHexString()}";
                case "68":
                    return $"[HeartBeatB] Send> {GetHexString()}";
                case "6F":
                    return $"[Metafile] Send> {GetHexString()}";
                case "7E":
                    return $"[AcceptConnection] Send> {GetHexString()}";
                default:
                    return $"[**Unknown**] Send> {GetHexString()}";
            }
        }
    }
}
