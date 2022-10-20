using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Chaos.Clients.Abstractions;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Core.Identity;
using Chaos.Core.Utilities;
using Chaos.Cryptography;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Geometry.JsonConverters;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities.Client;
using Chaos.Networking.Options;
using Chaos.Objects.Dialog;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Servers;

public sealed class WorldServer : ServerBase<IWorldClient>, IWorldServer
{
    private readonly ISimpleCacheProvider CacheProvider;
    private readonly IClientFactory<IWorldClient> ClientFactory;
    private readonly ICommandInterceptor<Aisling> CommandInterceptor;
    private readonly ParallelOptions ParallelOptions;
    private readonly PeriodicTimer PeriodicTimer;
    private readonly ISaveManager<Aisling> UserSaveManager;

    public IEnumerable<Aisling> Aislings => Clients
                                            .Select(kvp => kvp.Value.Aisling)
                                            .Where(user => user != null!);
    public ConcurrentDictionary<uint, IWorldClient> Clients { get; }
    protected override WorldOptions Options { get; }

    public WorldServer(
        IClientFactory<IWorldClient> clientFactory,
        ISimpleCacheProvider cacheProvider,
        ISaveManager<Aisling> userSaveManager,
        IRedirectManager redirectManager,
        IPacketSerializer packetSerializer,
        ICommandInterceptor<Aisling> commandInterceptor,
        IOptionsSnapshot<WorldOptions> options,
        ILogger<WorldServer> logger
    )
        : base(
            redirectManager,
            packetSerializer,
            options,
            logger)
    {
        Options = options.Value;
        PeriodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000.0 / options.Value.UpdatesPerSecond));
        ClientFactory = clientFactory;
        CacheProvider = cacheProvider;
        UserSaveManager = userSaveManager;
        CommandInterceptor = commandInterceptor;

        Clients = new ConcurrentDictionary<uint, IWorldClient>();

        ParallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        IndexHandlers();
    }

    #region Server Loop
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        var endPoint = new IPEndPoint(IPAddress.Any, Options.Port);
        Socket.Bind(endPoint);
        Socket.Listen(20);
        Socket.BeginAccept(OnConnection, Socket);
        Logger.LogInformation("Listening on {EndPoint}", endPoint);

        var deltaTime = new DeltaTime();
        var saveInterval = new IntervalTimer(TimeSpan.FromMinutes(Options.SaveIntervalMins));

        while (true)
            try
            {
                if (stoppingToken.IsCancellationRequested)
                    return;

                await PeriodicTimer.WaitForNextTickAsync(stoppingToken);
                var delta = deltaTime.ElapsedSpan;
                saveInterval.Update(delta);

                if (saveInterval.IntervalElapsed)
                    await ParallelSaveAsync();

                await ParallelUpdateAsync(delta);
            } catch (Exception e)
            {
                Logger.LogError(e, "Server update loop had an unhandled exception");
            }
    }

    private Task ParallelUpdateAsync(TimeSpan delta) => Parallel.ForEachAsync(
        CacheProvider.GetCache<MapInstance>(),
        ParallelOptions,
        (mapInstance, _) =>
        {
            try
            {
                mapInstance.Update(delta);
            } catch (Exception e)
            {
                Logger.LogCritical(e, "Failed to update map instance {MapInstance}", mapInstance.InstanceId);
            }

            return default;
        });

    //only needs to be async because it enters a semaphore
    private Task ParallelSaveAsync() =>
        Parallel.ForEachAsync(
            Aislings,
            ParallelOptions,
            async (user, _) =>
            {
                await user.Client.ReceiveSync.WaitAsync();

                try
                {
                    await SaveUserAsync(user);
                } catch (Exception e)
                {
                    Logger.LogCritical(e, "Failed to save user {User}", user.Name);
                } finally
                {
                    user.Client.ReceiveSync.Release();
                }
            });

    private async Task SaveUserAsync(Aisling aisling)
    {
        try
        {
            await UserSaveManager.SaveAsync(aisling);
        } catch (Exception e)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                IgnoreReadOnlyProperties = false,
                IgnoreReadOnlyFields = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                AllowTrailingCommas = true
            };

            jsonSerializerOptions.Converters.Add(new PointConverter());
            jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            Logger.LogError(e, "Exception while saving user. {User}", JsonSerializer.Serialize(aisling));
        }
    }
    #endregion

    #region OnHandlers
    public ValueTask OnBeginChant(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<BeginChantArgs>(ref clientPacket);

        if (client.Aisling.Status.HasFlag(Status.Dead))
        {
            client.SendCancelCasting();

            return default;
        }

        client.Aisling.UserState |= UserState.IsChanting;

        return default;
    }

    public ValueTask OnBoardRequest(IWorldClient client, ref ClientPacket clientPacket) => default;

    public ValueTask OnChant(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<DisplayChantArgs>(ref clientPacket);

        client.Aisling.Chant(args.ChantMessage);

        return default;
    }

    public ValueTask OnClick(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var targetId, var targetPoint) = PacketSerializer.Deserialize<ClickArgs>(ref clientPacket);

        if (targetId.HasValue)
            client.Aisling.MapInstance.Click(targetId.Value, client.Aisling);
        else if (targetPoint is not null)
            client.Aisling.MapInstance.Click(targetPoint, client.Aisling);

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
        var existingUser = Aislings.FirstOrDefault(user => user.Name.EqualsI(redirect.Name));

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

    public async ValueTask OnClientRedirectedAsync(IWorldClient client, ClientRedirectedArgs args, IRedirect redirect)
    {
        client.CryptoClient = new CryptoClient(args.Seed, args.Key, args.Name);
        var aisling = await UserSaveManager.LoadAsync(redirect.Name);

        client.Aisling = aisling;
        aisling.Client = client;
        aisling.BeginObserving();

        client.SendAttributes(StatUpdateType.Full);
        client.SendLightLevel(LightLevel.Lightest);
        client.SendUserId();
        aisling.MapInstance.AddObject(aisling, aisling);
        client.SendProfileRequest();
        aisling.MapInstance.ActivateReactors(aisling, ReactorActivationType.Walk);
    }

    public ValueTask OnClientWalk(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ClientWalkArgs>(ref clientPacket);

        client.Aisling.Walk(args.Direction);

        return default;
    }

    public ValueTask OnDialogResponse(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<DialogResponseArgs>(ref clientPacket);

        var currentDialogContext = client.Aisling.ActiveObject.TryGet<DialogContext>();

        //if there was no dialog, close whatever they think they have open
        if (currentDialogContext != null)
        {
            client.SendDialog(Dialog.CloseDialog, client.Aisling);

            return default;
        }
        
        

        return default;
    }

    public ValueTask OnEmote(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<EmoteArgs>(ref clientPacket);

        if ((int)args.BodyAnimation <= 44)
            client.Aisling.AnimateBody(args.BodyAnimation);

        return default;
    }

    public ValueTask OnExchange(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ExchangeArgs>(ref clientPacket);

        var exchange = client.Aisling.ActiveObject.TryGet<Exchange>();

        if (exchange == null)
            return default;

        if (exchange.GetOtherUser(client.Aisling).Id != args.OtherPlayerId)
            return default;

        switch (args.ExchangeRequestType)
        {
            case ExchangeRequestType.StartExchange:
                Logger.LogError("Someone attempted to directly start an exchange ({UserName})", client.Aisling.Name);

                break;
            case ExchangeRequestType.AddItem:
                exchange.AddItem(client.Aisling, args.SourceSlot!.Value);

                break;
            case ExchangeRequestType.AddStackableItem:
                exchange.AddStackableItem(client.Aisling, args.SourceSlot!.Value, args.ItemCount!.Value);

                break;
            case ExchangeRequestType.SetGold:
                exchange.SetGold(client.Aisling, args.GoldAmount!.Value);

                break;
            case ExchangeRequestType.Cancel:
                exchange.Cancel(client.Aisling);

                break;
            case ExchangeRequestType.Accept:
                exchange.Accept(client.Aisling);

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
            var redirect = new Redirect(
                ClientId.NextId,
                Options.LoginRedirect,
                ServerType.Login,
                client.CryptoClient.Key,
                client.CryptoClient.Seed);

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
        var map = client.Aisling.MapInstance;

        if (amount <= 0)
            return default;

        if (!client.Aisling.WithinRange(destinationPoint, Options.DropRange))
            return default;

        if (map.IsWall(destinationPoint))
            return default;

        var currentGold = client.Aisling.Gold;

        if (currentGold < amount)
            return default;

        currentGold -= amount;
        client.Aisling.Gold = currentGold;

        client.SendAttributes(StatUpdateType.ExpGold);

        var money = new Money(amount, map, destinationPoint);
        map.AddObject(money, destinationPoint);

        return default;
    }

    public ValueTask OnGoldDroppedOnCreature(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var amount, var targetId) = PacketSerializer.Deserialize<GoldDroppedOnCreatureArgs>(ref clientPacket);
        var map = client.Aisling.MapInstance;

        if (amount <= 0)
            return default;

        if (client.Aisling.Gold < amount)
            return default;

        if (!map.TryGetObject<Creature>(targetId, out var target))
            return default;

        if (!client.Aisling.WithinRange(target, Options.TradeRange))
            return default;

        target.OnGoldDroppedOn(client.Aisling, amount);

        return default;
    }

    public ValueTask OnGroupRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var groupRequestType, var targetName) = PacketSerializer.Deserialize<GroupRequestArgs>(ref clientPacket);
        var target = Aislings.FirstOrDefault(user => user.Name.EqualsI(targetName));

        if (target == null)
        {
            client.SendServerMessage(ServerMessageType.ActiveMessage, $"{targetName} is nowhere to be found");

            return default;
        }

        switch (groupRequestType)
        {
            case GroupRequestType.FormalInvite:
                Logger.LogWarning(
                    "Player \"{Name}\" attempted to send a formal invite to the server. This type of group request is something only the server should send",
                    client.Aisling.Name);

                return default;
            case GroupRequestType.TryInvite:
            {
                var existingGroup = client.Aisling.Group;

                if (existingGroup != null)
                    existingGroup.Invite(client.Aisling, target);
                else if (target.Group != null)
                {
                    client.SendServerMessage(ServerMessageType.ActiveMessage, $"{target.Name} is already in a group");

                    return default;
                } else
                    target.Client.SendGroupRequest(GroupRequestType.FormalInvite, client.Aisling.Name);

                return default;
            }
            case GroupRequestType.AcceptInvite:
            {
                var existingGroup = client.Aisling.Group;

                if (existingGroup != null)
                    client.SendServerMessage(ServerMessageType.ActiveMessage, "You are already in a group");
                else if (target.Group != null)
                    target.Group.AcceptInvite(target, client.Aisling);
                else
                {
                    var group = Group.Create(target, client.Aisling);
                    target.Group = group;
                    client.Aisling.Group = group;
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
                client.SendServerMessage(ServerMessageType.ScrollWindow, client.Aisling.IgnoreList.ToString());

                break;
            case IgnoreType.AddUser:
                if (!string.IsNullOrEmpty(targetName))
                    client.Aisling.IgnoreList.Add(targetName);

                break;
            case IgnoreType.RemoveUser:
                if (!string.IsNullOrEmpty(targetName))
                    client.Aisling.IgnoreList.Remove(targetName);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return default;
    }

    public ValueTask OnItemDropped(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var sourceSlot, var destinationPoint, var count) = PacketSerializer.Deserialize<ItemDropArgs>(ref clientPacket);

        var map = client.Aisling.MapInstance;

        if (map.IsWall(destinationPoint))
            return default;

        if (!client.Aisling.WithinRange(destinationPoint, Options.DropRange))
            return default;

        if (!client.Aisling.Inventory.RemoveQuantity(sourceSlot, count, out var item))
            return default;

        Logger.LogDebug("{UserName} dropped {Item}", client.Aisling.Name, item);
        client.Aisling.Drop(destinationPoint, item);

        return default;
    }

    public ValueTask OnItemDroppedOnCreature(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var sourceSlot, var targetId, var count) = PacketSerializer.Deserialize<ItemDroppedOnCreatureArgs>(ref clientPacket);

        var map = client.Aisling.MapInstance;

        if (!map.TryGetObject<Creature>(targetId, out var target))
            return default;

        if (!client.Aisling.WithinRange(target, Options.TradeRange))
            return default;

        if (!client.Aisling.Inventory.TryGetObject(sourceSlot, out var item))
            return default;

        if (item.Count < count)
            return default;

        target.OnItemDroppedOn(client.Aisling, sourceSlot, count);

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
        var metafileCache = CacheProvider.GetCache<Metafile>();

        switch (metafileRequestType)
        {
            case MetafileRequestType.DataByName:
                client.SendMetafile(MetafileRequestType.DataByName, metafileCache, name);

                break;
            case MetafileRequestType.AllCheckSums:
                client.SendMetafile(MetafileRequestType.AllCheckSums, metafileCache);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return default;
    }

    public ValueTask OnPickup(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var destinationSlot, var sourcePoint) = PacketSerializer.Deserialize<PickupArgs>(ref clientPacket);

        var map = client.Aisling.MapInstance;

        if (!client.Aisling.WithinRange(sourcePoint, Options.PickupRange))
            return default;

        var obj = map.GetEntitiesAtPoint<GroundEntity>(sourcePoint)
                     .TopOrDefault();

        if (obj == null)
            return default;

        switch (obj)
        {
            case GroundItem groundItem:
                if (!client.Aisling.CanCarry(groundItem.Item))
                {
                    client.SendServerMessage(
                        ServerMessageType.ActiveMessage,
                        $"You can't carry {groundItem.Name}. (Weight: {groundItem.Item.Template.Weight})");

                    return default;
                }

                client.Aisling.PickupItem(groundItem, destinationSlot);

                break;
            case Money money:
                if (client.Aisling.Gold + money.Amount > Options.MaxGoldHeld)
                {
                    client.SendServerMessage(
                        ServerMessageType.ActiveMessage,
                        $"You can't hold more than {Options.MaxGoldHeld} gold");

                    return default;
                }

                client.Aisling.PickupMoney(money);

                break;
        }

        return default;
    }

    public ValueTask OnProfile(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var portraitData, var profileMessage) = PacketSerializer.Deserialize<ProfileArgs>(ref clientPacket);

        client.Aisling.Portrait = portraitData;
        client.Aisling.ProfileText = profileMessage;

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

        if (CommandInterceptor.IsCommand(message))
        {
            CommandInterceptor.HandleCommand(client.Aisling, message);

            return default;
        }

        client.Aisling.ShowPublicMessage(publicMessageType, $"{client.Aisling.Name}: {message}");

        return default;
    }

    public ValueTask OnPursuitRequest(IWorldClient client, ref ClientPacket clientPacket) => throw new NotImplementedException();

    public ValueTask OnRaiseStat(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<RaiseStatArgs>(ref clientPacket);

        if (client.Aisling.UserStatSheet.UnspentPoints > 0)
            if (client.Aisling.UserStatSheet.IncrementStat(args.Stat))
            {
                if (args.Stat == Stat.STR)
                    client.Aisling.UserStatSheet.RecalculateMaxWeight();

                client.SendAttributes(StatUpdateType.Full);
            }

        return default;
    }

    public ValueTask OnRefreshRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        client.Aisling.Refresh();

        return default;
    }

    public ValueTask OnSocialStatus(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<SocialStatusArgs>(ref clientPacket);

        client.Aisling.SocialStatus = args.SocialStatus;

        return default;
    }

    public ValueTask OnSpacebar(IWorldClient client, ref ClientPacket clientPacket)
    {
        foreach (var skill in client.Aisling.SkillBook)
            if (skill.Template.IsAssail)
                client.Aisling.TryUseSkill(skill);

        return default;
    }

    public ValueTask OnSwapSlot(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var panelType, var slot1, var slot2) = PacketSerializer.Deserialize<SwapSlotArgs>(ref clientPacket);

        switch (panelType)
        {
            case PanelType.Inventory:
                client.Aisling.Inventory.TrySwap(slot1, slot2);

                break;
            case PanelType.SpellBook:
                client.Aisling.SpellBook.TrySwap(slot1, slot2);

                break;
            case PanelType.SkillBook:
                client.Aisling.SkillBook.TrySwap(slot1, slot2);

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
        client.Aisling.Options.Toggle(UserOption.Group);

        if (client.Aisling.Group != null)
            client.Aisling.Group?.Leave(client.Aisling);
        else
            client.SendSelfProfile();

        return default;
    }

    public ValueTask OnTurn(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<TurnArgs>(ref clientPacket);

        client.Aisling.Turn(args.Direction);

        return default;
    }

    public ValueTask OnUnequip(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<UnequipArgs>(ref clientPacket);

        client.Aisling.UnEquip(args.EquipmentSlot);

        return default;
    }

    public ValueTask OnUseItem(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ItemUseArgs>(ref clientPacket);

        if (client.Aisling.Inventory.TryGetObject(args.SourceSlot, out var item))
        {
            var exchange = client.Aisling.ActiveObject.TryGet<Exchange>();

            if (exchange != null)
            {
                exchange.AddItem(client.Aisling, item.Slot);

                return default;
            }

            item.Script.OnUse(client.Aisling);
        }

        return default;
    }

    public ValueTask OnUserOptionToggle(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<UserOptionToggleArgs>(ref clientPacket);

        if (args.UserOption == UserOption.Request)
        {
            client.SendServerMessage(ServerMessageType.UserOptions, client.Aisling.Options.ToString());

            return default;
        }

        client.Aisling.Options.Toggle(args.UserOption);
        client.SendServerMessage(ServerMessageType.UserOptions, client.Aisling.Options.ToString(args.UserOption));

        return default;
    }

    public ValueTask OnUseSkill(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<SkillUseArgs>(ref clientPacket);

        client.Aisling.TryUseSkill(args.SourceSlot);

        return default;
    }

    public ValueTask OnUseSpell(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var sourceSlot, var argsData) = PacketSerializer.Deserialize<SpellUseArgs>(ref clientPacket);

        if (client.Aisling.SpellBook.TryGetObject(sourceSlot, out var spell))
        {
            var source = (Creature)client.Aisling;
            var prompt = default(string?);
            uint? targetId = null;

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

                    break;
                case SpellType.Targeted:
                    var targetIdSegment = new ArraySegment<byte>(argsData, 0, 4);
                    var targetPointSegment = new ArraySegment<byte>(argsData, 4, 4);

                    targetId = (uint)((targetIdSegment[0] << 24)
                                      | (targetIdSegment[1] << 16)
                                      | (targetIdSegment[2] << 8)
                                      | targetIdSegment[3]);

                    // ReSharper disable once UnusedVariable
                    var targetPoint = new Point(
                        (targetPointSegment[0] << 8) | targetPointSegment[1],
                        (targetPointSegment[2] << 8) | targetPointSegment[3]);

                    break;

                case SpellType.Prompt1Num:
                case SpellType.Prompt2Nums:
                case SpellType.Prompt3Nums:
                case SpellType.Prompt4Nums:
                    break;

                case SpellType.NoTarget:
                    targetId = source.Id;

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            client.Aisling.TryUseSpell(spell, targetId, prompt);
        }

        return default;
    }

    public ValueTask OnWhisper(IWorldClient client, ref ClientPacket clientPacket)
    {
        (var targetName, var message) = PacketSerializer.Deserialize<WhisperArgs>(ref clientPacket);

        var targetUser = Aislings.FirstOrDefault(user => user.Name.EqualsI(targetName));

        if (message.Length > 100)
            return default;

        if (targetUser == null)
        {
            client.SendServerMessage(ServerMessageType.ActiveMessage, $"{targetName} is not online");

            return default;
        }

        if (targetUser.Equals(client.Aisling))
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
        if (targetUser.IgnoreList.ContainsI(client.Aisling.Name))
        {
            Logger.LogInformation(
                "Message sent by {FromName} was ignored by {TargetName} (Message: \"{Message}\")",
                client.Aisling.Name,
                targetUser.Name,
                message);

            client.SendServerMessage(ServerMessageType.Whisper, $"[{targetUser.Name}] > {message}");

            return default;
        }

        client.SendServerMessage(ServerMessageType.Whisper, $"[{targetUser.Name}] > {message}");
        targetUser.Client.SendServerMessage(ServerMessageType.Whisper, $"[{client.Aisling.Name}] < {message}");

        return default;
    }

    public ValueTask OnWorldListRequest(IWorldClient client, ref ClientPacket clientPacket)
    {
        client.SendWorldList(Aislings.ToList());

        return default;
    }

    public ValueTask OnWorldMapClick(IWorldClient client, ref ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<WorldMapClickArgs>(ref clientPacket);

        var worldMapTile = client.Aisling.MapInstance.GetEntitiesAtPoint<WorldMapTile>(client.Aisling)
                                     .FirstOrDefault();

        if (worldMapTile == null)
            return default;

        if (!worldMapTile.WorldMap.Nodes.TryGetValue(args.UniqueId, out var node))
            return default;

        node.OnClick(client.Aisling);
        
        return default;
    }
    #endregion

    #region Connection / Handler
    public override ValueTask HandlePacketAsync(IWorldClient client, ref ClientPacket packet)
    {
        var handler = ClientHandlers[(byte)packet.OpCode];

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var mapInstance = client.Aisling?.MapInstance;

        if (mapInstance == null)
            return handler?.Invoke(client, ref packet) ?? default;

        //yes, i know this handler is returning a ValueTask
        //however, only 1 handler ever calls asynchronous code, and thus only 1 handler will ever run asynchronously
        //that handler also happens to be the one where the user is loaded, and wont have a map instance when we receive the packet, so it's ok
        //methods that return tasks that do not ever call asynchronous code, will run entirely synchronously, so this is fine
        using var mapSync = mapInstance.Sync.EnterWithSafeExit();

        return handler?.Invoke(client, ref packet) ?? default;
    }

    protected override void IndexHandlers()
    {
        if (ClientHandlers == null!)
            return;

        base.IndexHandlers();

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

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        client.Aisling.MapInstance?.RemoveObject(client.Aisling);
        AsyncHelpers.RunSync(() => SaveUserAsync(client.Aisling));
    }
    #endregion
}