using System;

namespace Chaos
{
    internal sealed class ClientPacket : Packet
    {
        internal bool IsDialog => OpCode == 57 || OpCode == 58;

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
        internal ClientPacket(byte opcode) : base(opcode) { }
        internal ClientPacket(byte[] buffer) : base(buffer) { }

        internal void Decrypt(Crypto crypto)
        {
            byte[] key;
            int length = Data.Length - 7;
            ushort a = (ushort)((Data[length + 6] << 8 | Data[length + 4]) ^ 0x7470);
            byte b = (byte)(Data[length + 5] ^ 0x23);

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
                    Data[i] ^= crypto.Salt[Sequence];
            }
            ResizeArray(length);
        }
        internal void GenerateDialogHeader()
        {
            ushort num = CRC16.Calculate(Data, 6, Data.Length - 6);
            Data[0] = (byte)Utility.Random(0, 255);
            Data[1] = (byte)Utility.Random(0, 255);
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
                case "00":
                    return $"[Join] Recv> {GetHexString()}";
                case "02":
                    return $"[CreateA] Recv> {GetHexString()}";
                case "03":
                    return $"[Login] Recv> {GetHexString()}";
                case "04":
                    return $"[CreateB] Recv> {GetHexString()}";
                case "05":
                    return $"[RequestMapData] Recv> {GetHexString()}";
                case "06":
                    return $"[Walk] Recv> {GetHexString()}";
                case "07":
                    return $"[Pickup] Recv> {GetHexString()}";
                case "08":
                    return $"[Drop] Recv> {GetHexString()}";
                case "0B":
                    return $"[ClientExit] Recv> {GetHexString()}";
                case "0E":
                    return $"[PublicChat] Recv> {GetHexString()}";
                case "0F":
                    return $"[UseSpell] Recv> {GetHexString()}";
                case "10":
                    return $"[ClientJoin] Recv> {GetHexString()}";
                case "11":
                    return $"[Turn] Recv> {GetHexString()}";
                case "13":
                    return $"[Spacebar] Recv> {GetHexString()}";
                case "18":
                    return $"[RequestWorldList] Recv> {GetHexString()}";
                case "19":
                    return $"[Whisper] Recv> {GetHexString()}";
                case "1B":
                    return $"[UserOptions] Recv> {GetHexString()}";
                case "1C":
                    return $"[UseItem] Recv> {GetHexString()}";
                case "1D":
                    return $"[Emote] Recv> {GetHexString()}";
                case "24":
                    return $"[DropGold] Recv> {GetHexString()}";
                case "26":
                    return $"[ChangePassword] Recv> {GetHexString()}";
                case "29":
                    return $"[DropItemOnCreature] Recv> {GetHexString()}";
                case "2A":
                    return $"[DropGoldOnCreature] Recv> {GetHexString()}";
                case "2D":
                    return $"[ProfileRequest] Recv> {GetHexString()}";
                case "2E":
                    return $"[GroupRequest] Recv> {GetHexString()}";
                case "2F":
                    return $"[ToggleGroup] Recv> {GetHexString()}";
                case "30":
                    return $"[SwapSlot] Recv> {GetHexString()}";
                case "38":
                    return $"[RefreshRequest] Recv> {GetHexString()}";
                case "39":
                    return $"[Pursuit] Recv> {GetHexString()}";
                case "3A":
                    return $"[DialogResponse] Recv> {GetHexString()}";
                case "3B":
                    return $"[Board] Recv> {GetHexString()}";
                case "3E":
                    return $"[UseSkill] Recv> {GetHexString()}";
                case "3F":
                    return $"[ClickWorldMap] Recv> {GetHexString()}";
                case "43":
                    return $"[ClickObject] Recv> {GetHexString()}";
                case "44":
                    return $"[RemoveEquipment] Recv> {GetHexString()}";
                case "45":
                    return $"[HeartBeat] Recv> {GetHexString()}";
                case "47":
                    return $"[AdjustStat] Recv> {GetHexString()}";
                case "4A":
                    return $"[Exchange] Recv> {GetHexString()}";
                case "4B":
                    return $"[RequestNotification] Recv> {GetHexString()}";
                case "4D":
                    return $"[BeginChant] Recv> {GetHexString()}";
                case "4E":
                    return $"[Chant] Recv> {GetHexString()}";
                case "4F":
                    return $"[PortraitText] Recv> {GetHexString()}";
                case "57":
                    return $"[ServerTable] Recv> {GetHexString()}";
                case "68":
                    return $"[HomePage] Recv> {GetHexString()}";
                case "75":
                    return $"[HeartBeatTimer] Recv> {GetHexString()}";
                case "79":
                    return $"[SocialStatus] Recv> {GetHexString()}";
                case "7B":
                    return $"[MetafileRequest] Recv> {GetHexString()}";
                default:
                    return $"[**Unknown**] Recv> {GetHexString()}";
            }
        }
    }
}
