using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal class ServerPackets
    {
        internal ServerPackets() { }

        internal ServerPacket LobbyMessage(byte type, string message)
        {
            //type is the type of dialog box, message is what's in it
            var packet = new ServerPacket(2);
            packet.WriteByte(type);
            packet.WriteString8(message);

            return packet;
        }
        internal ServerPacket Redirect(IPAddress address, short port, byte length, byte seed, byte key, string name, int id)
        {
            var packet = new ServerPacket(3);
            packet.Write(address.GetAddressBytes());
            packet.WriteInt16(port);
            packet.WriteByte(length);
            packet.WriteByte(seed);
            packet.WriteByte(key);
            packet.WriteString8(name);
            packet.WriteInt32(id);

            return packet;
        }
        internal ServerPacket Location(Point point)
        {
            var packet = new ServerPacket(4);
            packet.WritePoint(point);

            return packet;
        }
        internal ServerPacket UserId(uint userId, TemClass userClass)
        {
            var packet = new ServerPacket(5);
            packet.WriteUInt32(userId);
            packet.WriteInt16(0);//dunno
            packet.WriteByte((byte)userClass);
            packet.WriteInt16(0);//dunno

            return packet;
        }
        internal ServerPacket DisplayItemMonster(List<Objects.VisibleObject> objects)
        {
            var packet = new ServerPacket(7);
            packet.WriteUInt16((ushort)objects.Count);
            foreach(var obj in objects)
            {
                packet.WritePoint(obj.Point);
                packet.WriteUInt32(obj.Id);
                packet.WriteUInt16(obj.Sprite);
                if(obj.Sprite < 32768) //monster sprites
                {
                    Objects.Creature newObj = obj as Objects.Creature;
                    packet.Write(new byte[4]); //dunno
                    packet.WriteByte((byte)newObj.Direction);
                    packet.WriteByte(0); //dunno
                    packet.WriteByte(newObj.Type);
                    if (newObj.Type == 2) //merchant type
                        packet.WriteString8(newObj.Name);
                }
                else //item sprites
                    packet.Write(new byte[3]); //dunno
            }

            return packet;
        }
        internal ServerPacket Attributes(StatUpdateFlags updateType, Attributes stats)
        {
            var packet = new ServerPacket(8);
            packet.WriteByte((byte)updateType);
            if (updateType.HasFlag(StatUpdateFlags.Primary))
            {
                packet.Write(new byte[3]); //dunno
                packet.WriteByte(stats.Level);
                packet.WriteByte(stats.Ability);
                packet.WriteUInt32(stats.MaximumHP);
                packet.WriteUInt32(stats.MaximumMP);
                packet.WriteByte(stats.CurrentStr);
                packet.WriteByte(stats.CurrentInt);
                packet.WriteByte(stats.CurrentWis);
                packet.WriteByte(stats.CurrentCon);
                packet.WriteByte(stats.CurrentDex);
                packet.WriteBoolean(stats.HasUnspentPoints);
                packet.WriteByte(stats.UnspentPoints);
                packet.WriteInt16(stats.MaximumWeight);
                packet.WriteInt16(stats.CurrentWeight);
                packet.Write(new byte[4]); //dunno
            }
            if (updateType.HasFlag(StatUpdateFlags.Vitality))
            {
                packet.WriteUInt32(stats.CurrentHP);
                packet.WriteUInt32(stats.CurrentMP);
            }
            if (updateType.HasFlag(StatUpdateFlags.Experience))
            {
                packet.WriteUInt32(stats.Experience);
                packet.WriteUInt32(stats.ToNextLevel);
                packet.WriteUInt32(stats.AbilityExp);
                packet.WriteUInt32(stats.ToNextAbility);
                packet.WriteUInt32(stats.GamePoints);
                packet.WriteUInt32(stats.Gold);
            }
            if (updateType.HasFlag(StatUpdateFlags.Secondary))
            {
                packet.Write(new byte[1]); //dunno
                packet.WriteByte(stats.Blind);
                packet.Write(new byte[4]); //dunno
                packet.WriteByte((byte)stats.OffenseElement);
                packet.WriteByte((byte)stats.DefenseElement);
                packet.WriteByte(stats.MagicResistance);
                packet.Write(new byte[1]); //dunno
                packet.WriteSByte(stats.ArmorClass);
                packet.WriteByte(stats.Dmg);
                packet.WriteByte(stats.Hit);
            }

            return packet;
        }
        internal ServerPacket SystemMessage(byte type, string message)
        {
            var packet = new ServerPacket(10);
            packet.WriteByte(type);
            packet.WriteString16(message);

            return packet;
        }
        internal ServerPacket ClientWalk(Direction direction, Point nextPoint)
        {
            var packet = new ServerPacket(11);
            packet.WriteByte((byte)direction);
            packet.WritePoint(nextPoint);

            return packet;
        }
        internal ServerPacket CreatureWalk(Objects.Creature creature, Direction direction)
        {
            var packet = new ServerPacket(12);
            packet.WriteUInt32(creature.Id);
            packet.WritePoint(creature.Point);
            packet.WriteByte((byte)creature.Direction);

            return packet;
        }
        internal ServerPacket PublicChat(byte type, string message)
        {
            var packet = new ServerPacket(13);
            packet.WriteByte(type);
            packet.WriteString8(message);

            return packet;
        }
        internal ServerPacket RemoveObject(Objects.VisibleObject obj)
        {
            var packet = new ServerPacket(14);
            packet.WriteUInt32(obj.Id);

            return packet;
        }
        internal ServerPacket AddItem(Objects.Item item)
        {
            var packet = new ServerPacket(15);
            packet.WriteByte(item.Slot);
            packet.WriteUInt16(item.Sprite);
            packet.WriteByte(item.Color);
            packet.WriteString8(item.Name);
            packet.WriteInt32(item.Count);
            packet.WriteBoolean(true); //stackable, need to implement this
            packet.WriteInt32(item.MaxDurability);
            packet.WriteInt32(item.CurrentDurability);
            //if stackable packet.WryteByte(0); dunno why

            return packet;           
        }
        internal ServerPacket RemoveItem(byte slot)
        {
            var packet = new ServerPacket(16);
            packet.WriteByte(slot);

            return packet;
        }
        internal ServerPacket CreatureTurn(uint id, Direction direction)
        {
            var packet = new ServerPacket(17);
            packet.WriteUInt32(id);
            packet.WriteByte((byte)direction);

            return packet;
        }
        internal ServerPacket HealthBar(uint id, byte hpPct)
        {
            var packet = new ServerPacket(19);
            packet.WriteUInt32(id);
            packet.WriteByte(0); //i've seen this as 0 if you get hit by someone else, or 2 if you're hitting something else... but it doesnt change anything
            packet.WriteByte(hpPct);

            return packet;
        }
        internal ServerPacket MapInfo(Objects.Map map)
        {
            var packet = new ServerPacket(21);
            packet.WriteUInt16(map.Id);
            packet.WriteByte(map.SizeX);
            packet.WriteByte(map.SizeY);
            packet.WriteByte(map.Flags);
            packet.Write(new byte[2]); //dunno
            packet.WriteUInt16(map.CRC);
            packet.WriteString8(map.Name);

            return packet;
        }
        internal ServerPacket AddSpell(Objects.Spell spell)
        {
            var packet = new ServerPacket(23);
            packet.WriteByte(spell.Slot);
            packet.WriteUInt16(spell.Sprite);
            packet.WriteByte(spell.Type);
            packet.WriteString8(spell.Name); //this is where youd have "Spell Name (Lev:100/100)" if you wanted it, spell leveling is completely server side and optional
            packet.WriteString8(spell.Prompt);
            packet.WriteByte(spell.CastLines);

            return packet;
        }
        internal ServerPacket RemoveSpell(byte slot)
        {
            var packet = new ServerPacket(24);
            packet.WriteByte(slot);

            return packet;
        }
        internal ServerPacket Sound(byte index)
        {
            var packet = new ServerPacket(25);
            packet.WriteByte(index);

            return packet;
        }
        internal ServerPacket CreatureAnimation(uint id, byte index, ushort speed, bool sound = false)
        {
            var packet = new ServerPacket(26);
            packet.WriteUInt32(id);
            packet.WriteByte(index);
            packet.WriteUInt16(speed);
            //this packet had a trailing 01 if a sound packet followed it, or a trailing FF if no sound followed it.
            //it changed nothing when i left off the end of the packet

            return packet;
        }
        internal ServerPacket MapChangeComplete()
        {
            var packet = new ServerPacket(31);
            packet.Write(new byte[2]); //pretty sure these are nothing

            return packet;
        }
        internal ServerPacket RefreshResponse()
        {
            return new ServerPacket(34); //literally nothing here
        }
        internal ServerPacket Animation(Animation animation)
        {
            var packet = new ServerPacket(41);
            packet.WriteUInt32(animation.TargetId);
            packet.WriteUInt32(animation.SourceId);
            packet.WriteUInt16(animation.TargetAnimation);
            packet.WriteUInt16(animation.SourceAnimation);
            packet.WriteUInt16(animation.AnimationSpeed);

            return packet;
        }

        internal ServerPacket Animation(Animation animation, Point point)
        {
            var packet = new ServerPacket(41);
            packet.WriteUInt32(0U);
            packet.WriteUInt16(animation.TargetAnimation);
            packet.WriteUInt16(animation.AnimationSpeed);
            packet.WritePoint(point);

            return packet;
        }
        internal ServerPacket AddSkill(Objects.Skill skill)
        {
            var packet = new ServerPacket(44);
            packet.WriteByte(skill.Slot);
            packet.WriteUInt16(skill.Sprite);
            packet.WriteString8(skill.Name); //this is where youd have "Skill Name (Lev:100/100)" if you wanted it, skill leveling is completely server side and optional

            return packet;
        }
        internal ServerPacket RemoveSkill(byte slot)
        {
            var packet = new ServerPacket(45);
            packet.WriteByte(slot);

            return packet;
        }
        internal ServerPacket WorldMap(Objects.WorldMap worldMap)
        {
            var packet = new ServerPacket(46);
            packet.WriteString8(worldMap.Field);
            packet.WriteByte((byte)worldMap.Nodes.Count);
            packet.WriteByte(1); //dunno
            foreach(var node in worldMap.Nodes)
            {
                packet.WritePoint(node.ScreenPosition); //position on the map
                packet.WriteString8(node.Name);
                packet.Write(new byte[2]); //dunno
                packet.WriteUInt16(node.MapId); //map you'll spawn on
                packet.WritePoint(node.TargetPoint); //point you'll spawn on
            }

            return packet;
        }
        internal ServerPacket MerchantMenu
        {
            get { return new ServerPacket(47); }
        }
        internal ServerPacket Dialog
        {
            get { return new ServerPacket(48); }
        }
        internal ServerPacket BulletinBoard
        {
            get { return new ServerPacket(49); }
        }
        internal ServerPacket Door
        {
            get { return new ServerPacket(50); }
        }
        internal ServerPacket DisplayUser
        {
            get { return new ServerPacket(51); }
        }
        internal ServerPacket Profile
        {
            get { return new ServerPacket(52); }
        }
        internal ServerPacket WorldList
        {
            get { return new ServerPacket(54); }
        }
        internal ServerPacket AddEquipment
        {
            get { return new ServerPacket(55); }
        }
        internal ServerPacket RemoveEquipment
        {
            get { return new ServerPacket(56); }
        }
        internal ServerPacket ProfileSelf
        {
            get { return new ServerPacket(57); }
        }
        internal ServerPacket SpellBar
        {
            get { return new ServerPacket(58); }
        }
        internal ServerPacket HeartbeatA
        {
            get { return new ServerPacket(59); }
        }
        internal ServerPacket MapData
        {
            get { return new ServerPacket(60); }
        }
        internal ServerPacket Cooldown
        {
            get { return new ServerPacket(63); }
        }
        internal ServerPacket Exchange
        {
            get { return new ServerPacket(66); }
        }
        internal ServerPacket CancelCasting
        {
            get { return new ServerPacket(72); }
        }
        internal ServerPacket MapLoadComplete
        {
            get { return new ServerPacket(88); }
        }
        internal ServerPacket LobbyNotification
        {
            get { return new ServerPacket(96); }
        }
        internal ServerPacket Website
        {
            get { return new ServerPacket(102); }
        }
        internal ServerPacket MapChangePending
        {
            get { return new ServerPacket(103); }
        }
        internal ServerPacket HeartbeatB
        {
            get { return new ServerPacket(104); }
        }
        internal ServerPacket Metafile
        {
            get { return new ServerPacket(111); }
        }
    }
}
