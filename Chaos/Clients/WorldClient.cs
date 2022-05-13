using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using AutoMapper;
using Chaos.Caches.Interfaces;
using Chaos.Clients.Interfaces;
using Chaos.Containers;
using Chaos.Core.Data;
using Chaos.Core.Definitions;
using Chaos.Core.Geometry;
using Chaos.Cryptography.Interfaces;
using Chaos.Effects.Abstractions;
using Chaos.Managers.Interfaces;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Model.Server;
using Chaos.Networking.Serializers;
using Chaos.Objects.Panel;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Packets;
using Chaos.Packets.Definitions;
using Chaos.Packets.Interfaces;
using Chaos.Servers.Interfaces;
using Microsoft.Extensions.Logging;
using ServerPacket = Chaos.Utilities.ServerPacket;

namespace Chaos.Clients;

public class WorldClient : SocketClientBase, IWorldClient
{
    private readonly IMapper Mapper;
    public User User { get; set; } = null!;

    public WorldClient(
        Socket socket,
        IMapper mapper,
        ICryptoClient cryptoClient,
        IWorldServer server,
        IPacketSerializer packetSerializer,
        ILogger<WorldClient> logger
    )
        : base(
            socket,
            cryptoClient,
            server,
            packetSerializer,
            logger) => Mapper = mapper;

    public void SendAddItemToPane(Item item)
    {
        var args = new AddItemToPaneArgs
        {
            Item = Mapper.Map<ItemArg>(item)
        };

        Send(args);
    }

    public void SendAddSkillToPane(Skill skill)
    {
        var args = new AddSkillToPaneArgs
        {
            Skill = Mapper.Map<SkillArg>(skill)
        };

        Send(args);
    }

    public void SendAddSpellToPane(Spell spell)
    {
        var args = new AddSpellToPaneArgs
        {
            Spell = Mapper.Map<SpellArg>(spell)
        };

        Send(args);
    }

    public void SendAnimation(Animation animation)
    {
        var args = new AnimationArgs
        {
            AnimationSpeed = animation.AnimationSpeed,
            SourceAnimation = animation.SourceAnimation,
            SourceId = animation.SourceId,
            TargetAnimation = animation.TargetAnimation,
            TargetId = animation.TargetId,
            TargetPoint = animation.TargetPoint
        };
        
        Send(args);
    }

    public void SendAttributes(StatUpdateType statUpdateType)
    {
        var args = Mapper.Map<AttributesArgs>(User);
        args.StatUpdateType = statUpdateType;
        Send(args);
    }

    public void SendBodyAnimation(
        uint id,
        BodyAnimation bodyAnimation,
        ushort speed,
        byte? sound = null
    )
    {
        var args = new BodyAnimationArgs
        {
            SourceId = id,
            BodyAnimation = bodyAnimation,
            Sound = sound ?? byte.MaxValue,
            Speed = speed
        };

        Send(args);
    }

    public void SendCancelCasting()
    {
        var packet = ServerPacket.FromData(ServerOpCode.CancelCasting);
        Send(ref packet);
    }

    public void SendConfirmClientWalk(Point oldPoint, Direction direction)
    {
        var args = new ConfirmClientWalkArgs
        {
            Direction = direction,
            OldPoint = oldPoint
        };

        Send(args);
    }

    public void SendConfirmExit()
    {
        var args = new ConfirmExitArgs
        {
            ExitConfirmed = true
        };

        Send(args);
    }

    public void SendCooldown(PanelObjectBase panelObjectBase)
    {
        var args = new CooldownArgs
        {
            IsSkill = panelObjectBase is Skill,
            Slot = panelObjectBase.Slot,
            CooldownSecs = Convert.ToUInt32(panelObjectBase.Cooldown.TotalSeconds)
        };

        Send(args);
    }

    public void SendCreatureTurn(uint id, Direction direction)
    {
        var args = new CreatureTurnArgs
        {
            SourceId = id,
            Direction = direction
        };

        Send(args);
    }

    public void SendCreatureWalk(uint id, Point point, Direction direction)
    {
        var args = new CreatureWalkArgs
        {
            SourceId = id,
            OldPoint = point,
            Direction = direction
        };

        Send(args);
    }

    public void SendDisplayUser(User user)
    {
        var args = Mapper.Map<DisplayAislingArgs>(user);

        Send(args);
    }

    public void SendDoors(params Door[] doors)
    {
        var args = new DoorArgs
        {
            Doors = doors.Select(
                    door => new DoorArg
                    {
                        Point = door.Point,
                        Closed = door.Closed,
                        OpenRight = door.OpenRight
                    })
                .ToList()
        };

        Send(args);
    }

    public void SendEffect(EffectBase effect) => throw new NotImplementedException();

    public void SendEquipment(Item item)
    {
        var args = new EquipmentArgs
        {
            Slot = (EquipmentSlot)item.Slot,
            Item = Mapper.Map<ItemArg>(item)
        };

        Send(args);
    }

    public void SendExchangeAccepted(bool persistExchange)
    {
        var args = new ExchangeArgs
        {
            ExchangeResponseType = ExchangeResponseType.Accept,
            PersistExchange = persistExchange
        };

        Send(args);
    }

    public void SendExchangeAddItem(bool rightSide, byte index, Item item)
    {
        var args = new ExchangeArgs
        {
            ExchangeResponseType = ExchangeResponseType.AddItem,
            RightSide = rightSide,
            ExchangeIndex = index,
            ItemSprite = item.Template.ItemSprite.OffsetPanelSprite,
            ItemColor = item.Color,
            ItemName = item.DisplayName
        };

        if (item.Count > 1)
            args.ItemName = $"{item.DisplayName} [{item.Count}]";

        Send(args);
    }

    public void SendExchangeCancel(bool rightSide)
    {
        var args = new ExchangeArgs
        {
            ExchangeResponseType = ExchangeResponseType.Cancel,
            RightSide = rightSide
        };

        Send(args);
    }

    public void SendExchangeRequestAmount(byte slot)
    {
        var args = new ExchangeArgs
        {
            ExchangeResponseType = ExchangeResponseType.RequestAmount,
            FromSlot = slot
        };

        Send(args);
    }

    public void SendExchangeSetGold(bool rightSide, int amount)
    {
        var args = new ExchangeArgs
        {
            ExchangeResponseType = ExchangeResponseType.SetGold,
            RightSide = rightSide,
            GoldAmount = amount
        };

        Send(args);
    }

    public void SendExchangeStart(User fromUser)
    {
        var args = new ExchangeArgs
        {
            ExchangeResponseType = ExchangeResponseType.StartExchange,
            OtherUserId = fromUser.Id,
            OtherUserName = fromUser.Name
        };

        Send(args);
    }

    public void SendForcedClientPacket(ref ClientPacket clientPacket) => throw new NotImplementedException();

    public void SendGroupRequest(GroupRequestType groupRequestType, string fromName)
    {
        var args = new GroupInviteArgs
        {
            GroupRequestType = groupRequestType,
            SourceName = fromName
        };

        Send(args);
    }

    public void SendHealthBar(Creature creature, byte? sound = null)
    {
        var args = new HealthBarArgs
        {
            SourceId = creature.Id,
            HealthPercent = (byte)Math.Clamp(creature.StatSheet.HealthPercent, 0, 100),
            Sound = sound ?? byte.MaxValue
        };

        Send(args);
    }

    public void SendLightLevel(LightLevel lightLevel)
    {
        var args = new LightLevelArgs
        {
            LightLevel = lightLevel
        };

        Send(args);
    }

    public void SendLocation()
    {
        var args = new LocationArgs { Point = User.Point };

        Send(args);
    }

    public void SendMapChangeComplete()
    {
        var packet = ServerPacket.FromData(ServerOpCode.MapChangeComplete, PacketSerializer.Encoding, new byte[2]);

        Send(ref packet);
    }

    public void SendMapChangePending()
    {
        var packet = ServerPacket.FromData(
            ServerOpCode.MapChangePending,
            PacketSerializer.Encoding,
            3,
            0,
            0,
            0,
            0,
            0);

        Send(ref packet);
    }

    public void SendMapData()
    {
        var mapTemplate = User.MapInstance.Template;

        for (byte y = 0; y < mapTemplate.Height; y++)
        {
            var args = new MapDataArgs
            {
                CurrentYIndex = y,
                Width = mapTemplate.Width,
                MapData = mapTemplate.GetRowData(y).ToArray()
            };

            Send(args);
        }
    }

    public void SendMapInfo()
    {
        var instance = User.MapInstance;
        var mapTemplate = instance.Template;

        var args = new MapInfoArgs
        {
            Name = instance.Name,
            MapId = mapTemplate.MapId,
            Width = mapTemplate.Width,
            Height = mapTemplate.Height,
            CheckSum = mapTemplate.CheckSum,
            Flags = instance.Flags
        };

        Send(args);
    }

    public void SendMapLoadComplete()
    {
        var packet = ServerPacket.FromData(ServerOpCode.MapLoadComplete, PacketSerializer.Encoding, 0);

        Send(ref packet);
    }

    public void SendMetafile(MetafileRequestType metafileRequestType, ISimpleCache<string, Metafile> metafileCache, string? name = null)
    {
        var args = new MetafileArgs
        {
            MetafileRequestType = metafileRequestType
        };

        switch (metafileRequestType)
        {
            case MetafileRequestType.DataByName:
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));

                var metafile = metafileCache.GetObject(name);

                args.MetafileData = new MetafileDataArg
                {
                    Name = metafile.Name,
                    CheckSum = metafile.CheckSum,
                    Data = metafile.Data
                };

                break;
            }
            case MetafileRequestType.AllCheckSums:
            {
                args.Info = metafileCache.Select(
                        metafile => new MetafileDataArg
                        {
                            Name = metafile.Name,
                            CheckSum = metafile.CheckSum
                        })
                    .ToList();

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(metafileRequestType), metafileRequestType, "Unknown enum value");
        }

        Send(args);
    }

    public void SendProfile(User user)
    {
        var args = Mapper.Map<ProfileArgs>(user);

        Send(args);
    }

    public void SendProfileRequest()
    {
        var packet = ServerPacket.FromData(ServerOpCode.ProfileRequest);

        Send(ref packet);
    }

    public void SendPublicMessage(uint sourceId, PublicMessageType publicMessageType, string message)
    {
        var args = new PublicMessageArgs
        {
            SourceId = sourceId,
            PublicMessageType = publicMessageType,
            Message = message
        };

        Send(args);
    }

    public void SendRefreshResponse()
    {
        var packet = ServerPacket.FromData(ServerOpCode.RefreshResponse);

        Send(ref packet);
    }

    public void SendRemoveItemFromPane(byte slot)
    {
        var args = new RemoveItemFromPaneArgs
        {
            Slot = slot
        };

        Send(args);
    }

    public void SendRemoveObject(uint id)
    {
        var args = new RemoveObjectArgs
        {
            SourceId = id
        };

        Send(args);
    }

    public void SendRemoveSkillFromPane(byte slot)
    {
        var args = new RemoveSkillFromPaneArgs
        {
            Slot = slot
        };

        Send(args);
    }

    public void SendRemoveSpellFromPane(byte slot)
    {
        var args = new RemoveSpellFromPaneArgs
        {
            Slot = slot
        };

        Send(args);
    }

    public void SendSelfProfile()
    {
        var args = Mapper.Map<SelfProfileArgs>(User);

        Send(args);
    }

    public void SendServerMessage(ServerMessageType serverMessageType, string message)
    {
        var args = new ServerMessageArgs
        {
            ServerMessageType = serverMessageType,
            Message = message
        };

        Send(args);
    }

    public void SendSound(byte sound, bool isMusic)
    {
        var args = new SoundArgs
        {
            Sound = sound,
            IsMusic = isMusic
        };

        Send(args);
    }

    public void SendUnequip(EquipmentSlot equipmentSlot)
    {
        var args = new UnequipArgs
        {
            EquipmentSlot = equipmentSlot
        };

        Send(args);
    }

    public void SendUserId()
    {
        var args = Mapper.Map<UserIdArgs>(User);

        Send(args);
    }

    public void SendVisibleObjects(params VisibleObject[] objects)
    {
        var args = new DisplayVisibleObjectArgs();
        var visibleArgs = new List<VisibleArg>();
        args.VisibleObjects = visibleArgs;

        foreach (var obj in objects)
            if (obj is GroundItem groundItem)
                visibleArgs.Add(
                    new GroundItemArg
                    {
                        Id = groundItem.Id,
                        Color = groundItem.Item.Color,
                        Point = groundItem.Point,
                        Sprite = groundItem.Sprite
                    });
            else if (obj is Creature creature)
                visibleArgs.Add(
                    new CreatureArg
                    {
                        Id = creature.Id,
                        Point = creature.Point,
                        Sprite = creature.Sprite,
                        CreatureType = creature.Type,
                        Direction = creature.Direction,
                        Name = creature.Name
                    });

        Send(args);
    }

    public void SendWorldList(ICollection<User> users)
    {
        var worldList = new List<WorldListArg>();

        var args = new WorldListArgs
        {
            WorldList = worldList
        };

        //these 2 statements are graphed as opposing intersecting lines
        //conal-ly positive (level range grows as level goes up)
        //the idea here is to be able to calculate level ranges for 2 people
        //on the edge of eachother's level range, but theyre both still
        //in eachother's range
        //if a flat range value was calculated based on level
        //a low level person would be in a high level person's larger range,
        //but the high level person would not be in the lower level person's range
        var upperBound = Math.Floor(User.StatSheet.Level + User.StatSheet.Level / 5m + 3);
        var lowerBound = Math.Ceiling(Math.Max(0, User.StatSheet.Level * 5m - 15) / 6);

        foreach (var user in users)
        {
            var arg = Mapper.Map<WorldListArg>(user);

            if ((user.StatSheet.Level <= upperBound) && (user.StatSheet.Level >= lowerBound))
                arg.Color = WorldListColor.WithinLevelRange;

            worldList.Add(arg);
            //TODO: check guild for color
        }

        Send(args);
    }

    public void SendWorldMap(WorldMap worldMap)
    {
        var args = new WorldMapArgs
        {
            FieldName = worldMap.Field,
            ImageIndex = 1,
            Nodes = worldMap.Nodes
        };

        Send(args);
    }
}