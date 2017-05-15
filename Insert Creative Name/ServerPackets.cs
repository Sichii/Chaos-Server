using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal static class ServerPackets
    {
        internal static ServerPacket LoginMessage
        {
            get { return new ServerPacket(2); }
        }
        internal static ServerPacket Redirect
        {
            get { return new ServerPacket(3); }
        }
        internal static ServerPacket Location
        {
            get { return new ServerPacket(4); }
        }
        internal static ServerPacket UserId
        {
            get { return new ServerPacket(5); }
        }
        internal static ServerPacket DisplayItemMonster
        {
            get { return new ServerPacket(7); }
        }
        internal static ServerPacket Attributes
        {
            get { return new ServerPacket(8); }
        }
        internal static ServerPacket SystemMessage
        {
            get { return new ServerPacket(10); }
        }
        internal static ServerPacket ClientWalk
        {
            get { return new ServerPacket(11); }
        }
        internal static ServerPacket CreatureWalk
        {
            get { return new ServerPacket(12); }
        }
        internal static ServerPacket PublicChat
        {
            get { return new ServerPacket(13); }
        }
        internal static ServerPacket RemoveObject
        {
            get { return new ServerPacket(14); }
        }
        internal static ServerPacket AddItem
        {
            get { return new ServerPacket(15); }
        }
        internal static ServerPacket RemoveItem
        {
            get { return new ServerPacket(16); }
        }
        internal static ServerPacket CreatureTurn
        {
            get { return new ServerPacket(17); }
        }
        internal static ServerPacket HealthBar
        {
            get { return new ServerPacket(19); }
        }
        internal static ServerPacket MapInfo
        {
            get { return new ServerPacket(21); }
        }
        internal static ServerPacket AddSpell
        {
            get { return new ServerPacket(23); }
        }
        internal static ServerPacket RemoveSpell
        {
            get { return new ServerPacket(24); }
        }
        internal static ServerPacket Sound
        {
            get { return new ServerPacket(25); }
        }
        internal static ServerPacket MapChangeComplete
        {
            get { return new ServerPacket(31); }
        }
        internal static ServerPacket RefreshResponse
        {
            get { return new ServerPacket(34); }
        }
        internal static ServerPacket Effect
        {
            get { return new ServerPacket(41); }
        }
        internal static ServerPacket AddSkill
        {
            get { return new ServerPacket(44); }
        }
        internal static ServerPacket RemoveSkill
        {
            get { return new ServerPacket(45); }
        }
        internal static ServerPacket WorldMap
        {
            get { return new ServerPacket(46); }
        }
        internal static ServerPacket MerchantMenu
        {
            get { return new ServerPacket(47); }
        }
        internal static ServerPacket Dialog
        {
            get { return new ServerPacket(48); }
        }
        internal static ServerPacket BulletinBoard
        {
            get { return new ServerPacket(49); }
        }
        internal static ServerPacket Door
        {
            get { return new ServerPacket(50); }
        }
        internal static ServerPacket DisplayUser
        {
            get { return new ServerPacket(51); }
        }
        internal static ServerPacket Profile
        {
            get { return new ServerPacket(52); }
        }
        internal static ServerPacket WorldList
        {
            get { return new ServerPacket(54); }
        }
        internal static ServerPacket AddEquipment
        {
            get { return new ServerPacket(55); }
        }
        internal static ServerPacket RemoveEquipment
        {
            get { return new ServerPacket(56); }
        }
        internal static ServerPacket ProfileSelf
        {
            get { return new ServerPacket(57); }
        }
        internal static ServerPacket SpellBar
        {
            get { return new ServerPacket(58); }
        }
        internal static ServerPacket HeartbeatA
        {
            get { return new ServerPacket(59); }
        }
        internal static ServerPacket MapData
        {
            get { return new ServerPacket(60); }
        }
        internal static ServerPacket Cooldown
        {
            get { return new ServerPacket(63); }
        }
        internal static ServerPacket Exchange
        {
            get { return new ServerPacket(66); }
        }
        internal static ServerPacket CancelCasting
        {
            get { return new ServerPacket(72); }
        }
        internal static ServerPacket MapLoadComplete
        {
            get { return new ServerPacket(88); }
        }
        internal static ServerPacket LobbyMessage
        {
            get { return new ServerPacket(96); }
        }
        internal static ServerPacket Website
        {
            get { return new ServerPacket(102); }
        }
        internal static ServerPacket MapChangePending
        {
            get { return new ServerPacket(103); }
        }
        internal static ServerPacket HeartbeatB
        {
            get { return new ServerPacket(104); }
        }
        internal static ServerPacket Metafile
        {
            get { return new ServerPacket(111); }
        }
    }
}
