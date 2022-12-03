using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Chaos.Clients.Abstractions;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Abstractions;
using Chaos.Common.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Identity;
using Chaos.Common.Synchronization;
using Chaos.Containers;
using Chaos.Cryptography;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities.Client;
using Chaos.Networking.Options;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;
using Chaos.Services.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Servers;

public sealed class WorldServer : ServerBase<IWorldClient>, IWorldServer<IWorldClient>
{
    private readonly ISimpleCacheProvider CacheProvider;
    private readonly IClientFactory<IWorldClient> ClientFactory;
    private readonly ICommandInterceptor<Aisling> CommandInterceptor;
    private readonly DeltaMonitor DeltaMonitor;
    private readonly DeltaTime DeltaTime;
    private readonly IGroupService GroupService;
    private readonly ParallelOptions ParallelOptions;
    private readonly PeriodicTimer PeriodicTimer;
    private readonly IIntervalTimer SaveTimer;
    private readonly ISaveManager<Aisling> UserSaveManager;

    public IEnumerable<Aisling> Aislings => ClientRegistry
                                            .Select(c => c.Aisling)
                                            .Where(player => player != null!);
    protected override WorldOptions Options { get; }

    public WorldServer(
        IClientRegistry<IWorldClient> clientRegistry,
        IClientFactory<IWorldClient> clientFactory,
        ISimpleCacheProvider cacheProvider,
        ISaveManager<Aisling> userSaveManager,
        IRedirectManager redirectManager,
        IPacketSerializer packetSerializer,
        ICommandInterceptor<Aisling> commandInterceptor,
        IGroupService groupService,
        IOptionsSnapshot<WorldOptions> options,
        ILogger<WorldServer> logger
    )
        : base(
            redirectManager,
            packetSerializer,
            clientRegistry,
            options,
            logger)
    {
        Options = options.Value;
        var delta = 1000.0 / options.Value.UpdatesPerSecond;
        PeriodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(delta));
        DeltaTime = new DeltaTime();
        DeltaMonitor = new DeltaMonitor(logger, TimeSpan.FromMinutes(1), delta);
        SaveTimer = new IntervalTimer(TimeSpan.FromMinutes(Options.SaveIntervalMins), false);
        ClientFactory = clientFactory;
        CacheProvider = cacheProvider;
        UserSaveManager = userSaveManager;
        CommandInterceptor = commandInterceptor;
        GroupService = groupService;

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

        while (true)
        {
            if (stoppingToken.IsCancellationRequested)
                return;

            await PeriodicTimer.WaitForNextTickAsync(stoppingToken);

            try
            {
                DeltaTime.SetDelta();

                var start = ValueStopwatch.GetTimestamp();
                await ParallelUpdateAsync();
                var end = ValueStopwatch.GetTimestamp();

                var executionDelta = ValueStopwatch.GetElapsedTime(start, end);
                DeltaMonitor.AddExecutionDelta(executionDelta);
                DeltaMonitor.Update(DeltaTime.DeltaSpan);
            } catch (Exception e)
            {
                Logger.LogError(e, "Server update loop had an unhandled exception");
            }
        }
    }

    private Task ParallelUpdateAsync()
    {
        SaveTimer.Update(DeltaTime.DeltaSpan);

        /*
        foreach (var map in CacheProvider.GetCache<MapInstance>())
        {
            await using var sync = await map.Sync.WaitAsync();
            map.Update(delta);

            if (SaveTimer.IntervalElapsed)
                await Task.WhenAll(map.GetEntities<Aisling>().Select(SaveUserAsync));
        }*/

        return Parallel.ForEachAsync(CacheProvider.GetCache<MapInstance>(), ParallelOptions, UpdateMap);
    }

    private async ValueTask UpdateMap(MapInstance map, CancellationToken token)
    {
        await using var sync = await map.Sync.WaitAsync();

        try
        {
            map.Update(DeltaTime.DeltaSpan);

            if (SaveTimer.IntervalElapsed)
                await Task.WhenAll(map.GetEntities<Aisling>().Select(SaveUserAsync));
        } catch (Exception e)
        {
            Logger.LogCritical(e, "Failed to update map instance {MapInstance}", map);
        }
    }

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

            Logger.LogCritical(e, "Exception while saving user. {Player}", JsonSerializer.Serialize(aisling, jsonSerializerOptions));
        }
    }
    #endregion

    #region OnHandlers
    public ValueTask OnBeginChant(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<BeginChantArgs>(in clientPacket);

        static ValueTask InnerOnBeginChant(IWorldClient localClient, BeginChantArgs localArgs)
        {
            if (localClient.Aisling.Status.HasFlag(Status.Dead))
            {
                localClient.SendCancelCasting();

                return default;
            }

            localClient.Aisling.UserState |= UserState.IsChanting;
            localClient.Aisling.ChantTimer.Start(localArgs.CastLineCount);

            return default;
        }

        return ExecuteHandler(client, args, InnerOnBeginChant);
    }

    public ValueTask OnBoardRequest(IWorldClient client, in ClientPacket clientPacket)
    {
        static ValueTask InnerOnBoardRequest(IWorldClient localClient)
        {
            //TODO: maybe implement board, but not sure if it's worth it
            localClient.SendBoard();

            return default;
        }

        /*
        //this packet is literally retarded
        private void Board(Client client, ClientPacket packet)
        {
            var type = (BoardRequestType)packet.ReadByte();

            switch (type) //request type
            {
                case BoardRequestType.BoardList:
                    //Board List
                    //client.Enqueue(client.ServerPackets.BulletinBoard);
                    break;
                case BoardRequestType.ViewBoard:
                    {
                        //Post list for boardNum
                        ushort boardNum = packet.ReadUInt16();
                        ushort startPostNum = packet.ReadUInt16(); //you send the newest mail first, which will have the highest number. startPostNum counts down.
                        //packet.ReadByte() is always 0xF0(240) ???
                        //the client spam requests this like holy fuck, put a timer on this so you only send 1 packet
                        break;
                    }
                case BoardRequestType.ViewPost:
                    {
                        //Post
                        ushort boardNum = packet.ReadUInt16();
                        ushort postId = packet.ReadUInt16(); //the post number they want, counting up (what the fuck?)
                        //mailbox = boardNum 0
                        //otherwise boardnum is the index of the board you're accessing
                        switch (packet.ReadSByte()) //board controls
                        {
                            case -1: //clicked next for older post
                                break;
                            case 0: //requested a specific post from the post list
                                break;
                            case 1: //clicked previous for newer post
                                break;
                        }
                        break;
                    }
                case BoardRequestType.NewPost: //new post
                    {
                        ushort boardNum = packet.ReadUInt16();
                        string subject = packet.ReadString8();
                        string message = packet.ReadString16();
                        break;
                    }
                case BoardRequestType.Delete: //delete post
                    {
                        ushort boardNum = packet.ReadUInt16();
                        ushort postId = packet.ReadUInt16(); //the post number they want to delete, counting up
                        break;
                    }

                case BoardRequestType.SendMail: //send mail
                    {
                        ushort boardNum = packet.ReadUInt16();
                        string targetName = packet.ReadString8();
                        string subject = packet.ReadString8();
                        string message = packet.ReadString16();
                        break;
                    }
                case BoardRequestType.Highlight: //highlight message
                    {
                        ushort boardNum = packet.ReadUInt16();
                        ushort postId = packet.ReadUInt16();
                        break;
                    }
            }

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] TYPE: {type}", client);
            Game.Boards(client);
        }
         */

        return ExecuteHandler(client, InnerOnBoardRequest);
    }

    public ValueTask OnChant(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<DisplayChantArgs>(in clientPacket);

        static ValueTask InnerOnChant(IWorldClient localClient, DisplayChantArgs localArgs)
        {
            localClient.Aisling.Chant(localArgs.ChantMessage);

            return default;
        }

        return ExecuteHandler(client, args, InnerOnChant);
    }

    public ValueTask OnClick(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ClickArgs>(in clientPacket);

        static ValueTask InnerOnClick(IWorldClient localClient, ClickArgs localArgs)
        {
            (var targetId, var targetPoint) = localArgs;

            if (targetId.HasValue)
                localClient.Aisling.MapInstance.Click(targetId.Value, localClient.Aisling);
            else if (targetPoint is not null)
                localClient.Aisling.MapInstance.Click(targetPoint, localClient.Aisling);

            return default;
        }

        return ExecuteHandler(client, args, InnerOnClick);
    }

    public ValueTask OnClientRedirected(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ClientRedirectedArgs>(in clientPacket);

        ValueTask InnerOnClientRedirected(IWorldClient localClient, ClientRedirectedArgs localArgs)
        {
            if (!RedirectManager.TryGetRemove(localArgs.Id, out var redirect))
            {
                Logger.LogWarning("A client tried to redirect to the world with an invalid id. ({Args})", localArgs);
                localClient.Disconnect();

                return default;
            }

            if (localArgs.Name != redirect.Name)
            {
                Logger.LogCritical("A client tried to impersonate a redirect (Args: {Args}, Redirect: {Redirect})", localArgs, redirect);
                localClient.Disconnect();

                return default;
            }

            Logger.LogDebug("Received redirect to world. ({Redirect})", redirect);
            var existingUser = Aislings.FirstOrDefault(user => user.Name.EqualsI(redirect.Name));

            //double logon, disconnect both clients
            if (existingUser != null)
            {
                Logger.LogDebug("Duplicate login detected for {Name}, disconnecting both users", redirect.Name);
                existingUser.Client.Disconnect();
                localClient.Disconnect();

                return default;
            }

            return OnClientRedirectedAsync(localClient, localArgs, redirect);
        }

        return ExecuteHandler(client, args, InnerOnClientRedirected);
    }

    public async ValueTask OnClientRedirectedAsync(IWorldClient client, ClientRedirectedArgs args, IRedirect redirect)
    {
        client.CryptoClient = new CryptoClient(args.Seed, args.Key, args.Name);
        var aisling = await UserSaveManager.LoadAsync(redirect.Name);

        client.Aisling = aisling;
        aisling.Client = client;

        await using var sync = await aisling.MapInstance.Sync.WaitAsync();

        try
        {
            aisling.BeginObserving();
            client.SendAttributes(StatUpdateType.Full);
            client.SendLightLevel(LightLevel.Lightest);
            client.SendUserId();
            aisling.MapInstance.AddObject(aisling, aisling);
            client.SendProfileRequest();

            foreach (var reactor in aisling.MapInstance.GetEntitiesAtPoint<ReactorTile>(Point.From(aisling)))
                reactor.OnWalkedOn(aisling);
        } catch (Exception e)
        {
            Logger.LogCritical(e, "{Player} failed to load", aisling);
        }
    }

    public ValueTask OnClientWalk(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ClientWalkArgs>(in clientPacket);

        static ValueTask InnerOnClientWalk(IWorldClient localClient, ClientWalkArgs localArgs)
        {
            //if player is in a world map, dont allow them to walk
            if (localClient.Aisling.ActiveObject.TryGet<WorldMap>() != null)
                return default;

            localClient.Aisling.Walk(localArgs.Direction);

            return default;
        }

        return ExecuteHandler(client, args, InnerOnClientWalk);
    }

    public ValueTask OnDialogResponse(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<DialogResponseArgs>(in clientPacket);

        ValueTask InnerOnDialogResponse(IWorldClient localClient, DialogResponseArgs localArgs)
        {
            var dialog = localClient.Aisling.ActiveDialog.Get();

            if (dialog == null)
            {
                Logger.LogWarning("No active dialog found for {UserName}", localClient.Aisling.Name);

                return default;
            }

            //since we always send a dialog id of 0, we can easily get the result without comparing ids
            var dialogResult = (DialogResult)localArgs.DialogId;

            if (localArgs.Args != null)
                dialog.MenuArgs = new ArgumentCollection(localArgs.Args);

            switch (dialogResult)
            {
                case DialogResult.Previous:
                    dialog.Previous(localClient.Aisling);

                    break;
                case DialogResult.Close:
                    //we dont actually need to send them a close if they alrdy closed it
                    break;
                case DialogResult.Next:
                    dialog.Next(localClient.Aisling, localArgs.Option);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return default;
        }

        return ExecuteHandler(client, args, InnerOnDialogResponse);
    }

    public ValueTask OnEmote(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<EmoteArgs>(in clientPacket);

        ValueTask InnerOnEmote(IWorldClient localClient, EmoteArgs localArgs)
        {
            if ((int)localArgs.BodyAnimation <= 44)
                client.Aisling.AnimateBody(localArgs.BodyAnimation);

            return default;
        }

        return ExecuteHandler(client, args, InnerOnEmote);
    }

    public ValueTask OnExchange(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ExchangeArgs>(in clientPacket);

        ValueTask InnerOnExchange(IWorldClient localClient, ExchangeArgs localArgs)
        {
            var exchange = localClient.Aisling.ActiveObject.TryGet<Exchange>();

            if (exchange == null)
                return default;

            if (exchange.GetOtherUser(localClient.Aisling).Id != localArgs.OtherPlayerId)
                return default;

            switch (localArgs.ExchangeRequestType)
            {
                case ExchangeRequestType.StartExchange:
                    Logger.LogError("Someone attempted to directly start an exchange ({UserName})", localClient.Aisling.Name);

                    break;
                case ExchangeRequestType.AddItem:
                    exchange.AddItem(localClient.Aisling, localArgs.SourceSlot!.Value);

                    break;
                case ExchangeRequestType.AddStackableItem:
                    exchange.AddStackableItem(localClient.Aisling, localArgs.SourceSlot!.Value, localArgs.ItemCount!.Value);

                    break;
                case ExchangeRequestType.SetGold:
                    exchange.SetGold(localClient.Aisling, localArgs.GoldAmount!.Value);

                    break;
                case ExchangeRequestType.Cancel:
                    exchange.Cancel(localClient.Aisling);

                    break;
                case ExchangeRequestType.Accept:
                    exchange.Accept(localClient.Aisling);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return default;
        }

        return ExecuteHandler(client, args, InnerOnExchange);
    }

    public ValueTask OnExitRequest(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ExitRequestArgs>(in clientPacket);

        ValueTask InnerOnExitRequest(IWorldClient localClient, ExitRequestArgs localArgs)
        {
            if (localArgs.IsRequest)
                localClient.SendConfirmExit();
            else
            {
                var redirect = new Redirect(
                    ClientId.NextId,
                    Options.LoginRedirect,
                    ServerType.Login,
                    localClient.CryptoClient.Key,
                    localClient.CryptoClient.Seed);

                RedirectManager.Add(redirect);

                Logger.LogDebug(
                    "Redirecting world client to login server at {ServerAddress}:{ServerPort}",
                    Options.LoginRedirect.Address,
                    Options.LoginRedirect.Port);

                localClient.SendRedirect(redirect);
            }

            return default;
        }

        return ExecuteHandler(client, args, InnerOnExitRequest);
    }

    public ValueTask OnGoldDropped(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<GoldDropArgs>(in clientPacket);

        ValueTask InnerOnGoldDropped(IWorldClient localClient, GoldDropArgs localArgs)
        {
            (var amount, var destinationPoint) = localArgs;
            var map = localClient.Aisling.MapInstance;

            if (!localClient.Aisling.WithinRange(destinationPoint, Options.DropRange))
                return default;

            if (map.IsWall(destinationPoint))
                return default;

            localClient.Aisling.DropGold(destinationPoint, amount);

            return default;
        }

        return ExecuteHandler(client, args, InnerOnGoldDropped);
    }

    public ValueTask OnGoldDroppedOnCreature(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<GoldDroppedOnCreatureArgs>(in clientPacket);

        ValueTask InnerOnGoldDroppedOnCreature(IWorldClient localClient, GoldDroppedOnCreatureArgs localArgs)
        {
            (var amount, var targetId) = localArgs;

            var map = localClient.Aisling.MapInstance;

            if (amount <= 0)
                return default;

            if (!map.TryGetObject<Creature>(targetId, out var target))
                return default;

            if (!localClient.Aisling.WithinRange(target, Options.TradeRange))
                return default;

            target.OnGoldDroppedOn(localClient.Aisling, amount);

            return default;
        }

        return ExecuteHandler(client, args, InnerOnGoldDroppedOnCreature);
    }

    public ValueTask OnGroupRequest(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<GroupRequestArgs>(in clientPacket);

        ValueTask InnerOnGroupRequest(IWorldClient localClient, GroupRequestArgs localArgs)
        {
            (var groupRequestType, var targetName) = localArgs;
            var target = Aislings.FirstOrDefault(user => user.Name.EqualsI(targetName));

            if (target == null)
            {
                localClient.SendServerMessage(ServerMessageType.ActiveMessage, $"{targetName} is nowhere to be found");

                return default;
            }

            var localAisling = localClient.Aisling;

            switch (groupRequestType)
            {
                case GroupRequestType.FormalInvite:
                    Logger.LogWarning(
                        "{Player} attempted to send a formal invite to the server. This type of group request is something only the server should send",
                        localAisling);

                    return default;
                case GroupRequestType.TryInvite:
                {
                    GroupService.Invite(localAisling, target);

                    return default;
                }
                case GroupRequestType.AcceptInvite:
                {
                    GroupService.AcceptInvite(target, localAisling);

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

        return ExecuteHandler(client, args, InnerOnGroupRequest);
    }

    public ValueTask OnIgnore(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<IgnoreArgs>(in clientPacket);

        static ValueTask InnerOnIgnore(IWorldClient localClient, IgnoreArgs localArgs)
        {
            (var ignoreType, var targetName) = localArgs;

            switch (ignoreType)
            {
                case IgnoreType.Request:
                    localClient.SendServerMessage(ServerMessageType.ScrollWindow, localClient.Aisling.IgnoreList.ToString());

                    break;
                case IgnoreType.AddUser:
                    if (!string.IsNullOrEmpty(targetName))
                        localClient.Aisling.IgnoreList.Add(targetName);

                    break;
                case IgnoreType.RemoveUser:
                    if (!string.IsNullOrEmpty(targetName))
                        localClient.Aisling.IgnoreList.Remove(targetName);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return default;
        }

        return ExecuteHandler(client, args, InnerOnIgnore);
    }

    public ValueTask OnItemDropped(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ItemDropArgs>(in clientPacket);

        static ValueTask InnerOnItemDropped(IWorldClient localClient, ItemDropArgs localArgs)
        {
            (var sourceSlot, var destinationPoint, var count) = localArgs;

            localClient.Aisling.Drop(destinationPoint, sourceSlot, count);

            return default;
        }

        return ExecuteHandler(client, args, InnerOnItemDropped);
    }

    public ValueTask OnItemDroppedOnCreature(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ItemDroppedOnCreatureArgs>(in clientPacket);

        ValueTask InnerOnItemDroppedOnCreature(IWorldClient localClient, ItemDroppedOnCreatureArgs localArgs)
        {
            (var sourceSlot, var targetId, var count) = localArgs;
            var map = localClient.Aisling.MapInstance;

            if (!map.TryGetObject<Creature>(targetId, out var target))
                return default;

            if (!localClient.Aisling.WithinRange(target, Options.TradeRange))
                return default;

            if (!localClient.Aisling.Inventory.TryGetObject(sourceSlot, out var item))
                return default;

            if (item.Count < count)
                return default;

            target.OnItemDroppedOn(localClient.Aisling, sourceSlot, count);

            return default;
        }

        return ExecuteHandler(client, args, InnerOnItemDroppedOnCreature);
    }

    public ValueTask OnMapDataRequest(IWorldClient client, in ClientPacket clientPacket)
    {
        static ValueTask InnerOnMapDataRequest(IWorldClient localClient)
        {
            localClient.SendMapData();

            return default;
        }

        return ExecuteHandler(client, InnerOnMapDataRequest);
    }

    public ValueTask OnMetafileRequest(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<MetafileRequestArgs>(in clientPacket);

        ValueTask InnerOnMetafileRequest(IWorldClient localClient, MetafileRequestArgs localArgs)
        {
            (var metafileRequestType, var name) = localArgs;
            var metafileCache = CacheProvider.GetCache<Metafile>();

            switch (metafileRequestType)
            {
                case MetafileRequestType.DataByName:
                    localClient.SendMetafile(MetafileRequestType.DataByName, metafileCache, name);

                    break;
                case MetafileRequestType.AllCheckSums:
                    localClient.SendMetafile(MetafileRequestType.AllCheckSums, metafileCache);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return default;
        }

        return ExecuteHandler(client, args, InnerOnMetafileRequest);
    }

    public ValueTask OnPickup(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<PickupArgs>(in clientPacket);

        ValueTask InnerOnPickup(IWorldClient localClient, PickupArgs localArgs)
        {
            (var destinationSlot, var sourcePoint) = localArgs;
            var map = localClient.Aisling.MapInstance;

            if (!localClient.Aisling.WithinRange(sourcePoint, Options.PickupRange))
                return default;

            var obj = map.GetEntitiesAtPoint<GroundEntity>(sourcePoint)
                         .TopOrDefault();

            var reactor = map.GetEntitiesAtPoint<ReactorTile>(sourcePoint)
                             .TopOrDefault();

            if (obj == null)
                return default;

            switch (obj)
            {
                case GroundItem groundItem:
                    localClient.Aisling.PickupItem(groundItem, destinationSlot);
                    reactor?.OnItemPickedUpFrom(localClient.Aisling, groundItem);

                    break;
                case Money money:
                    localClient.Aisling.PickupMoney(money);
                    reactor?.OnGoldPickedUpFrom(localClient.Aisling, money);

                    break;
            }

            return default;
        }

        return ExecuteHandler(client, args, InnerOnPickup);
    }

    public ValueTask OnProfile(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ProfileArgs>(in clientPacket);

        static ValueTask InnerOnProfile(IWorldClient localClient, ProfileArgs localArgs)
        {
            (var portraitData, var profileMessage) = localArgs;
            localClient.Aisling.Portrait = portraitData;
            localClient.Aisling.ProfileText = profileMessage;

            return default;
        }

        return ExecuteHandler(client, args, InnerOnProfile);
    }

    public ValueTask OnProfileRequest(IWorldClient client, in ClientPacket clientPacket)
    {
        static ValueTask InnerOnProfileRequest(IWorldClient localClient)
        {
            localClient.SendSelfProfile();

            return default;
        }

        return ExecuteHandler(client, InnerOnProfileRequest);
    }

    public ValueTask OnPublicMessage(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<PublicMessageArgs>(in clientPacket);

        async ValueTask InnerOnPublicMessage(IWorldClient localClient, PublicMessageArgs localArgs)
        {
            (var publicMessageType, var message) = localArgs;

            if (CommandInterceptor.IsCommand(message))
            {
                await CommandInterceptor.HandleCommandAsync(localClient.Aisling, message);

                return;
            }

            localClient.Aisling.ShowPublicMessage(publicMessageType, message);
        }

        return ExecuteHandler(client, args, InnerOnPublicMessage);
    }

    public ValueTask OnPursuitRequest(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<PursuitRequestArgs>(in clientPacket);

        ValueTask InnerOnPursuitRequest(IWorldClient localClient, PursuitRequestArgs localArgs)
        {
            var dialog = localClient.Aisling.ActiveDialog.Get();

            if (dialog == null)
            {
                Logger.LogWarning("No active dialog found for {UserName}", localClient.Aisling.Name);

                return default;
            }

            if (localArgs.Args != null)
                dialog.MenuArgs = new ArgumentCollection(localArgs.Args);

            dialog.Next(localClient.Aisling, (byte)localArgs.PursuitId);

            return default;
        }

        return ExecuteHandler(client, args, InnerOnPursuitRequest);
    }

    public ValueTask OnRaiseStat(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<RaiseStatArgs>(in clientPacket);

        static ValueTask InnerOnRaiseStat(IWorldClient localClient, RaiseStatArgs localArgs)
        {
            if (localClient.Aisling.UserStatSheet.UnspentPoints > 0)
                if (localClient.Aisling.UserStatSheet.IncrementStat(localArgs.Stat))
                {
                    if (localArgs.Stat == Stat.STR)
                        localClient.Aisling.UserStatSheet.RecalculateMaxWeight();

                    localClient.SendAttributes(StatUpdateType.Full);
                }

            return default;
        }

        return ExecuteHandler(client, args, InnerOnRaiseStat);
    }

    public ValueTask OnRefreshRequest(IWorldClient client, in ClientPacket clientPacket)
    {
        static ValueTask InnerOnRefreshRequest(IWorldClient localClient)
        {
            localClient.Aisling.Refresh();

            return default;
        }

        return ExecuteHandler(client, InnerOnRefreshRequest);
    }

    public ValueTask OnSocialStatus(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<SocialStatusArgs>(in clientPacket);

        static ValueTask InnerOnSocialStatus(IWorldClient localClient, SocialStatusArgs localArgs)
        {
            localClient.Aisling.SocialStatus = localArgs.SocialStatus;

            return default;
        }

        return ExecuteHandler(client, args, InnerOnSocialStatus);
    }

    public ValueTask OnSpacebar(IWorldClient client, in ClientPacket clientPacket)
    {
        static ValueTask InnerOnSpacebar(IWorldClient localClient)
        {
            foreach (var skill in localClient.Aisling.SkillBook)
                if (skill.Template.IsAssail)
                    localClient.Aisling.TryUseSkill(skill);

            return default;
        }

        return ExecuteHandler(client, InnerOnSpacebar);
    }

    public ValueTask OnSwapSlot(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<SwapSlotArgs>(in clientPacket);

        static ValueTask InnerOnSwapSlot(IWorldClient localClient, SwapSlotArgs localArgs)
        {
            (var panelType, var slot1, var slot2) = localArgs;

            switch (panelType)
            {
                case PanelType.Inventory:
                    localClient.Aisling.Inventory.TrySwap(slot1, slot2);

                    break;
                case PanelType.SpellBook:
                    localClient.Aisling.SpellBook.TrySwap(slot1, slot2);

                    break;
                case PanelType.SkillBook:
                    localClient.Aisling.SkillBook.TrySwap(slot1, slot2);

                    break;
                case PanelType.Equipment:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return default;
        }

        return ExecuteHandler(client, args, InnerOnSwapSlot);
    }

    public ValueTask OnToggleGroup(IWorldClient client, in ClientPacket clientPacket)
    {
        static ValueTask InnerOnToggleGroup(IWorldClient localClient)
        {
            //don't need to send the updated option, because they arent currently looking at it
            localClient.Aisling.Options.Toggle(UserOption.Group);

            if (localClient.Aisling.Group != null)
                localClient.Aisling.Group?.Leave(localClient.Aisling);
            else
                localClient.SendSelfProfile();

            return default;
        }

        return ExecuteHandler(client, InnerOnToggleGroup);
    }

    public ValueTask OnTurn(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<TurnArgs>(in clientPacket);

        static ValueTask InnerOnTurn(IWorldClient localClient, TurnArgs localArgs)
        {
            localClient.Aisling.Turn(localArgs.Direction);

            return default;
        }

        return ExecuteHandler(client, args, InnerOnTurn);
    }

    public ValueTask OnUnequip(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<UnequipArgs>(in clientPacket);

        static ValueTask InnerOnUnequip(IWorldClient localClient, UnequipArgs localArgs)
        {
            localClient.Aisling.UnEquip(localArgs.EquipmentSlot);

            return default;
        }

        return ExecuteHandler(client, args, InnerOnUnequip);
    }

    public ValueTask OnUseItem(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<ItemUseArgs>(in clientPacket);

        static ValueTask InnerOnUseItem(IWorldClient localClient, ItemUseArgs localArgs)
        {
            if (localClient.Aisling.Inventory.TryGetObject(localArgs.SourceSlot, out var item))
            {
                var exchange = localClient.Aisling.ActiveObject.TryGet<Exchange>();

                if (exchange != null)
                {
                    exchange.AddItem(localClient.Aisling, item.Slot);

                    return default;
                }

                item.Script.OnUse(localClient.Aisling);
            }

            return default;
        }

        return ExecuteHandler(client, args, InnerOnUseItem);
    }

    public ValueTask OnUserOptionToggle(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<UserOptionToggleArgs>(in clientPacket);

        static ValueTask InnerOnUsrOptionToggle(IWorldClient localClient, UserOptionToggleArgs localArgs)
        {
            if (localArgs.UserOption == UserOption.Request)
            {
                localClient.SendServerMessage(ServerMessageType.UserOptions, localClient.Aisling.Options.ToString());

                return default;
            }

            localClient.Aisling.Options.Toggle(localArgs.UserOption);
            localClient.SendServerMessage(ServerMessageType.UserOptions, localClient.Aisling.Options.ToString(localArgs.UserOption));

            return default;
        }

        return ExecuteHandler(client, args, InnerOnUsrOptionToggle);
    }

    public ValueTask OnUseSkill(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<SkillUseArgs>(in clientPacket);

        static ValueTask InnerOnUseSkill(IWorldClient localClient, SkillUseArgs localArgs)
        {
            localClient.Aisling.TryUseSkill(localArgs.SourceSlot);

            return default;
        }

        return ExecuteHandler(client, args, InnerOnUseSkill);
    }

    public ValueTask OnUseSpell(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<SpellUseArgs>(in clientPacket);

        ValueTask InnerOnUseSpell(IWorldClient localClient, SpellUseArgs localArgs)
        {
            (var sourceSlot, var argsData) = localArgs;

            if (localClient.Aisling.SpellBook.TryGetObject(sourceSlot, out var spell))
            {
                var source = (Creature)localClient.Aisling;
                var prompt = default(string?);
                uint? targetId = null;

                //if we expect the spell we're casting to be more than 0 lines
                //it should have started a chant... so we check the chant timer for validation
                if ((spell.CastLines > 0) && !localClient.Aisling.ChantTimer.Validate(spell.CastLines))
                    return default;

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
                    case SpellType.NoTarget:
                        targetId = source.Id;

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                localClient.Aisling.TryUseSpell(spell, targetId, prompt);
            }

            localClient.Aisling.UserState &= ~UserState.IsChanting;

            return default;
        }

        return ExecuteHandler(client, args, InnerOnUseSpell);
    }

    public ValueTask OnWhisper(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<WhisperArgs>(in clientPacket);

        ValueTask InnerOnWhisper(IWorldClient localClient, WhisperArgs localArgs)
        {
            (var targetName, var message) = localArgs;
            var targetUser = Aislings.FirstOrDefault(user => user.Name.EqualsI(targetName));

            if (message.Length > 100)
                return default;

            if (targetUser == null)
            {
                localClient.SendServerMessage(ServerMessageType.ActiveMessage, $"{targetName} is not online");

                return default;
            }

            if (targetUser.Equals(localClient.Aisling))
            {
                localClient.SendServerMessage(ServerMessageType.Whisper, "Talking to yourself?");

                return default;
            }

            if (targetUser.SocialStatus == SocialStatus.DoNotDisturb)
            {
                localClient.SendServerMessage(ServerMessageType.Whisper, $"{targetUser.Name} doesn't want to be bothered");

                return default;
            }

            //if someone is being ignored, they shouldnt know it
            //let them waste their time typing for no reason
            if (targetUser.IgnoreList.ContainsI(localClient.Aisling.Name))
            {
                Logger.LogInformation(
                    "Message sent by {FromName} was ignored by {TargetName} (Message: \"{Message}\")",
                    localClient.Aisling.Name,
                    targetUser.Name,
                    message);

                localClient.SendServerMessage(ServerMessageType.Whisper, $"[{targetUser.Name}] > {message}");

                return default;
            }

            localClient.SendServerMessage(ServerMessageType.Whisper, $"[{targetUser.Name}] > {message}");
            targetUser.Client.SendServerMessage(ServerMessageType.Whisper, $"[{localClient.Aisling.Name}] < {message}");

            return default;
        }

        return ExecuteHandler(client, args, InnerOnWhisper);
    }

    public ValueTask OnWorldListRequest(IWorldClient client, in ClientPacket clientPacket)
    {
        ValueTask InnerOnWorldListRequest(IWorldClient localClient)
        {
            localClient.SendWorldList(Aislings.ToList());

            return default;
        }

        return ExecuteHandler(client, InnerOnWorldListRequest);
    }

    public ValueTask OnWorldMapClick(IWorldClient client, in ClientPacket clientPacket)
    {
        var args = PacketSerializer.Deserialize<WorldMapClickArgs>(in clientPacket);

        static ValueTask InnerOnWorldMapClick(IWorldClient localClient, WorldMapClickArgs localArgs)
        {
            var worldMap = localClient.Aisling.ActiveObject.TryGet<WorldMap>();

            //if player is not in a world map, return
            if (worldMap == null)
                return default;

            if (!worldMap.Nodes.TryGetValue(localArgs.UniqueId, out var node))
                return default;

            node.OnClick(localClient.Aisling);

            return default;
        }

        return ExecuteHandler(client, args, InnerOnWorldMapClick);
    }
    #endregion

    #region Connection / Handler
    public async ValueTask ExecuteHandler<TArgs>(IWorldClient client, TArgs args, Func<IWorldClient, TArgs, ValueTask> action)
    {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var mapInstance = client.Aisling?.MapInstance;
        IPolyDisposable disposable;

        if (mapInstance == null)
            disposable = new NoOpDisposable();
        else
        {
            //await entrancy into the map synchronization
            disposable = await mapInstance.Sync.WaitAsync();

            //if for some reason we changed maps while waiting entrancy, we need to recheck
            while (mapInstance != client.Aisling!.MapInstance)
            {
                disposable.Dispose();
                mapInstance = client.Aisling.MapInstance;
                disposable = await mapInstance.Sync.WaitAsync();
            }
        }

        await using var sync = disposable;

        try
        {
            await action(client, args);
        } catch (Exception e)
        {
            Logger.LogError(
                e,
                "Failed to execute inner handler with args {@Args} for client {@Client}",
                args,
                client);
        }
    }

    public async ValueTask ExecuteHandler(IWorldClient client, Func<IWorldClient, ValueTask> action)
    {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var mapInstance = client.Aisling?.MapInstance;
        IPolyDisposable disposable;

        if (mapInstance == null)
            disposable = new NoOpDisposable();
        else
        {
            //await entrancy into the map synchronization
            disposable = await mapInstance.Sync.WaitAsync();

            //if for some reason we changed maps while waiting entrancy, we need to recheck
            while (mapInstance != client.Aisling!.MapInstance)
            {
                disposable.Dispose();
                mapInstance = client.Aisling.MapInstance;
                disposable = await mapInstance.Sync.WaitAsync();
            }
        }

        await using var sync = disposable;

        try
        {
            await action(client);
        } catch (Exception e)
        {
            Logger.LogError(e, "Failed to execute inner handler for client {@Client}", client);
        }
    }

    public override ValueTask HandlePacketAsync(IWorldClient client, in ClientPacket packet)
    {
        var handler = ClientHandlers[(byte)packet.OpCode];

        return handler?.Invoke(client, in packet) ?? default;
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
        ClientHandlers[(byte)ClientOpCode.UseSpell] = OnUseSpell;
        ClientHandlers[(byte)ClientOpCode.ClientRedirected] = OnClientRedirected;
        ClientHandlers[(byte)ClientOpCode.Turn] = OnTurn;
        ClientHandlers[(byte)ClientOpCode.SpaceBar] = OnSpacebar;
        ClientHandlers[(byte)ClientOpCode.WorldListRequest] = OnWorldListRequest;
        ClientHandlers[(byte)ClientOpCode.Whisper] = OnWhisper;
        ClientHandlers[(byte)ClientOpCode.UserOptionToggle] = OnUserOptionToggle;
        ClientHandlers[(byte)ClientOpCode.UseItem] = OnUseItem;
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
        ClientHandlers[(byte)ClientOpCode.UseSkill] = OnUseSkill;
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

        if (!ClientRegistry.TryAdd(client))
        {
            Logger.LogError("Somehow two clients got the same id. (Id: {Id})", client.Id);
            client.Disconnect();

            return;
        }

        client.OnDisconnected += OnDisconnect;
        client.BeginReceive();
    }

    private async void OnDisconnect(object? sender, EventArgs e)
    {
        //we dont need to Task.Run this because it's async void
        //when async void reaches async code, control returns to the caller and the method is not awaited

        var client = (IWorldClient)sender!;
        var aisling = client.Aisling;
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var mapInstance = aisling?.MapInstance;

        await using var sync = mapInstance == null ? new NoOpDisposable() : await mapInstance.Sync.WaitAsync();

        try
        {
            //remove client from client list
            ClientRegistry.TryRemove(client.Id, out _);

            if (aisling != null)
            {
                //if the player has an exchange open, cancel it so items are returned
                var activeExchange = aisling.ActiveObject.TryGet<Exchange>();
                activeExchange?.Cancel(aisling);

                //remove aisling from map
                mapInstance?.RemoveObject(client.Aisling);
                //save aisling
                await SaveUserAsync(client.Aisling);
            }
        } catch (Exception ex)
        {
            Logger.LogError(ex, "Exception thrown while {@Client} was trying to disconnect", client);
        }
    }
    #endregion
}