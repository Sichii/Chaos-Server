#region
using System.Net.Sockets;
using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Cryptography.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Networking;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Board;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.Panel.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Services.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Networking;

public sealed class ChaosWorldClient : WorldClientBase, IChaosWorldClient
{
    private readonly ITypeMapper Mapper;
    private readonly IWorldServer<IChaosWorldClient> Server;
    private Animation? CurrentAnimation;
    private Task HeartbeatTask = null!;
    public Aisling Aisling { get; set; } = null!;
    public byte? Heartbeat1 { get; set; }
    public byte? Heartbeat2 { get; set; }

    /// <inheritdoc />
    public uint LoginId1 { get; set; }

    /// <inheritdoc />
    public ushort LoginId2 { get; set; }

    public ChaosWorldClient(
        Socket socket,
        IOptions<ChaosOptions> chaosOptions,
        ITypeMapper mapper,
        ICrypto crypto,
        IWorldServer<IChaosWorldClient> server,
        IPacketSerializer packetSerializer,
        ILogger<ChaosWorldClient> logger)
        : base(
            socket,
            crypto,
            packetSerializer,
            logger)
    {
        LogRawPackets = chaosOptions.Value.LogRawPackets;
        LogSendPacketCode = chaosOptions.Value.LogSendPacketCode;
        LogReceivePacketCode = chaosOptions.Value.LogReceivePacketCode;
        Mapper = mapper;
        Server = server;
    }

    /// <inheritdoc />
    public override void BeginReceive()
    {
        base.BeginReceive();
        HeartbeatTask = DoHeartbeatAsync();
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
        //check if the current animation is null
        var currentAnimation = Interlocked.CompareExchange(ref CurrentAnimation, animation, null);

        if (currentAnimation is null || animation.TargetPoint.HasValue || Aisling is { Options.PriorityAnimations: false })
        {
            InnerSendAnimation();

            return;
        }

        //this is for thread safety
        //if the current animation is not null
        while (true)
        {
            //if the current animation is higher priority, don't replace it
            if (!animation.ShouldAnimateOver(currentAnimation))
                break;

            //try to replace the current animation with the new animation
            var newCurrentAnimation = Interlocked.CompareExchange(ref CurrentAnimation, animation, currentAnimation);

            //the animation replaced was the animation we expected
            if (newCurrentAnimation == currentAnimation)
            {
                InnerSendAnimation();

                break;
            }

            currentAnimation = newCurrentAnimation;
        }

        return;

        void InnerSendAnimation()
        {
            animation.Started = DateTime.UtcNow;
            var args = Mapper.Map<AnimationArgs>(animation);

            Send(args);
        }
    }

    public void SendAttributes(StatUpdateType statUpdateType)
    {
        var args = Mapper.Map<AttributesArgs>(Aisling);

        args.StatUpdateType |= statUpdateType;
        Send(args);
    }

    public void SendBoardList(IEnumerable<BoardBase> boards)
    {
        var args = new DisplayBoardArgs
        {
            Type = BoardOrResponseType.BoardList,
            Boards = Mapper.MapMany<BoardBase, BoardInfo>(boards)
                           .ToList()
        };

        Send(args);
    }

    public void SendBoardResponse(BoardOrResponseType responseType, string message, bool success)
    {
        var args = new DisplayBoardArgs
        {
            Type = responseType,
            ResponseMessage = message,
            Success = success
        };

        Send(args);
    }

    public void SendBodyAnimation(
        uint id,
        BodyAnimation bodyAnimation,
        ushort speed,
        byte? sound = null)
    {
        if (bodyAnimation is BodyAnimation.None)
            return;

        if (Aisling is { Options.ShowBodyAnimations: false })
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

    public override void SendCancelCasting()
    {
        var args = new CancelCastingArgs();

        Send(args);
    }

    public void SendClientWalkResponse(Point oldPoint, Direction direction)
    {
        var args = new ClientWalkResponseArgs
        {
            Direction = direction,
            OldPoint = oldPoint
        };

        Send(args);
    }

    public void SendCooldown(PanelEntityBase panelEntityBase)
    {
        if (panelEntityBase is Item)
            return;

        if (!panelEntityBase.Cooldown.HasValue)
            return;

        if (!panelEntityBase.Elapsed.HasValue)
            return;

        var remaining = panelEntityBase.Cooldown.Value.TotalSeconds - panelEntityBase.Elapsed.Value.TotalSeconds;

        if (remaining < 0)
            return;

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

    public void SendDisplayAisling(Aisling aisling)
    {
        var args = Mapper.Map<DisplayAislingArgs>(aisling);

        //we can always see ourselves, and we're never hostile to ourself
        if (!Aisling.Equals(aisling))
        {
            if (Aisling.IsHostileTo(aisling))
                args.NameTagStyle = NameTagStyle.Hostile;
            else if (Aisling.IsFriendlyTo(aisling))
                args.NameTagStyle = NameTagStyle.FriendlyHover;
            else
                args.NameTagStyle = NameTagStyle.NeutralHover;

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

    /// <inheritdoc />
    public void SendDisplayBoard(BoardBase boardBase, short? startPostId = null)
    {
        var args = new DisplayBoardArgs
        {
            Type = boardBase is MailBox ? BoardOrResponseType.MailBoard : BoardOrResponseType.PublicBoard,
            Board = Mapper.Map<BoardInfo>(boardBase),
            StartPostId = startPostId
        };

        Send(args);
    }

    /// <inheritdoc />
    public void SendDisplayDialog(Dialog dialog)
    {
        var dialogType = dialog.Type.ToDialogType();
        var menuType = dialog.Type.ToMenuType();

        if (dialogType != null)
        {
            var args = Mapper.Map<DisplayDialogArgs>(dialog);

            Send(args);
        } else if (menuType != null)
        {
            var args = Mapper.Map<DisplayMenuArgs>(dialog);

            Send(args);
        }
    }

    public void SendDisplayGroupInvite(ServerGroupSwitch serverGroupSwitch, string fromName, DisplayGroupBoxInfo? groupBoxInfo = null)
    {
        var args = new DisplayGroupInviteArgs
        {
            ServerGroupSwitch = serverGroupSwitch,
            SourceName = fromName
        };

        if (serverGroupSwitch == ServerGroupSwitch.ShowGroupBox)
            args.GroupBoxInfo = groupBoxInfo;

        Send(args);
    }

    /// <inheritdoc />
    public void SendDisplayNotepad(
        NotepadType type,
        Item item,
        byte width,
        byte height)
    {
        var args = new DisplayNotepadArgs
        {
            Slot = item.Slot,
            NotepadType = type,
            Width = width,
            Height = height,
            Message = item.NotepadText ?? string.Empty
        };

        Send(args);
    }

    public void SendDisplayPublicMessage(uint sourceId, PublicMessageType publicMessageType, string message)
    {
        var args = new DisplayPublicMessageArgs
        {
            SourceId = sourceId,
            PublicMessageType = publicMessageType,
            Message = message
        };

        Send(args);
    }

    public void SendDisplayUnequip(EquipmentSlot equipmentSlot)
    {
        var args = new DisplayUnequipArgs
        {
            EquipmentSlot = equipmentSlot
        };

        Send(args);
    }

    public void SendDoors(IEnumerable<Door> doors)
    {
        var args = new DoorArgs
        {
            Doors = Mapper.MapMany<DoorInfo>(doors)
                          .ToList()
        };

        if (args.Doors.Count != 0)
            Send(args);
    }

    public override void SendEditableProfileRequest()
    {
        var args = new EditableProfileRequestArgs();

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
        var args = new DisplayExchangeArgs
        {
            ExchangeResponseType = ExchangeResponseType.Accept,
            PersistExchange = persistExchange,
            Message = "Exchange Accepted."
        };

        Send(args);
    }

    public void SendExchangeAddItem(bool rightSide, byte index, Item item)
    {
        var args = new DisplayExchangeArgs
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
        var args = new DisplayExchangeArgs
        {
            ExchangeResponseType = ExchangeResponseType.Cancel,
            RightSide = rightSide,
            Message = "Exchange Canceled."
        };

        Send(args);
    }

    public void SendExchangeRequestAmount(byte slot)
    {
        var args = new DisplayExchangeArgs
        {
            ExchangeResponseType = ExchangeResponseType.RequestAmount,
            FromSlot = slot
        };

        Send(args);
    }

    public void SendExchangeSetGold(bool rightSide, int amount)
    {
        var args = new DisplayExchangeArgs
        {
            ExchangeResponseType = ExchangeResponseType.SetGold,
            RightSide = rightSide,
            GoldAmount = amount
        };

        Send(args);
    }

    public void SendExchangeStart(Aisling fromAisling)
    {
        var args = new DisplayExchangeArgs
        {
            ExchangeResponseType = ExchangeResponseType.StartExchange,
            OtherUserId = fromAisling.Id,
            OtherUserName = fromAisling.Name
        };

        Send(args);
    }

    public void SendExitResponse()
    {
        var args = new ExitResponseArgs
        {
            ExitConfirmed = true
        };

        Send(args);
    }

    public void SendForceClientPacket(ref Packet packet)
    {
        var args = new ForceClientPacketArgs
        {
            ClientOpCode = (ClientOpCode)packet.OpCode,
            Data = packet.Buffer.ToArray()
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

    public override void SendMapChangeComplete()
    {
        var args = new MapChangeCompleteArgs();

        Send(args);
    }

    public override void SendMapChangePending()
    {
        var args = new MapChangePendingArgs();

        Send(args);
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
                MapData = mapTemplate.GetRowData(y)
                                     .ToArray()
            };

            Send(args);
        }
    }

    public void SendMapInfo()
    {
        var args = Mapper.Map<MapInfoArgs>(Aisling.MapInstance);

        Send(args);
    }

    public override void SendMapLoadComplete()
    {
        var args = new MapLoadCompleteArgs();

        Send(args);
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
        string? message)
    {
        var args = new DisplayNotepadArgs
        {
            Slot = identifier,
            NotepadType = type,
            Height = height,
            Width = width,
            Message = message ?? string.Empty
        };

        Send(args);
    }

    public void SendOtherProfile(Aisling aisling)
    {
        var args = Mapper.Map<OtherProfileArgs>(aisling);

        Send(args);
    }

    public void SendPost(Post post, bool isMail, bool enablePrevBtn = true)
    {
        var args = new DisplayBoardArgs
        {
            Type = isMail ? BoardOrResponseType.MailPost : BoardOrResponseType.PublicPost,
            Post = Mapper.Map<PostInfo>(post),
            EnablePrevBtn = enablePrevBtn
        };

        Send(args);
    }

    public override void SendRefreshResponse()
    {
        var args = new RefreshResponseArgs();

        Send(args);
    }

    public void SendRemoveEntity(uint id)
    {
        var args = new RemoveEntityArgs
        {
            SourceId = id
        };

        Send(args);
    }

    public void SendRemoveItemFromPane(byte slot)
    {
        var args = new RemoveItemFromPaneArgs
        {
            Slot = slot
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
        if ((message.Length < CONSTANTS.MAX_MESSAGE_LINE_LENGTH)
            || serverMessageType is ServerMessageType.WoodenBoard
                                    or ServerMessageType.ScrollWindow
                                    or ServerMessageType.NonScrollWindow
                                    or ServerMessageType.UserOptions
                                    or ServerMessageType.Whisper)
            InnerSendServerMessage(serverMessageType, message);
        else
            foreach (var chunk in Helpers.ChunkMessage(message))
                InnerSendServerMessage(serverMessageType, chunk);

        if (serverMessageType is ServerMessageType.Whisper
                                 or ServerMessageType.OrangeBar1
                                 or ServerMessageType.OrangeBar2
                                 or ServerMessageType.ActiveMessage
                                 or ServerMessageType.OrangeBar3
                                 or ServerMessageType.AdminMessage
                                 or ServerMessageType.OrangeBar5
                                 or ServerMessageType.GroupChat
                                 or ServerMessageType.GuildChat)
            Aisling.Trackers.LastOrangeBarMessage = DateTime.UtcNow;

        return;

        void InnerSendServerMessage(ServerMessageType localType, string localMessage)
        {
            var args = new ServerMessageArgs
            {
                ServerMessageType = localType,
                Message = localMessage
            };

            Send(args);
        }
    }

    public void SendSound(byte sound, bool isMusic)
    {
        if (Aisling is { Options.ListenToHitSounds: false } && !isMusic && (sound == 1))
            return;

        var args = new SoundArgs
        {
            Sound = sound,
            IsMusic = isMusic
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
        foreach (var chunk in objects.OrderBy(o => o.Creation)
                                     .Chunk(5000))
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
                            groundItemInfo.Color = DisplayColor.MatteBlack;
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
            CountryList = worldList
        };

        foreach (var aisling in orderedAislings)
        {
            var arg = Mapper.Map<WorldListMemberInfo>(aisling);

            if (aisling.IsAdmin)
                continue;

            if (Aisling.WithinLevelRange(aisling))
                arg.Color = WorldListColor.WithinLevelRange;

            if (aisling.Guild is not null && (aisling.Guild == Aisling.Guild))
            {
                arg.IsGuilded = true;
                arg.Color = WorldListColor.Guilded;
            }

            worldList.Add(arg);
        }

        args.WorldMemberCount = (ushort)worldList.Count;

        Send(args);
    }

    public void SendWorldMap(WorldMap worldMap)
    {
        var args = Mapper.Map<WorldMapArgs>(worldMap);

        Send(args);
    }

    private async Task DoHeartbeatAsync()
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(WorldOptions.Instance.HeartbeatIntervalSecs));

        while (Connected)
        {
            try
            {
                await timer.WaitForNextTickAsync();

                //if heartbeat is still populated, that means the client has not responded to the last heartbeat
                //assume the client has disconnected
                if (Heartbeat1.HasValue || Heartbeat2.HasValue)
                {
                    Disconnect();

                    return;
                }

                Heartbeat1 = Random.Shared.Next<byte>();
                Heartbeat2 = Random.Shared.Next<byte>();

                SendHeartBeat(Heartbeat1.Value, Heartbeat2.Value);
            } catch
            {
                //ignored
            }
        }
    }

    /// <inheritdoc />
    protected override ValueTask HandlePacketAsync(Span<byte> span)
    {
        var opCode = span[3];
        var packet = new Packet(ref span, Crypto.IsClientEncrypted(opCode));

        if (packet.IsEncrypted)
            Crypto.Decrypt(ref packet);

        if (LogRawPackets)
            Logger.WithTopics(
                      Topics.Servers.WorldServer,
                      Topics.Qualifiers.Raw,
                      Topics.Entities.Client,
                      Topics.Entities.Packet,
                      Topics.Actions.Receive)
                  .WithProperty(this)
                  .LogTrace("[Rcv] {@Packet}", packet.ToString());
        else if (LogReceivePacketCode)
            Logger.WithTopics(
                      Topics.Qualifiers.Raw,
                      Topics.Entities.Client,
                      Topics.Entities.Packet,
                      Topics.Actions.Receive)
                  .WithProperty(this)
                  .LogTrace("Received packet with code {@OpCode} from {@ClientIp}", opCode, RemoteIp);

        return Server.HandlePacketAsync(this, in packet);
    }
}