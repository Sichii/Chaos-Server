using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

namespace Chaos
{
    internal sealed class ServerPackets
    {
        internal ServerPacket ConnectionInfo(uint tableCheckSum, byte seed, byte[] key)
        {
            var packet = new ServerPacket(0);

            packet.WriteByte(0);
            packet.WriteUInt32(tableCheckSum);
            packet.WriteByte(seed);
            packet.WriteData8(key);

            return packet;
        }
        internal ServerPacket LoginMessage(LoginMessageType type, string message = "")
        {
            var packet = new ServerPacket(2);

            //type is the type of dialog box, message is what's in it
            packet.WriteByte((byte)type);
            packet.WriteString8(type == LoginMessageType.Confirm ? "\0" : message);

            return packet;
        }
        internal ServerPacket Redirect(Redirect redirect)
        {
            var packet = new ServerPacket(3);

            packet.Write(redirect.EndPoint.Address.GetAddressBytes().Reverse().ToArray());
            packet.WriteByte((byte)(redirect.EndPoint.Port / 256));
            packet.WriteByte((byte)(redirect.EndPoint.Port % 256));
            packet.WriteByte((byte)(redirect.Key.Length + Encoding.GetEncoding(949).GetBytes(redirect.Name).Length + 7));
            packet.WriteByte(redirect.Seed);
            packet.WriteData8(redirect.Key);
            packet.WriteString8(redirect.Name);
            packet.WriteInt32(redirect.Id);

            return packet;
        }
        internal ServerPacket Location(Point point)
        {
            var packet = new ServerPacket(4);

            packet.WritePoint16(point);

            return packet;
        }
        internal ServerPacket UserId(int userId, BaseClass userClass)
        {
            var packet = new ServerPacket(5);

            packet.WriteUInt32((uint)userId);
            packet.WriteInt16(0);//dunno
            packet.WriteByte((byte)userClass);
            packet.WriteInt16(0);//dunno

            return packet;
        }
        internal ServerPacket DisplayItemMonster(params VisibleObject[] objects)
        {
            var packet = new ServerPacket(7);

            packet.WriteUInt16((ushort)objects.Length);
            foreach(var obj in objects)
            {
                packet.WritePoint16(obj.Point);
                packet.WriteInt32(obj.Id);
                packet.WriteUInt16(obj.Sprite);
                if(obj.Sprite < 32768) //monster sprites
                {
                    Creature newObj = obj as Creature;
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
                packet.Write(new byte[3]); //dunno
                packet.WriteByte((byte)stats.MailFlags);
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
        internal ServerPacket ServerMessage(ServerMessageType type, string message)
        {
            var packet = new ServerPacket(10);

            packet.WriteByte((byte)type);
            packet.WriteString16(message);

            return packet;
        }
        internal ServerPacket ClientWalk(Direction direction, Point nextPoint)
        {
            var packet = new ServerPacket(11);

            packet.WriteByte((byte)direction);
            packet.WritePoint16(nextPoint);

            return packet;
        }
        internal ServerPacket CreatureWalk(Creature creature, Direction direction)
        {
            var packet = new ServerPacket(12);

            packet.WriteInt32(creature.Id);
            packet.WritePoint16(creature.Point);
            packet.WriteByte((byte)creature.Direction);

            return packet;
        }
        internal ServerPacket PublicChat(ClientMessageType type, int id, string message)
        {
            var packet = new ServerPacket(13);

            packet.WriteByte((byte)type);
            packet.WriteInt32(id);
            packet.WriteString8(message);

            return packet;
        }
        internal ServerPacket RemoveObject(VisibleObject obj)
        {
            var packet = new ServerPacket(14);

            packet.WriteInt32(obj.Id);

            return packet;
        }
        internal ServerPacket AddItem(Item item)
        {
            var packet = new ServerPacket(15);

            packet.WriteByte(item.Slot);
            packet.WriteUInt16(item.Sprite);
            packet.WriteByte(item.Color);
            packet.WriteString8(item.Name);
            packet.WriteInt32(item.Count);
            packet.WriteBoolean(item.Stackable);
            packet.WriteUInt32(item.MaxDurability);
            packet.WriteUInt32(item.CurrentDurability);
            if (item.Stackable)
                packet.WriteByte(0);

            return packet;           
        }
        internal ServerPacket RemoveItem(byte slot)
        {
            var packet = new ServerPacket(16);

            packet.WriteByte(slot);

            return packet;
        }
        internal ServerPacket CreatureTurn(int id, Direction direction)
        {
            var packet = new ServerPacket(17);

            packet.WriteInt32(id);
            packet.WriteByte((byte)direction);

            return packet;
        }
        internal ServerPacket HealthBar(uint id, byte hpPct)
        {
            var packet = new ServerPacket(19);

            packet.WriteUInt32(id);
            packet.WriteByte(0); //i've seen this as 0 if you get hit by someone else, or 2 if you're hitting something else... but it doesnt change anything
            packet.WriteByte(hpPct);
            //packet.WriteByte()  This byte indicates a sound to play when the hp bar hits.(1 for normal assail, etc) You can either (255) or leave off this byte entirely for no sound

            return packet;
        }
        internal ServerPacket MapInfo(Map map)
        {
            var packet = new ServerPacket(21);

            packet.WriteUInt16(map.Id);
            packet.WriteByte(map.SizeX);
            packet.WriteByte(map.SizeY);
            packet.Write(new byte[3]); //dunno
            packet.WriteByte((byte)(map.CheckSum / 256));
            packet.WriteByte((byte)(map.CheckSum % 256));
            packet.WriteString8(map.Name);

            return packet;
        }
        internal ServerPacket AddSpell(Spell spell)
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
        internal ServerPacket CreatureAnimation(int id, byte animation, ushort speed, bool sound = false)
        {
            var packet = new ServerPacket(26);

            packet.WriteInt32(id);
            packet.WriteByte(animation);
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
        internal ServerPacket LightLevel(LightLevel lightLevel)
        {
            var packet = new ServerPacket(32);

            packet.WriteByte((byte)lightLevel);

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
            packet.WritePoint16(point);

            return packet;
        }
        internal ServerPacket AddSkill(Skill skill)
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
        internal ServerPacket WorldMap(WorldMap worldMap)
        {
            var packet = new ServerPacket(46);

            packet.WriteString8(worldMap.Field);
            packet.WriteByte((byte)worldMap.Nodes.Count);
            packet.WriteByte(1); //image
            foreach(var node in worldMap.Nodes)
            {
                packet.WritePoint16(node.Position); //position on the worldmap
                packet.WriteString8(node.Name);
                packet.WriteUInt16(node.CheckSum);
                packet.WriteUInt16(node.MapId); //map you'll spawn on
                packet.WritePoint16(node.Point); //point you'll spawn on
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
        internal ServerPacket Door(params Door[] doors)
        {
            var packet = new ServerPacket(50);

            packet.WriteByte((byte)doors.Length);
            foreach(var door in doors)
            {
                packet.WritePoint8(door.Point);
                packet.WriteBoolean(door.Opened);
                packet.WriteBoolean(door.OpenRight);
            }

            return packet;
        }
        internal ServerPacket DisplayUser(User user)
        {
            DisplayData display = user.DisplayData;
            var packet = new ServerPacket(51);

            packet.WritePoint16(user.Point);
            packet.WriteByte((byte)user.Direction);
            packet.WriteInt32(user.Id);
            if(user.Sprite == 0)
            {
                packet.WriteUInt16(display.HeadSprite);
                packet.WriteByte(display.BodySprite);
                packet.WriteUInt16(display.ArmorSprite1);
                packet.WriteByte(display.BootsSprite);
                packet.WriteUInt16(display.ArmorSprite2);
                packet.WriteByte(display.ShieldSprite);
                packet.WriteUInt16(display.WeaponSprite);
                packet.WriteByte(display.HeadColor);
                packet.WriteByte(display.BootsColor);
                packet.WriteByte(display.AccessoryColor1);
                packet.WriteUInt16(display.AccessorySprite1);
                packet.WriteByte(display.AccessoryColor2);
                packet.WriteUInt16(display.AccessorySprite2);
                packet.WriteByte(display.AccessoryColor3);
                packet.WriteUInt16(display.AccessorySprite3);
                packet.WriteByte((byte)display.LanternSize);
                packet.WriteByte((byte)display.RestPosition);
                packet.WriteUInt16(display.OvercoatSprite);
                packet.WriteByte(display.OvercoatColor);
                packet.WriteByte((byte)display.BodyColor);
                packet.WriteBoolean(display.IsHidden);
                packet.WriteByte(display.FaceSprite);
            }
            else
            {
                packet.WriteUInt16(ushort.MaxValue);
                packet.WriteUInt16((ushort)(user.Sprite + 16384U));
                packet.WriteByte(display.HeadColor);
                packet.WriteByte(display.BootsColor);
                packet.Write(new byte[6]); //dunno
            }
            packet.WriteByte((byte)display.NameTagStyle);
            packet.WriteString8(display.BodySprite == 0 ? string.Empty : (user.Name ?? string.Empty));
            packet.WriteString8(display.GroupName ?? string.Empty);

            return packet;
        }
        internal ServerPacket Profile(User user)
        {
            var packet = new ServerPacket(52);

            packet.WriteInt32(user.Id);
            for(byte slot = 1; slot < user.Equipment.Length; slot++)
            {
                packet.WriteUInt16(user.Equipment[slot]?.Sprite ?? 0);
                packet.WriteByte(user.Equipment[slot]?.Color ?? 0);
            }
            packet.WriteBoolean(user.UserOptions.Group);
            packet.WriteString8(user.Name);
            packet.WriteByte((byte)user.Nation);
            packet.WriteString8(user.Titles.FirstOrDefault() ?? "");
            packet.WriteBoolean(user.Grouped);
            packet.WriteString8(user.Guild?.TitleOf(user.Name) ?? "");
            packet.WriteString8(user.AdvClass == AdvClass.None ? user.BaseClass.ToString() : user.AdvClass.ToString());
            packet.WriteString8(user.Guild?.Name ?? "");
            packet.WriteByte(user.Legend.Length);
            foreach(var mark in user.Legend)
            {
                packet.WriteByte((byte)mark.Icon);
                packet.WriteByte((byte)mark.Color);
                packet.WriteString8(mark.Key);
                packet.WriteString8(mark.ToString());
            }
            packet.WriteUInt16((ushort)(user.Personal.Portrait.Length + user.Personal.Message.Length + 4));
            packet.WriteData16(user.Personal.Portrait);
            packet.WriteString16(user.Personal.Message);

            return packet;
        }
        internal ServerPacket WorldList(IEnumerable<User> users, byte UserLevel)
        {
            var packet = new ServerPacket(54);

            packet.WriteUInt16((ushort)users.Count());
            packet.WriteUInt16((ushort)users.Count());
            foreach(var user in users)
            {
                byte range = (byte)((UserLevel / 5) + 3);
                packet.WriteByte((byte)user.BaseClass);
                packet.WriteByte((byte)(Math.Abs(user.Attributes.Level - UserLevel) <= range ? 151 : 255));
                packet.WriteByte((byte)user.SocialStatus);
                packet.WriteString8(user.Titles?.FirstOrDefault() ?? "");
                packet.WriteBoolean(user.IsMaster);
                packet.WriteString8(user.Name);
            }

            return packet;
        }
        internal ServerPacket AddEquipment(Item item)
        {
            var packet = new ServerPacket(55);

            packet.WriteByte((byte)item.EquipmentSlot);
            packet.WriteUInt16((ushort)(item.Sprite + 32768));
            packet.WriteByte(item.Color);
            packet.WriteString8(item.Name);
            packet.WriteByte(0);
            packet.WriteUInt32(item.MaxDurability);
            packet.WriteUInt32(item.CurrentDurability);

            return packet;
        }
        internal ServerPacket RemoveEquipment(EquipmentSlot slot)
        {
            var packet = new ServerPacket(56);

            packet.WriteByte((byte)slot);

            return packet;
        }
        internal ServerPacket ProfileSelf(User user)
        {
            var packet = new ServerPacket(57);

            packet.WriteByte((byte)user.Nation);
            packet.WriteString8(user.Guild?.Name ?? "");
            packet.WriteString8(user.Guild?[user.Name] ?? ""); //idk?
            packet.WriteString8(user.Group?.ToString() ?? (user.Spouse != null ? $@"Spouse: {user.Spouse}" : "Adventuring alone"));
            packet.WriteBoolean(user.UserOptions.Group);
            packet.WriteBoolean(user.Group?.Box != null);
            if(user.Group?.Box != null)
            {
                packet.WriteString8(user.Group.Box.GroupLeader.Name);
                packet.WriteString8(user.Group.Box.Name);
                packet.Write(new byte[13]); //other groupbox stuff will add later
            }
            packet.WriteByte((byte)user.BaseClass);
            packet.WriteBoolean(user.AdvClass != AdvClass.None); //is med class
            packet.WriteBoolean(user.IsMaster);
            packet.WriteString8(user.AdvClass != AdvClass.None ? user.AdvClass.ToString() : user.BaseClass.ToString()); //class string
            packet.WriteString8(user.Guild?.Name ?? "");
            packet.WriteByte(user.Legend.Length);
            foreach(var mark in user.Legend)
            {
                packet.WriteByte((byte)mark.Icon);
                packet.WriteByte((byte)mark.Color);
                packet.WriteString8(mark.Key);
                packet.WriteString8(mark.ToString());
            }

            return packet;
        }
        internal ServerPacket EffectsBar(ushort effect, EffectsBarColor color)
        {
            var packet = new ServerPacket(58);

            packet.WriteUInt16(effect);
            packet.WriteByte((byte)color);

            return packet;
        }
        internal ServerPacket HeartbeatA(byte a, byte b)
        {
            var packet = new ServerPacket(59);

            packet.WriteByte(a);
            packet.WriteByte(b);

            return packet;
        }
        internal ServerPacket[] MapData(Map map)
        {
            List<ServerPacket> staggeredData = new List<ServerPacket>();
            int key = 0;
            
            for(ushort y = 0; y < map.SizeY; ++y)
            {
                var packet = new ServerPacket(60);
                packet.WriteUInt16(y);
                for (int x = 0; x < map.SizeX * 6; x += 2)
                {
                    packet.WriteByte(map.Data[key + 1]);
                    packet.WriteByte(map.Data[key]);
                    key += 2;
                }

                staggeredData.Add(packet);
            }

            return staggeredData.ToArray();
        }
        internal ServerPacket Cooldown(bool isSkill, byte slot, uint ticks)
        {
            var packet = new ServerPacket(63);

            packet.WriteBoolean(isSkill);
            packet.WriteByte(slot);
            packet.WriteUInt32(ticks);

            return packet;
        }
        internal ServerPacket Exchange()
        {
            //i'll do this later, its cancer
            return new ServerPacket(66);
        }
        internal ServerPacket CancelCasting()
        {
            //i don't believe there's anything here
            return new ServerPacket(72);
        }
        internal ServerPacket RequestPersonal()
        {
            //i don't believe there's anything here
            return new ServerPacket(73);
        }
        internal ServerPacket ConfirmExit()
        {
            var packet = new ServerPacket(76);

            packet.WriteBoolean(true);
            packet.Write(new byte[2]);

            return packet;
        }
        internal ServerPacket ServerTable(byte[] serverTbl = null)
        {
            var packet = new ServerPacket(86);

            packet.WriteData16(serverTbl);

            return packet;
        }
        internal ServerPacket MapLoadComplete()
        {
            //i don't believe there's anything here
            return new ServerPacket(88);
        }
        internal ServerPacket LobbyNotification(bool sendNotif, uint loginMsgCheckSum = 0, byte[] notif = null)
        {
            var packet = new ServerPacket(96);

            packet.WriteBoolean(sendNotif);
            if (sendNotif)
                packet.WriteData16(notif);
            else
                packet.WriteUInt32(loginMsgCheckSum);

            return packet;
        }
        internal ServerPacket GroupRequest(GroupRequestType type, string sender)
        {
            var packet = new ServerPacket(99);

            packet.WriteByte((byte)type);
            packet.WriteString8(sender);

            return packet;
        }
        internal ServerPacket LobbyControls(byte type, string message)
        {
            var packet = new ServerPacket(102);
            //1 = Exit and load directory
            //2 = load directory
            //3 = website
            //there's more to the 1 and 2 bytes, will figure out later
            packet.WriteByte(type);
            packet.WriteString8(message);

            return packet;
        }
        internal ServerPacket MapChangePending()
        {
            //i don't believe there's anything here
            return new ServerPacket(103);
        }
        internal ServerPacket HeartbeatB()
        {
            var packet = new ServerPacket(104);
            //helps the client keep synchronized
            packet.WriteInt32(Environment.TickCount);

            return packet;
        }
        internal ServerPacket[] Metafile(bool sendPath, params MetaFile[] metafiles)
        {
            List<ServerPacket> packets = new List<ServerPacket>();

            //if sendpath, you're just sending filenames, client will respond if it needs the file
            if (sendPath)
            {
                var packet = new ServerPacket(111);
                packet.WriteBoolean(true);
                packet.WriteUInt16((ushort)metafiles.Length);
                foreach (var metafile in metafiles)
                {
                    packet.WriteString8(metafile.Name);
                    packet.WriteUInt32(Crypto.Generate32(metafile.Data));
                }
                packets.Add(packet);
            }
            else
            {
                //here we just send them all, but you can send a single one
                foreach (var metafile in metafiles)
                {
                    var packet = new ServerPacket(111);
                    packet.WriteBoolean(false);
                    packet.WriteString8(metafile.Name);
                    packet.WriteUInt32(Crypto.Generate32(metafile.Data));
                    packet.WriteData16(metafile.Data);

                    packets.Add(packet);
                }
            }

            return packets.ToArray();
        }

        internal ServerPacket AcceptConnection()
        {
            var packet = new ServerPacket(126);

            packet.WriteByte(27);
            packet.WriteString("CONNECTED SERVER", true);

            return packet;
        }
    }
}
