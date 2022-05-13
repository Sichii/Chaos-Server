using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Chaos.Caches.Interfaces;
using Chaos.Clients.Interfaces;
using Chaos.Containers;
using Chaos.Core.Data;
using Chaos.Core.Definitions;
using Chaos.Core.Extensions;
using Chaos.Core.Geometry;
using Chaos.Core.JsonConverters;
using Chaos.Cryptography;
using Chaos.Factories.Interfaces;
using Chaos.Managers.Interfaces;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Interfaces;
using Chaos.Networking.Model;
using Chaos.Networking.Model.Client;
using Chaos.Objects;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Observers;
using Chaos.Options;
using Chaos.Packets;
using Chaos.Packets.Definitions;
using Chaos.Packets.Interfaces;
using Chaos.Servers.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Servers;

public class WorldServer : ServerBase, IWorldServer
{
    private readonly IClientFactory<IWorldClient> ClientFactory;
    private readonly ISimpleCache<string, Metafile> Metafile;
    private readonly ISaveManager<User> UserSaveManager;
    private readonly IExchangeFactory ExchangeFactory;
    public ConcurrentDictionary<uint, IWorldClient> Clients { get; }

    public IEnumerable<User> Users => Clients
        .Select(kvp => kvp.Value.User)
        .Where(user => user != null!);
    protected new WorldClientHandler?[] ClientHandlers { get; }
    protected override WorldOptions Options { get; }

    public WorldServer(
        IClientFactory<IWorldClient> clientFactory,
        ISimpleCache<string, Metafile> metafile,
        ISaveManager<User> userSaveManager,
        IRedirectManager redirectManager,
        IPacketSerializer packetSerializer,
        IExchangeFactory exchangeFactory,
        IOptionsSnapshot<WorldOptions> options,
        ILogger<WorldServer> logger
    )
        : base(
            redirectManager,
            packetSerializer,
            options,
            logger)
    {
        ClientFactory = clientFactory;
        Metafile = metafile;
        UserSaveManager = userSaveManager;
        ExchangeFactory = exchangeFactory;
        Options = options.Value;
        Clients = new ConcurrentDictionary<uint, IWorldClient>();
        ClientHandlers = new WorldClientHandler[byte.MaxValue];

        IndexHandlers();
    }

    #region OnHandlers
    public ValueTask OnBeginChant(IWorldClient client, ref ClientPacket clientPacket)
    {
        if (!client.User.Status.HasFlag(Status.Dead))
            client.User.UserState |= UserState.IsChanting;
        else
            client.SendCancelCasting();

        return default;
    }

    public ValueTask OnBoardRequest(IWorldClient client, ref ClientPacket clientPacket) => default;

    public ValueTask OnChant(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<DisplayChantArgs>(ref clientPacket);

        client.User.MapInstance.ShowPublicMessage(PublicMessageType.Chant, args.ChantMessage, client.User);

        return default;
    }

    public ValueTask OnClick(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var targetId, var targetPoint) = PacketSerializer.Deserialize<ClickArgs>(ref clientPacket);

        if (targetId.HasValue)
            client.User.MapInstance.Click(targetId.Value, client.User);
        else if (targetPoint.HasValue)
            client.User.MapInstance.Click(targetPoint.Value, client.User);

        return default;
    }

    public ValueTask OnClientRedirected(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ClientRedirectedArgs>(ref clientPacket);

        if (!RedirectManager.TryGetRemove(args.Id, out var redirect))
        {
            Logger.LogWarning("A client tried to redirect to the world with an invalid id. ({Args})", args);
            client.Disconnect();

            return default;
        }

        if (args.Name != redirect.Name)
        {
            Logger.LogCritical("A client tried to impersonate a redirect (Args: {Args}, Redirect: {Redirect})", args, redirect);
            client.Disconnect();

            return default;
        }

        Logger.LogDebug("Received redirect to world. ({Redirect})", redirect);
        var existingUser = Users.FirstOrDefault(user => user.Name.EqualsI(redirect.Name));

        //double logon, disconnect both clients
        if (existingUser != null)
        {
            Logger.LogDebug("Duplicate login detected for {Name}, disconnecting both users", redirect.Name);
            existingUser.Client.Disconnect();
            client.Disconnect();

            return default;
        }

        return OnClientRedirectedAsync(client, args, redirect);
    }

    public async ValueTask OnClientRedirectedAsync(IWorldClient client, ClientRedirectedArgs args, Redirect redirect)
    {
        client.CryptoClient = new CryptoClient(args.Seed, args.Key, args.Name);
        var user = await UserSaveManager.LoadAsync(client, redirect.Name);
        
        client.SendAttributes(StatUpdateType.Full);
        client.SendLightLevel(LightLevel.Lightest);
        client.SendUserId();
        user.MapInstance.AddObject(user, user.Point);
        client.SendProfileRequest();
    }

    public ValueTask OnClientWalk(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ClientWalkArgs>(ref clientPacket);

        client.User.MapInstance.ClientWalk(client.User, args.Direction);

        return default;
    }

    public ValueTask OnDialogResponse(IWorldClient client, ref ClientPacket clientPacket) => throw new NotImplementedException();

    public ValueTask OnEmote(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<EmoteArgs>(ref clientPacket);

        if ((int)args.BodyAnimation <= 44)
            client.User.MapInstance.ShowBodyAnimation(args.BodyAnimation, client.User);

        return default;
    }

    public ValueTask OnExchange(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ExchangeArgs>(ref clientPacket);

        var exchange = client.User.ActiveObject.TryGet<Exchange>();
        
        if (exchange == null)
            return default;
        
        if (exchange.GetOtherUser(client.User).Id != args.OtherPlayerId)
            return default;
        
        switch (args.ExchangeRequestType)
        {
            case ExchangeRequestType.StartExchange:
                Logger.LogError("Someone attempted to directly start an exchange ({UserName})", client.User.Name);
                break;
            case ExchangeRequestType.AddItem:
                exchange.AddItem(client.User, args.SourceSlot!.Value);
                break;
            case ExchangeRequestType.AddStackableItem:
                exchange.AddStackableItem(client.User, args.SourceSlot!.Value, args.ItemCount!.Value);
                break;
            case ExchangeRequestType.SetGold:
                exchange.SetGold(client.User, args.GoldAmount!.Value);
                break;
            case ExchangeRequestType.Cancel:
                exchange.Cancel(client.User);
                break;
            case ExchangeRequestType.Accept:
                exchange.Accept(client.User);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return default;
    }

    public ValueTask OnExitRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ExitRequestArgs>(ref clientPacket);

        if (args.IsRequest)
            client.SendConfirmExit();
        else
        {
            var redirect = new Redirect(client.CryptoClient, Options.LoginRedirect, ServerType.Login);
            RedirectManager.Add(redirect);

            Logger.LogDebug(
                "Redirecting world client to login server at {ServerAddress}:{ServerPort}",
                Options.LoginRedirect.Address,
                Options.LoginRedirect.Port);
            
            client.SendRedirect(redirect);
        }

        return default;
    }

    public ValueTask OnGoldDropped(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var amount, var destinationPoint) = PacketSerializer.Deserialize<GoldDropArgs>(ref clientPacket);
        var map = client.User.MapInstance;

        if (amount <= 0)
            return default;

        using var monitor = map.AutoMonitor.Enter();

        if (!client.User.WithinRange(destinationPoint, Options.DropRange))
            return default;

        if (map.IsWall(destinationPoint))
            return default;

        var currentGold = client.User.Gold;

        if (currentGold < amount)
            return default;

        currentGold -= amount;
        client.User.Gold = currentGold;

        client.SendAttributes(StatUpdateType.ExpGold);

        var money = new Money(amount);
        map.AddObject(money, destinationPoint);

        return default;
    }

    public ValueTask OnGoldDroppedOnCreature(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var amount, var targetId) = PacketSerializer.Deserialize<GoldDroppedOnCreatureArgs>(ref clientPacket);
        var map = client.User.MapInstance;

        if (amount <= 0)
            return default;

        if (client.User.Gold < amount)
            return default;

        using var monitor = map.AutoMonitor.Enter();

        if (!map.TryGetObject<Creature>(targetId, out var target))
            return default;

        if (!client.User.WithinRange(target, Options.TradeRange))
            return default;

        target.GoldDroppedOn(amount, client.User);

        return default;
    }

    public ValueTask OnGroupRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var groupRequestType, var targetName) = PacketSerializer.Deserialize<GroupRequestArgs>(ref clientPacket);
        var target = Users.FirstOrDefault(user => user.Name.EqualsI(targetName));

        if (target == null)
        {
            client.SendServerMessage(ServerMessageType.ActiveMessage, $"{targetName} is nowhere to be found");

            return default;
        }
        
        switch(groupRequestType)
        {
            case GroupRequestType.FormalInvite:
                Logger.LogWarning(
                    "Player \"{Name}\" attempted to send a formal invite to the server. This type of group request is something only the server should send",
                    client.User.Name);
                return default;
            case GroupRequestType.TryInvite:
            {
                var existingGroup = client.User.Group;

                if (existingGroup != null)
                    existingGroup.Invite(client.User, target);
                else if (target.Group != null)
                {
                    client.SendServerMessage(ServerMessageType.ActiveMessage, $"{target.Name} is already in a group");

                    return default;
                } else
                    target.Client.SendGroupRequest(GroupRequestType.FormalInvite, client.User.Name);

                return default;
            }
            case GroupRequestType.AcceptInvite:
            {
                var existingGroup = client.User.Group;

                if (existingGroup != null)
                    client.SendServerMessage(ServerMessageType.ActiveMessage, "You are already in a group");
                else if (target.Group != null)
                    target.Group.AcceptInvite(target, client.User);
                else
                {
                    var group = Group.Create(target, client.User);
                    target.Group = group;
                    client.User.Group = group;
                }
                return default;
            }
            case GroupRequestType.Groupbox:
                //TODO: implement this maybe

                return default;
            case GroupRequestType.RemoveGroupBox:
                //TODO: implement this maybe

                return default;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public ValueTask OnIgnore(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var ignoreType, var targetName) = PacketSerializer.Deserialize<IgnoreArgs>(ref clientPacket);

        switch (ignoreType)
        {
            case IgnoreType.Request:
                client.SendServerMessage(ServerMessageType.ScrollWindow, client.User.IgnoreList.ToString());

                break;
            case IgnoreType.AddUser:
                if (!string.IsNullOrEmpty(targetName))
                    client.User.IgnoreList.Add(targetName);

                break;
            case IgnoreType.RemoveUser:
                if (!string.IsNullOrEmpty(targetName))
                    client.User.IgnoreList.Remove(targetName);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return default;
    }

    public ValueTask OnItemDropped(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var sourceSlot, var destinationPoint, var count) = PacketSerializer.Deserialize<ItemDropArgs>(ref clientPacket);

        var map = client.User.MapInstance;

        using var monitor = map.AutoMonitor.Enter();

        if (map.IsWall(destinationPoint))
            return default;

        if (!client.User.WithinRange(destinationPoint, Options.DropRange))
            return default;

        if (!client.User.Inventory.RemoveQuantity(sourceSlot, count, out var item))
            return default;

        map.AddObject(item.ToGroundItem(), destinationPoint);

        return default;
    }

    public ValueTask OnItemDroppedOnCreature(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var sourceSlot, var targetId, var count) = PacketSerializer.Deserialize<ItemDroppedOnCreatureArgs>(ref clientPacket);

        var map = client.User.MapInstance;

        using var monitor = map.AutoMonitor.Enter();

        if (!map.TryGetObject<Creature>(targetId, out var target))
            return default;

        if (!client.User.WithinRange(target, Options.TradeRange))
            return default;

        if (!client.User.Inventory.TryGetObject(sourceSlot, out var item))
            return default;

        if (item == null)
            return default;

        if (item.Count < count)
            return default;

        target.ItemDroppedOn(sourceSlot, count, client.User);

        return default;
    }

    public ValueTask OnMapDataRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        client.SendMapData();

        return default;
    }

    public ValueTask OnMetafileRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var metafileRequestType, var name) = PacketSerializer.Deserialize<MetafileRequestArgs>(ref clientPacket);

        switch (metafileRequestType)
        {
            case MetafileRequestType.DataByName:
                client.SendMetafile(MetafileRequestType.DataByName, Metafile, name);

                break;
            case MetafileRequestType.AllCheckSums:
                client.SendMetafile(MetafileRequestType.AllCheckSums, Metafile);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return default;
    }

    public ValueTask OnPickup(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var destinationSlot, var sourcePoint) = PacketSerializer.Deserialize<PickupArgs>(ref clientPacket);

        var map = client.User.MapInstance;

        using var monitor = map.AutoMonitor.Enter();

        if (!client.User.WithinRange(sourcePoint, Options.PickupRange))
            return default;

        var obj = map.ObjectsAtPoint<GroundItem>(sourcePoint).FirstOrDefault();

        if (obj == null)
            return default;

        if (!client.User.CanCarry(obj.Item))
            return default;

        if (client.User.Inventory.TryAdd(destinationSlot, obj.Item))
            map.RemoveObject(obj);

        return default;
    }

    public ValueTask OnProfile(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var portraitData, var profileMessage) = PacketSerializer.Deserialize<ProfileArgs>(ref clientPacket);

        client.User.Portrait = portraitData;
        client.User.ProfileMessage = profileMessage;

        return default;
    }

    public ValueTask OnProfileRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        client.SendSelfProfile();

        return default;
    }

    public ValueTask OnPublicMessage(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var publicMessageType, var message) = PacketSerializer.Deserialize<PublicMessageArgs>(ref clientPacket);

        client.User.MapInstance.ShowPublicMessage(publicMessageType, $"{client.User.Name}: {message}", client.User);

        return default;
    }

    public ValueTask OnPursuitRequest(IWorldClient client, ref ClientPacket clientPacket) => throw new NotImplementedException();

    public ValueTask OnRaiseStat(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<RaiseStatArgs>(ref clientPacket);

        if (client.User.StatSheet.UnspentPoints > 0)
            if (client.User.StatSheet.AddStat(args.Stat))
                client.SendAttributes(StatUpdateType.Full);

        return default;
    }

    public ValueTask OnRefreshRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        client.User.MapInstance.Refresh(client.User);

        return default;
    }

    public ValueTask OnSocialStatus(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<SocialStatusArgs>(ref clientPacket);

        client.User.SocialStatus = args.SocialStatus;

        return default;
    }

    public ValueTask OnSpacebar(IWorldClient client, ref ClientPacket clientPacket) =>
        //TODO: assails i guess
        default;

    public ValueTask OnSwapSlot(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var panelType, var slot1, var slot2) = PacketSerializer.Deserialize<SwapSlotArgs>(ref clientPacket);

        switch (panelType)
        {
            case PanelType.Inventory:
                client.User.Inventory.TrySwap(slot1, slot2);

                break;
            case PanelType.SpellBook:
                client.User.SpellBook.TrySwap(slot1, slot2);

                break;
            case PanelType.SkillBook:
                client.User.SkillBook.TrySwap(slot1, slot2);

                break;
            case PanelType.Equipment:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return default;
    }

    public ValueTask OnToggleGroup(IWorldClient client, ref ClientPacket clientPacket)
    {
        //don't need to send the updated option, because they arent currently looking at it
        client.User.Options.Toggle(UserOption.Group);

        if (client.User.Group != null)
            client.User.Group?.Leave(client.User);
        else
            client.SendSelfProfile();

        return default;
    }

    public ValueTask OnTurn(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<TurnArgs>(ref clientPacket);

        client.User.MapInstance.ShowTurn(args.Direction, client.User);

        return default;
    }

    public ValueTask OnUnequip(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<UnequipArgs>(ref clientPacket);

        if (client.User.Inventory.IsFull)
            return default;

        if (!client.User.Equipment.TryGetRemove((byte)args.EquipmentSlot, out var item))
            return default;

        if (item == null)
            return default;

        item.Script.OnUnequip(client.User);
        client.User.Inventory.TryAddToNextSlot(item);

        return default;
    }

    public ValueTask OnUseItem(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ItemUseArgs>(ref clientPacket);

        if (client.User.Inventory.TryGetObject(args.SourceSlot, out var item))
            item?.Script.OnUse(client.User);

        return default;
    }

    public ValueTask OnUserOptionToggle(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<UserOptionToggleArgs>(ref clientPacket);

        if (args.UserOption == UserOption.Request)
        {
            client.SendServerMessage(ServerMessageType.UserOptions, client.User.Options.ToString());

            return default;
        }

        client.User.Options.Toggle(args.UserOption);
        client.SendServerMessage(ServerMessageType.UserOptions, client.User.Options.ToString(args.UserOption));

        return default;
    }

    public ValueTask OnUseSkill(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<SkillUseArgs>(ref clientPacket);

        if (client.User.SkillBook.TryGetObject(args.SourceSlot, out var skill))
            skill?.Script.OnUse(client.User);

        return default;
    }

    public ValueTask OnUseSpell(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var sourceSlot, var argsData) = PacketSerializer.Deserialize<SpellUseArgs>(ref clientPacket);
        var mapInstance = client.User.MapInstance;

        if (client.User.SpellBook.TryGetObject(sourceSlot, out var spell) && (spell != null))
        {
            using var sync = mapInstance.AutoMonitor.Enter();

            var source = (Creature)client.User;
            var target = source;
            var prompt = default(string?);

            //it's impossible to know what kind of spell is being used during deserialization
            //there is no spell type specified in the packet, so we arent sure if the packet will
            //contains a prompt or target info
            //so we have to do that deserialization here, where we know what spell type we're dealing with
            //we also need to build the activation context for the spell
            switch (spell.Template.SpellType)
            {
                case SpellType.None:
                    return default;
                case SpellType.Prompt:
                    prompt = PacketSerializer.Encoding.GetString(argsData);
                    target = source;

                    break;
                case SpellType.Targeted:
                    var targetIdSegment = new ArraySegment<byte>(argsData, 0, 4);
                    var targetPointSegment = new ArraySegment<byte>(argsData, 4, 4);

                    var targetId = (uint)((targetIdSegment[0] << 24)
                                          | (targetIdSegment[1] << 16)
                                          | (targetIdSegment[2] << 8)
                                          | targetIdSegment[3]);

                    // ReSharper disable once UnusedVariable
                    Point targetPoint = ((targetPointSegment[0] << 8) | targetPointSegment[1],
                        (targetPointSegment[2] << 8) | targetPointSegment[3]);

                    if (mapInstance.TryGetObject<Creature>(targetId, out var creature))
                        target = creature;

                    break;

                case SpellType.Prompt1Num:
                case SpellType.Prompt2Nums:
                case SpellType.Prompt3Nums:
                case SpellType.Prompt4Nums:
                    break;

                case SpellType.NoTarget:
                    target = source;

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            var context = new ActivationContext(target, source, prompt);
            spell.Script.OnUse(context);
        }

        return default;
    }

    public ValueTask OnWhisper(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var targetName, var message) = PacketSerializer.Deserialize<WhisperArgs>(ref clientPacket);

        var targetUser = Users.FirstOrDefault(user => user.Name.EqualsI(targetName));

        if (message.Length > 100)
            return default;

        if (targetUser == null)
        {
            client.SendServerMessage(ServerMessageType.ActiveMessage, $"{targetName} is not online");

            return default;
        }

        if (targetUser.Equals(client.User))
        {
            client.SendServerMessage(ServerMessageType.Whisper, "Talking to yourself?");

            return default;
        }

        if (targetUser.SocialStatus == SocialStatus.DoNotDisturb)
        {
            client.SendServerMessage(ServerMessageType.Whisper, $"{targetUser.Name} doesn't want to be bothered");

            return default;
        }

        //if someone is being ignored, they shouldnt know it
        //let them waste their time typing for no reason
        if (targetUser.IgnoreList.ContainsI(client.User.Name))
        {
            Logger.LogInformation(
                "Ignored by: {TargetName}, From: {FromName}, Message: {Message}",
                targetUser.Name,
                client.User.Name,
                message);

            client.SendServerMessage(ServerMessageType.Whisper, $"[{targetUser.Name}] > {message}");

            return default;
        }

        client.SendServerMessage(ServerMessageType.Whisper, $"[{targetUser.Name}] > {message}");
        targetUser.Client.SendServerMessage(ServerMessageType.Whisper, $"[{client.User.Name}] < {message}");

        return default;
    }

    public ValueTask OnWorldListRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        client.SendWorldList(Users.ToList());

        return default;
    }

    public ValueTask OnWorldMapClick(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<WorldMapClickArgs>(ref clientPacket);

        //TODO: world maps dont exist yet

        return default;
    }
    
    #endregion
    
    #region Other Duties
    public Exchange CreateExchange(User sender, User receiver) => ExchangeFactory.CreateExchange(sender, receiver);
    #endregion

    #region Connection / Handler
    protected delegate ValueTask WorldClientHandler(IWorldClient client, ref ClientPacket packet);

    public override ValueTask HandlePacketAsync<TClient>(TClient client, ref ClientPacket packet)
    {
        if (client is IWorldClient worldClient)
        {
            var handler = ClientHandlers[(byte)packet.OpCode];

            return handler?.Invoke(worldClient, ref packet) ?? default;
        }

        return base.HandlePacketAsync(client, ref packet);
    }

    protected sealed override void IndexHandlers()
    {
        if (ClientHandlers == null!)
            return;

        base.IndexHandlers();
        var oldHandlers = base.ClientHandlers;

        for (var i = 0; i < byte.MaxValue; i++)
        {
            var old = oldHandlers[i];

            if (old == null)
                continue;

            ClientHandlers[i] = new WorldClientHandler(old);
        }

        //ClientHandlers[(byte)ClientOpCode.] =
        ClientHandlers[(byte)ClientOpCode.RequestMapData] = OnMapDataRequest;
        ClientHandlers[(byte)ClientOpCode.ClientWalk] = OnClientWalk;
        ClientHandlers[(byte)ClientOpCode.Pickup] = OnPickup;
        ClientHandlers[(byte)ClientOpCode.ItemDrop] = OnItemDropped;
        ClientHandlers[(byte)ClientOpCode.ExitRequest] = OnExitRequest;
        //ClientHandlers[(byte)ClientOpCode.DisplayObjectRequest] =
        ClientHandlers[(byte)ClientOpCode.Ignore] = OnIgnore;
        ClientHandlers[(byte)ClientOpCode.PublicMessage] = OnPublicMessage;
        ClientHandlers[(byte)ClientOpCode.SpellUse] = OnUseSpell;
        ClientHandlers[(byte)ClientOpCode.ClientRedirected] = OnClientRedirected;
        ClientHandlers[(byte)ClientOpCode.Turn] = OnTurn;
        ClientHandlers[(byte)ClientOpCode.SpaceBar] = OnSpacebar;
        ClientHandlers[(byte)ClientOpCode.RequestWorldList] = OnWorldListRequest;
        ClientHandlers[(byte)ClientOpCode.Whisper] = OnWhisper;
        ClientHandlers[(byte)ClientOpCode.UserOptionToggle] = OnUserOptionToggle;
        ClientHandlers[(byte)ClientOpCode.ItemUse] = OnUseItem;
        ClientHandlers[(byte)ClientOpCode.Emote] = OnEmote;
        ClientHandlers[(byte)ClientOpCode.GoldDrop] = OnGoldDropped;
        ClientHandlers[(byte)ClientOpCode.ItemDroppedOnCreature] = OnItemDroppedOnCreature;
        ClientHandlers[(byte)ClientOpCode.GoldDroppedOnCreature] = OnGoldDroppedOnCreature;
        ClientHandlers[(byte)ClientOpCode.RequestProfile] = OnProfileRequest;
        ClientHandlers[(byte)ClientOpCode.GroupRequest] = OnGroupRequest;
        ClientHandlers[(byte)ClientOpCode.ToggleGroup] = OnToggleGroup;
        ClientHandlers[(byte)ClientOpCode.SwapSlot] = OnSwapSlot;
        ClientHandlers[(byte)ClientOpCode.RequestRefresh] = OnRefreshRequest;
        ClientHandlers[(byte)ClientOpCode.PursuitRequest] = OnPursuitRequest;
        ClientHandlers[(byte)ClientOpCode.DialogResponse] = OnDialogResponse;
        ClientHandlers[(byte)ClientOpCode.BoardRequest] = OnBoardRequest;
        ClientHandlers[(byte)ClientOpCode.SkillUse] = OnUseSkill;
        ClientHandlers[(byte)ClientOpCode.WorldMapClick] = OnWorldMapClick;
        ClientHandlers[(byte)ClientOpCode.Click] = OnClick;
        ClientHandlers[(byte)ClientOpCode.Unequip] = OnUnequip;
        ClientHandlers[(byte)ClientOpCode.RaiseStat] = OnRaiseStat;
        ClientHandlers[(byte)ClientOpCode.Exchange] = OnExchange;
        ClientHandlers[(byte)ClientOpCode.BeginChant] = OnBeginChant;
        ClientHandlers[(byte)ClientOpCode.Chant] = OnChant;
        ClientHandlers[(byte)ClientOpCode.Profile] = OnProfile;
        ClientHandlers[(byte)ClientOpCode.SocialStatus] = OnSocialStatus;
        ClientHandlers[(byte)ClientOpCode.MetafileRequest] = OnMetafileRequest;
    }

    protected override void OnConnection(IAsyncResult ar)
    {
        var serverSocket = (Socket)ar.AsyncState!;
        var clientSocket = serverSocket.EndAccept(ar);

        serverSocket.BeginAccept(OnConnection, serverSocket);

        var client = ClientFactory.CreateClient(clientSocket);

        if (!Clients.TryAdd(client.Id, client))
        {
            Logger.LogError("Somehow two clients got the same id. (Id: {Id})", client.Id);
            client.Disconnect();

            return;
        }

        client.OnDisconnected += OnDisconnect;

        client.BeginReceive();
    }

    private void OnDisconnect(object? sender, EventArgs e)
    {
        var client = (IWorldClient)sender!;
        Clients.TryRemove(client.Id, out _);

        client.User.MapInstance?.RemoveObject(client.User);
        SaveUser(client.User);
    }

    private async void SaveUser(User user)
    {
        try
        {
            await UserSaveManager.SaveAsync(user);
        } catch (Exception e)
        {
            var desperationOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                IgnoreReadOnlyProperties = false,
                IgnoreReadOnlyFields = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                AllowTrailingCommas = true
            };

            desperationOptions.Converters.Add(new PointConverter());
            desperationOptions.Converters.Add(new JsonStringEnumConverter());
            Logger.LogError(e, "Exception while saving user. {User}", JsonSerializer.Serialize(user));
        }
    }
    #endregion
}