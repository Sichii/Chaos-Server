using System.Net.Sockets;
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Cryptography.Abstractions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Networking;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.Panel.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;
using Chaos.Services.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Networking;

public sealed class WorldClient : SocketClientBase, IWorldClient
{
    private readonly ITypeMapper Mapper;
    private readonly IWorldServer<IWorldClient> Server;
    public Aisling Aisling { get; set; } = null!;

    public WorldClient(
        Socket socket,
        IOptions<ChaosOptions> chaosOptions,
        ITypeMapper mapper,
        ICrypto crypto,
        IWorldServer<IWorldClient> server,
        IPacketSerializer packetSerializer,
        ILogger<WorldClient> logger
    )
        : base(
            socket,
            crypto,
            packetSerializer,
            logger)
    {
        LogRawPackets = chaosOptions.Value.LogRawPackets;
        Mapper = mapper;
        Server = server;
    }

    /// <inheritdoc />
    protected override ValueTask HandlePacketAsync(Span<byte> span)
    {
        var opCode = span[3];
        var isEncrypted = Crypto.ShouldBeEncrypted(opCode);
        var packet = new ClientPacket(ref span, isEncrypted);

        if (isEncrypted)
            Crypto.Decrypt(ref packet);

        if (LogRawPackets)
            Logger.WithProperty(this)
                  .LogTrace("[Rcv] {@Packet}", packet.ToString());

        return Server.HandlePacketAsync(this, in packet);
    }

    public void SendAddItemToPane(Item item)
    {
        var args = new AddItemToPaneArgs
        {
            Item = Mapper.Map<ItemInfo>(item)
        };

        Send(args);
    }

    public void SendAddSkillToPane(Skill skill)
    {
        var args = new AddSkillToPaneArgs
        {
            Skill = Mapper.Map<SkillInfo>(skill)
        };

        Send(args);
    }

    public void SendAddSpellToPane(Spell spell)
    {
        var args = new AddSpellToPaneArgs
        {
            Spell = Mapper.Map<SpellInfo>(spell)
        };

        Send(args);
    }

    public void SendAnimation(Animation animation)
    {
        var args = Mapper.Map<AnimationArgs>(animation);

        Send(args);
    }

    public void SendAttributes(StatUpdateType statUpdateType)
    {
        var args = Mapper.Map<AttributesArgs>(Aisling);
        args.StatUpdateType = statUpdateType;
        Send(args);
    }

    /// <inheritdoc />
    public void SendBoard()
    {
        var packet = ServerPacketEx.FromData(
            ServerOpCode.BulletinBoard,
            PacketSerializer.Encoding,
            1,
            0);

        Send(ref packet);
    }

    public void SendBodyAnimation(
        uint id,
        BodyAnimation bodyAnimation,
        ushort speed,
        byte? sound = null
    )
    {
        if (bodyAnimation is BodyAnimation.None)
            return;

        var args = new BodyAnimationArgs
        {
            SourceId = id,
            BodyAnimation = bodyAnimation,
            Sound = sound,
            AnimationSpeed = speed
        };

        Send(args);
    }

    public void SendCancelCasting()
    {
        var packet = ServerPacketEx.FromData(ServerOpCode.CancelCasting, PacketSerializer.Encoding);
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

    public void SendCooldown(PanelEntityBase panelEntityBase)
    {
        if (!panelEntityBase.Cooldown.HasValue)
            return;

        if (!panelEntityBase.Elapsed.HasValue)
            return;

        var remaining = panelEntityBase.Cooldown.Value.TotalSeconds - panelEntityBase.Elapsed.Value.TotalSeconds;

        var args = new CooldownArgs
        {
            IsSkill = panelEntityBase is Skill,
            Slot = panelEntityBase.Slot,
            CooldownSecs = Convert.ToUInt32(remaining)
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

    public void SendCreatureWalk(uint id, Point startPoint, Direction direction)
    {
        var args = new CreatureWalkArgs
        {
            SourceId = id,
            OldPoint = startPoint,
            Direction = direction
        };

        Send(args);
    }

    /// <inheritdoc />
    public void SendDialog(Dialog dialog)
    {
        var dialogType = dialog.Type.ToDialogType();
        var menuType = dialog.Type.ToMenuType();

        if (dialogType != null)
        {
            var args = Mapper.Map<DialogArgs>(dialog);

            Send(args);
        } else if (menuType != null)
        {
            var args = Mapper.Map<MenuArgs>(dialog);

            Send(args);
        }
    }

    public void SendDisplayAisling(Aisling aisling)
    {
        var args = Mapper.Map<DisplayAislingArgs>(aisling);

        //we can always see ourselves, and we're never hostile to ourself
        if (!Aisling.Equals(aisling))
        {
            if (!Aisling.IsFriendlyTo(aisling))
                args.NameTagStyle = NameTagStyle.Hostile;

            //if we're not an admin, and the aisling is not visible
            if (!Aisling.IsAdmin && aisling.Visibility is not VisibilityType.Normal)
            {
                //remove the name
                args.Name = string.Empty;

                //if we cant see the aisling, hide it (it is otherwise transparent)
                if (!Aisling.Script.CanSee(aisling))
                    args.IsHidden = true;
            }
        }

        Send(args);
    }

    public void SendDoors(IEnumerable<Door> doors)
    {
        var args = new DoorArgs
        {
            Doors = Mapper.MapMany<DoorInfo>(doors).ToList()
        };

        if (args.Doors.Any())
            Send(args);
    }

    public void SendEffect(EffectColor effectColor, byte effectIcon)
    {
        var args = new EffectArgs
        {
            EffectColor = effectColor,
            EffectIcon = effectIcon
        };

        Send(args);
    }

    public void SendEquipment(Item item)
    {
        var args = new EquipmentArgs
        {
            Slot = (EquipmentSlot)item.Slot,
            Item = Mapper.Map<ItemInfo>(item)
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
            ItemSprite = item.ItemSprite.PanelSprite,
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

    public void SendExchangeStart(Aisling fromAisling)
    {
        var args = new ExchangeArgs
        {
            ExchangeResponseType = ExchangeResponseType.StartExchange,
            OtherUserId = fromAisling.Id,
            OtherUserName = fromAisling.Name
        };

        Send(args);
    }

    public void SendForcedClientPacket(ref ClientPacket clientPacket) => throw new NotImplementedException();

    public void SendGroupRequest(GroupRequestType groupRequestType, string fromName)
    {
        var args = new GroupRequestArgs
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
            HealthPercent = (byte)creature.StatSheet.HealthPercent,
            Sound = sound
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
        var args = new LocationArgs
        {
            X = Aisling.X,
            Y = Aisling.Y
        };

        Send(args);
    }

    public void SendMapChangeComplete()
    {
        var packet = ServerPacketEx.FromData(ServerOpCode.MapChangeComplete, PacketSerializer.Encoding, new byte[2]);

        Send(ref packet);
    }

    public void SendMapChangePending()
    {
        var packet = ServerPacketEx.FromData(
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
        var mapTemplate = Aisling.MapInstance.Template;

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
        var args = Mapper.Map<MapInfoArgs>(Aisling.MapInstance);

        Send(args);
    }

    public void SendMapLoadComplete()
    {
        var packet = ServerPacketEx.FromData(ServerOpCode.MapLoadComplete, PacketSerializer.Encoding, 0);

        Send(ref packet);
    }

    public void SendMetaData(MetaDataRequestType metaDataRequestType, IMetaDataStore metaDataStore, string? name = null)
    {
        var args = new MetaDataArgs
        {
            MetaDataRequestType = metaDataRequestType
        };

        switch (metaDataRequestType)
        {
            case MetaDataRequestType.DataByName:
            {
                ArgumentNullException.ThrowIfNull(name);

                var metadata = metaDataStore.Get(name);

                args.MetaDataInfo = Mapper.Map<MetaDataInfo>(metadata);

                break;
            }
            case MetaDataRequestType.AllCheckSums:
            {
                args.MetaDataCollection = Mapper.MapMany<MetaDataInfo>(metaDataStore)
                                                .ToList();

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(metaDataRequestType), metaDataRequestType, "Unknown enum value");
        }

        Send(args);
    }

    public void SendNotepad(
        byte identifier,
        NotepadType type,
        byte height,
        byte width,
        string? message
    )
    {
        var args = new NotepadArgs
        {
            Slot = identifier,
            NotepadType = type,
            Height = height,
            Width = width,
            Message = message ?? string.Empty
        };

        Send(args);
    }

    public void SendProfile(Aisling aisling)
    {
        var args = Mapper.Map<ProfileArgs>(aisling);

        Send(args);
    }

    public void SendProfileRequest()
    {
        var packet = ServerPacketEx.FromData(ServerOpCode.ProfileRequest, PacketSerializer.Encoding);

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
        var packet = ServerPacketEx.FromData(ServerOpCode.RefreshResponse, PacketSerializer.Encoding);

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
        var args = Mapper.Map<SelfProfileArgs>(Aisling);

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
        var args = Mapper.Map<UserIdArgs>(Aisling);

        Send(args);
    }

    public void SendVisibleEntities(IEnumerable<VisibleEntity> objects)
    {
        //split this into chunks so as not to crash the client
        foreach (var chunk in objects.OrderBy(o => o.Creation).Chunk(5000))
        {
            var args = new DisplayVisibleEntitiesArgs();
            var visibleArgs = new List<VisibleEntityInfo>();
            args.VisibleObjects = visibleArgs;

            foreach (var obj in chunk)
                switch (obj)
                {
                    case GroundItem groundItem:
                        var groundItemInfo = Mapper.Map<GroundItemInfo>(groundItem);

                        //non visible item that can be seen
                        if (groundItem.Visibility is not VisibilityType.Normal && (Aisling.IsAdmin || Aisling.Script.CanSee(groundItem)))
                        {
                            groundItemInfo.Sprite = 11978;
                            groundItemInfo.Color = DisplayColor.Black;
                        }

                        visibleArgs.Add(groundItemInfo);

                        break;
                    case Money money:
                        var moneyInfo = Mapper.Map<GroundItemInfo>(money);

                        //non visible money that can be seen
                        if (money.Visibility is not VisibilityType.Normal && (Aisling.IsAdmin || Aisling.Script.CanSee(money)))
                            moneyInfo.Sprite = 138;

                        visibleArgs.Add(moneyInfo);

                        break;
                    case Creature creature:
                        var creatureInfo = Mapper.Map<CreatureInfo>(creature);

                        //none visible creature that can be seen
                        if (creature.Visibility is not VisibilityType.Normal && (Aisling.IsAdmin || Aisling.Script.CanSee(creature)))
                            creatureInfo.Sprite = 405;

                        visibleArgs.Add(creatureInfo);

                        break;
                }

            Send(args);
        }
    }

    public void SendWorldList(IEnumerable<Aisling> aislings)
    {
        var worldList = new List<WorldListMemberInfo>();
        var orderedAislings = aislings.OrderByDescending(aisling => aisling.StatSheet.MaximumMp * 2 + aisling.StatSheet.MaximumHp);

        var args = new WorldListArgs
        {
            WorldList = worldList
        };

        foreach (var aisling in orderedAislings)
        {
            var arg = Mapper.Map<WorldListMemberInfo>(aisling);

            if (Aisling.WithinLevelRange(aisling))
                arg.Color = WorldListColor.WithinLevelRange;

            worldList.Add(arg);
            //TODO: check guild for color
        }

        Send(args);
    }

    public void SendWorldMap(WorldMap worldMap)
    {
        var args = Mapper.Map<WorldMapArgs>(worldMap);

        Send(args);
    }
}