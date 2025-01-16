#region
using System.Net;
using System.Net.Sockets;
using System.Text;
using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Collections.Common;
using Chaos.Common.Abstractions;
using Chaos.Common.Identity;
using Chaos.Common.Synchronization;
using Chaos.Cryptography;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities;
using Chaos.Networking.Entities.Client;
using Chaos.Networking.Entities.Server;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Other;
using Chaos.Services.Other.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Services.Servers;

public sealed class WorldServer : ServerBase<IChaosWorldClient>, IWorldServer<IChaosWorldClient>
{
    private readonly IAsyncStore<Aisling> AislingStore;
    private readonly BulletinBoardKeyMapper BulletinBoardKeyMapper;
    private readonly IStore<BulletinBoard> BulletinBoardStore;
    private readonly IChannelService ChannelService;
    private readonly IFactory<IChaosWorldClient> ClientFactory;
    private readonly ICommandInterceptor<Aisling> CommandInterceptor;
    private readonly IGroupService GroupService;
    private readonly IStore<MailBox> MailStore;
    private readonly IMerchantFactory MerchantFactory;
    private readonly IMetaDataStore MetaDataStore;
    private new WorldOptions Options { get; }

    public IEnumerable<Aisling> Aislings
        => ClientRegistry.Select(c => c.Aisling)
                         .Where(player => player != null!);

    public WorldServer(
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IFactory<IChaosWorldClient> clientFactory,
        IAsyncStore<Aisling> aislingStore,
        IRedirectManager redirectManager,
        IPacketSerializer packetSerializer,
        ICommandInterceptor<Aisling> commandInterceptor,
        IGroupService groupService,
        IMerchantFactory merchantFactory,
        IOptions<WorldOptions> options,
        ILogger<WorldServer> logger,
        IMetaDataStore metaDataStore,
        IChannelService channelService,
        IStore<MailBox> mailStore,
        BulletinBoardKeyMapper bulletinBoardKeyMapper,
        IStore<BulletinBoard> bulletinBoardStore)
        : base(
            redirectManager,
            packetSerializer,
            clientRegistry,
            options,
            logger)
    {
        Options = options.Value;
        ClientFactory = clientFactory;
        AislingStore = aislingStore;
        CommandInterceptor = commandInterceptor;
        GroupService = groupService;
        MerchantFactory = merchantFactory;
        MetaDataStore = metaDataStore;
        ChannelService = channelService;
        MailStore = mailStore;
        BulletinBoardKeyMapper = bulletinBoardKeyMapper;
        BulletinBoardStore = bulletinBoardStore;

        IndexHandlers();
    }

    #region Server Loop
    private Task SaveUserAsync(Aisling aisling) => AislingStore.SaveAsync(aisling);
    #endregion

    #region OnHandlers
    public ValueTask OnBeginChant(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<BeginChantArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnBeginChant);

        static ValueTask InnerOnBeginChant(IChaosWorldClient localClient, BeginChantArgs localArgs)
        {
            localClient.Aisling.UserState |= UserState.IsChanting;
            localClient.Aisling.ChantTimer.Start(localArgs.CastLineCount);

            return default;
        }
    }

    #region Board Request
    private bool TryGetBoard(IChaosWorldClient client, BoardInteractionArgs args, [MaybeNullWhen(false)] out BoardBase boardBase)
    {
        boardBase = null;

        switch (args.BoardId)
        {
            case null:
                break;
            case MailBox.BOARD_ID:
            {
                //if the "To" property is populated, we are sending mail to someone
                //we want to return their mailbox
                //otherwise, return our mailbox
                boardBase = !string.IsNullOrEmpty(args.To) ? MailStore.Load(args.To) : client.Aisling.MailBox;

                break;
            }
            default:
            {
                var key = BulletinBoardKeyMapper.GetKey(args.BoardId.Value);

                if (!string.IsNullOrEmpty(key))
                    boardBase = BulletinBoardStore.Load(key);

                break;
            }
        }

        if (boardBase is null)
        {
            Logger.WithTopics(
                      Topics.Entities.Aisling,
                      Topics.Entities.BulletinBoard,
                      Topics.Entities.MailBox,
                      Topics.Actions.Read)
                  .WithProperty(client)
                  .LogError("{@AislingName} requested an invalid board id: {@BoardId}", client.Aisling.Name, args.BoardId);

            return false;
        }

        return true;
    }
    #endregion

    public ValueTask OnBoardInteraction(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<BoardInteractionArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnBoardInteraction);

        ValueTask InnerOnBoardInteraction(IChaosWorldClient localClient, BoardInteractionArgs localArgs)
        {
            switch (localArgs.BoardRequestType)
            {
                case BoardRequestType.BoardList:
                {
                    var boards = localClient.Aisling.Script.GetBoardList();
                    localClient.SendBoardList(boards);

                    break;
                }
                case BoardRequestType.ViewBoard:
                {
                    if (!TryGetBoard(localClient, localArgs, out var board))
                        return default;

                    board.Show(localClient.Aisling, localArgs.StartPostId!.Value);

                    break;
                }
                case BoardRequestType.ViewPost:
                {
                    if (!TryGetBoard(localClient, localArgs, out var board))
                        return default;

                    board.ShowPost(localClient.Aisling, localArgs.PostId!.Value, localArgs.Controls!.Value);

                    break;
                }
                case BoardRequestType.NewPost:
                {
                    if (!TryGetBoard(localClient, localArgs, out var board))
                        return default;

                    //mailboxes use a different boardRequestType for sending mail
                    if (board is MailBox)
                    {
                        Logger.WithTopics(
                                  Topics.Entities.Aisling,
                                  Topics.Entities.BulletinBoard,
                                  Topics.Entities.MailBox,
                                  Topics.Actions.Read)
                              .WithProperty(client)
                              .LogError(
                                  "{@AislingName} requested an invalid board id for request type {@BoardRequestType}: {@BoardId}",
                                  client.Aisling.Name,
                                  localArgs.BoardRequestType,
                                  args.BoardId);

                        return default;
                    }

                    board.Post(
                        localClient.Aisling,
                        localClient.Aisling.Name,
                        localArgs.Subject!,
                        localArgs.Message!);

                    break;
                }
                case BoardRequestType.Delete:
                {
                    if (!TryGetBoard(localClient, localArgs, out var board))
                        return default;

                    board.Delete(localClient.Aisling, localArgs.PostId!.Value);

                    break;
                }
                case BoardRequestType.SendMail:
                {
                    if (!TryGetBoard(localClient, localArgs, out var board))
                        return default;

                    board.Post(
                        localClient.Aisling,
                        localClient.Aisling.Name,
                        localArgs.Subject!,
                        localArgs.Message!,
                        true);

                    break;
                }
                case BoardRequestType.Highlight:
                {
                    if (!TryGetBoard(localClient, localArgs, out var board))
                        return default;

                    //you cant highlight mail messages
                    if (board is MailBox)
                    {
                        Logger.WithTopics(Topics.Entities.MailBox, Topics.Entities.BulletinBoard, Topics.Actions.Highlight)
                              .WithProperty(client)
                              .LogError(
                                  "{@AislingName} requested an invalid board id for request type {@BoardRequestType}: {@BoardId}",
                                  client.Aisling.Name,
                                  localArgs.BoardRequestType,
                                  args.BoardId);

                        return default;
                    }

                    board.Highlight(localClient.Aisling, localArgs.PostId!.Value);

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return default;
        }
    }

    public ValueTask OnChant(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<ChantArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnChant);

        static ValueTask InnerOnChant(IChaosWorldClient localClient, ChantArgs localArgs)
        {
            var message = localArgs.ChantMessage;

            if (message.Length > CONSTANTS.MAX_MESSAGE_LINE_LENGTH)
                message = message[..CONSTANTS.MAX_MESSAGE_LINE_LENGTH];

            localClient.Aisling.Chant(message);

            return default;
        }
    }

    public ValueTask OnClick(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<ClickArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnClick);

        ValueTask InnerOnClick(IChaosWorldClient localClient, ClickArgs localArgs)
        {
            if (localArgs.TargetId.HasValue)
            {
                if (localArgs.TargetId == uint.MaxValue)
                {
                    var f1Merchant = MerchantFactory.Create(
                        Options.F1MerchantTemplateKey,
                        localClient.Aisling.MapInstance,
                        Point.From(localClient.Aisling));

                    f1Merchant.OnClicked(localClient.Aisling);

                    return default;
                }

                localClient.Aisling.MapInstance.Click(localArgs.TargetId.Value, localClient.Aisling);
            } else if (localArgs.TargetPoint is not null)
                localClient.Aisling.MapInstance.Click(localArgs.TargetPoint, localClient.Aisling);

            return default;
        }
    }

    public ValueTask OnClientRedirected(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<ClientRedirectedArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnClientRedirected);

        ValueTask InnerOnClientRedirected(IChaosWorldClient localClient, ClientRedirectedArgs localArgs)
        {
            if (!RedirectManager.TryGetRemove(localArgs.Id, out var redirect))
            {
                Logger.WithTopics(
                          Topics.Servers.WorldServer,
                          Topics.Entities.Client,
                          Topics.Actions.Redirect,
                          Topics.Qualifiers.Cheating)
                      .WithProperty(localArgs)
                      .LogWarning("{@ClientIp} tried to redirect to the world with invalid details", client.RemoteIp);

                localClient.Disconnect();

                return default;
            }

            //keep this case sensitive
            if (localArgs.Name != redirect.Name)
            {
                Logger.WithTopics(
                          Topics.Servers.WorldServer,
                          Topics.Entities.Client,
                          Topics.Actions.Redirect,
                          Topics.Qualifiers.Cheating)
                      .WithProperty(redirect)
                      .WithProperty(localArgs)
                      .LogWarning(
                          "{@ClientIp} tried to impersonate a redirect with redirect {@RedirectId}",
                          localClient.RemoteIp,
                          redirect.Id);

                localClient.Disconnect();

                return default;
            }

            Logger.WithTopics(Topics.Servers.WorldServer, Topics.Entities.Client, Topics.Actions.Redirect)
                  .WithProperty(localClient)
                  .WithProperty(redirect)
                  .LogDebug("Received world redirect {@RedirectId}", redirect.Id);

            var existingAisling = Aislings.FirstOrDefault(user => user.Name.EqualsI(redirect.Name));

            //double logon, disconnect both clients
            if (existingAisling != null)
            {
                Logger.WithTopics(
                          Topics.Servers.WorldServer,
                          Topics.Entities.Aisling,
                          Topics.Actions.Login,
                          Topics.Actions.Redirect)
                      .WithProperty(localClient)
                      .WithProperty(existingAisling)
                      .LogDebug("Duplicate login detected for aisling {@AislingName}, disconnecting both clients", existingAisling.Name);

                existingAisling.Client.Disconnect();
                localClient.Disconnect();

                return default;
            }

            return LoadAislingAsync(localClient, redirect);
        }
    }

    public async ValueTask LoadAislingAsync(IChaosWorldClient client, IRedirect redirect)
    {
        try
        {
            client.Crypto = new Crypto(redirect.Seed, redirect.Key, redirect.Name);

            var aisling = await AislingStore.LoadAsync(redirect.Name);

            client.Aisling = aisling;
            aisling.Client = client;

            await using var sync = await aisling.MapInstance.Sync.WaitAsync();

            try
            {
                aisling.Guild?.Associate(aisling);
                aisling.MailBox = MailStore.Load(aisling.Name);
                aisling.BeginObserving();
                client.SendAttributes(StatUpdateType.Full);
                client.SendLightLevel(aisling.MapInstance.CurrentLightLevel);
                client.SendUserId();
                aisling.MapInstance.AddAislingDirect(aisling, aisling);
                client.SendEditableProfileRequest();

                foreach (var channel in aisling.ChannelSettings.ToList())
                {
                    //try to join channel
                    //remove from channel if it fails
                    if (!ChannelService.JoinChannel(aisling, channel.ChannelName, true))
                    {
                        aisling.ChannelSettings.Remove(channel);

                        continue;
                    }

                    //set custom channel color if it exists
                    if (channel.MessageColor.HasValue)
                        ChannelService.SetChannelColor(aisling, channel.ChannelName, channel.MessageColor.Value);
                }

                client.ReceiveSync.Name = $"WorldClient {client.RemoteIp} {aisling.Name}";

                Logger.WithTopics(
                          Topics.Servers.WorldServer,
                          Topics.Entities.Aisling,
                          Topics.Actions.Redirect,
                          Topics.Actions.Login)
                      .WithProperty(client)
                      .LogInformation("World redirect finalized for {@AislingName}", aisling.Name);
            } catch (Exception e)
            {
                Logger.WithTopics(
                          Topics.Servers.WorldServer,
                          Topics.Entities.Aisling,
                          Topics.Actions.Redirect,
                          Topics.Actions.Login)
                      .WithProperty(aisling)
                      .LogError(e, "Failed to add aisling {@AislingName} to the world", aisling.Name);

                client.Disconnect();
            }
        } catch (Exception e)
        {
            Logger.WithTopics(
                      Topics.Servers.WorldServer,
                      Topics.Entities.Aisling,
                      Topics.Actions.Redirect,
                      Topics.Actions.Login)
                  .WithProperty(client)
                  .WithProperty(redirect)
                  .LogError(
                      e,
                      "Client with ip {@ClientIp} failed to load aisling {@AislingName}",
                      client.RemoteIp,
                      redirect.Name);

            client.Disconnect();
        }
    }

    public ValueTask OnClientWalk(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<ClientWalkArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnClientWalk);

        static ValueTask InnerOnClientWalk(IChaosWorldClient localClient, ClientWalkArgs localArgs)
        {
            //if player is in a world map, dont allow them to walk
            if (localClient.Aisling.ActiveObject.TryGet<WorldMap>() != null)
                return default;

            //TODO: should i refresh the client if the points don't match up? seems like it might get obnoxious

            localClient.Aisling.Walk(localArgs.Direction);

            return default;
        }
    }

    public ValueTask OnDialogInteraction(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<DialogInteractionArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnDialogInteraction);

        ValueTask InnerOnDialogInteraction(IChaosWorldClient localClient, DialogInteractionArgs localArgs)
        {
            var dialog = localClient.Aisling.ActiveDialog.Get();

            if (dialog == null)
            {
                localClient.Aisling.DialogHistory.Clear();

                Logger.WithTopics(Topics.Entities.Dialog, Topics.Actions.Read, Topics.Qualifiers.Cheating)
                      .WithProperty(localClient.Aisling)
                      .WithProperty(localArgs)
                      .LogWarning(
                          "Aisling {@AislingName} attempted to access a dialog, but there is no active dialog (possibly packeting)",
                          localClient.Aisling.Name);

                return default;
            }

            //since we always send a dialog id of 0, we can easily get the result without comparing ids
            var dialogResult = (DialogResult)localArgs.DialogId;

            if (localArgs.Args != null)
                dialog.MenuArgs = new ArgumentCollection(dialog.MenuArgs.Append(localArgs.Args.Last()));

            switch (dialogResult)
            {
                case DialogResult.Previous:
                    dialog.Previous(localClient.Aisling);

                    break;
                case DialogResult.Close:
                    localClient.Aisling.DialogHistory.Clear();

                    break;
                case DialogResult.Next:
                    dialog.Next(localClient.Aisling, localArgs.Option);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return default;
        }
    }

    public ValueTask OnEmote(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<EmoteArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnEmote);

        ValueTask InnerOnEmote(IChaosWorldClient localClient, EmoteArgs localArgs)
        {
            if ((int)localArgs.BodyAnimation <= 44)
                client.Aisling.AnimateBody(localArgs.BodyAnimation, 100);

            return default;
        }
    }

    public ValueTask OnDisplayEntityRequest(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<DisplayEntityRequestArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnDisplayEntityRequest);

        ValueTask InnerOnDisplayEntityRequest(IChaosWorldClient localClient, DisplayEntityRequestArgs localArgs)
        {
            var aisling = localClient.Aisling;
            var mapInstance = aisling.MapInstance;

            if (mapInstance.TryGetEntity<VisibleEntity>(localArgs.TargetId, out var obj) && !aisling.CanObserve(obj))
                Logger.WithTopics(Topics.Entities.Aisling, Topics.Qualifiers.Forced)
                      .WithProperty(aisling)
                      .WithProperty(obj)
                      .LogTrace(
                          "Aisling {@AislingName} attempted to forcefully display an entity {@EntityId} that they cannot observe. (Unknown why this happens)",
                          aisling.Name,
                          obj.Id);

            return default;
        }
    }

    public ValueTask OnExchangeInteraction(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<ExchangeInteractionArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnExchangeInteraction);

        ValueTask InnerOnExchangeInteraction(IChaosWorldClient localClient, ExchangeInteractionArgs localArgs)
        {
            var exchange = localClient.Aisling.ActiveObject.TryGet<Exchange>();

            if (exchange == null)
                return default;

            if (exchange.GetOther(localClient.Aisling)
                        .Id
                != localArgs.OtherPlayerId)
                return default;

            switch (localArgs.ExchangeRequestType)
            {
                case ExchangeRequestType.StartExchange:
                    Logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Exchange,
                              Topics.Actions.Create,
                              Topics.Qualifiers.Cheating)
                          .WithProperty(localClient)
                          .LogWarning(
                              "Aisling {@AislingName} attempted to directly start an exchange. This should not be possible unless packeting",
                              localClient.Aisling.Name);

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
    }

    public ValueTask OnExitRequest(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<ExitRequestArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnExitRequest);

        ValueTask InnerOnExitRequest(IChaosWorldClient localClient, ExitRequestArgs localArgs)
        {
            if (localArgs.IsRequest)
                localClient.SendExitResponse();
            else
            {
                var redirect = new Redirect(
                    EphemeralRandomIdGenerator<uint>.Shared.NextId,
                    Options.LoginRedirect,
                    ServerType.Login,
                    Encoding.ASCII.GetString(localClient.Crypto.Key),
                    localClient.Crypto.Seed);

                RedirectManager.Add(redirect);

                Logger.WithTopics(
                          Topics.Servers.WorldServer,
                          Topics.Entities.Aisling,
                          Topics.Actions.Logout,
                          Topics.Actions.Redirect)
                      .WithProperty(localClient)
                      .LogDebug("Redirecting {@ClientIp} to {@ServerIp}", client.RemoteIp, Options.LoginRedirect.Address.ToString());

                localClient.SendRedirect(redirect);
            }

            return default;
        }
    }

    public ValueTask OnGoldDrop(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<GoldDropArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnGoldDrop);

        ValueTask InnerOnGoldDrop(IChaosWorldClient localClient, GoldDropArgs localArgs)
        {
            var map = localClient.Aisling.MapInstance;

            if (!localClient.Aisling.WithinRange(localArgs.DestinationPoint, Options.DropRange))
                return default;

            if (map.IsWall(localArgs.DestinationPoint))
                return default;

            localClient.Aisling.TryDropGold(localArgs.DestinationPoint, localArgs.Amount, out _);

            return default;
        }
    }

    public ValueTask OnGoldDroppedOnCreature(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<GoldDroppedOnCreatureArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnGoldDroppedOnCreature);

        ValueTask InnerOnGoldDroppedOnCreature(IChaosWorldClient localClient, GoldDroppedOnCreatureArgs localArgs)
        {
            var map = localClient.Aisling.MapInstance;

            if (localArgs.Amount <= 0)
                return default;

            if (!map.TryGetEntity<Creature>(localArgs.TargetId, out var target))
                return default;

            if (!localClient.Aisling.WithinRange(target, Options.TradeRange))
                return default;

            target.OnGoldDroppedOn(localClient.Aisling, localArgs.Amount);

            return default;
        }
    }

    public ValueTask OnGroupInvite(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<GroupInviteArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnGroupInvite);

        ValueTask InnerOnGroupInvite(IChaosWorldClient localClient, GroupInviteArgs localArgs)
        {
            localArgs.TargetName = localArgs.TargetName.ReplaceI("_", " ");

            var target = Aislings.FirstOrDefault(user => user.Name.EqualsI(localArgs.TargetName));

            if (target == null)
            {
                localClient.Aisling.SendActiveMessage($"{localArgs.TargetName} is nowhere to be found");

                return default;
            }

            var aisling = localClient.Aisling;

            switch (localArgs.ClientGroupSwitch)
            {
                case ClientGroupSwitch.FormalInvite:
                    Logger.WithTopics(
                              Topics.Entities.Aisling,
                              Topics.Entities.Group,
                              Topics.Actions.Invite,
                              Topics.Qualifiers.Cheating)
                          .WithProperty(aisling)
                          .LogWarning(
                              "Aisling {@AislingName} attempted to send a formal group invite to the server. This type of group request is something only the server should send",
                              localClient);

                    return default;
                case ClientGroupSwitch.TryInvite:
                {
                    GroupService.Invite(aisling, target);

                    return default;
                }
                case ClientGroupSwitch.AcceptInvite:
                {
                    var type = GroupService.DetermineRequestType(target, aisling);

                    switch (type)
                    {
                        case IGroupService.RequestType.Invite:
                            GroupService.AcceptInvite(target, aisling);

                            break;
                        case IGroupService.RequestType.RequestToJoin:
                            GroupService.AcceptRequestToJoin(target, aisling);

                            break;
                        default:
                            Logger.WithTopics(
                                      Topics.Entities.Aisling,
                                      Topics.Entities.Group,
                                      Topics.Actions.Invite,
                                      Topics.Qualifiers.Cheating)
                                  .LogWarning(
                                      "{@Aisling} attempted to accept an invite from {@Target}, but there was no invite or request",
                                      aisling,
                                      target);

                            break;
                    }

                    return default;
                }
                case ClientGroupSwitch.CreateGroupbox:
                    client.Aisling.GroupBox = new GroupBox
                    {
                        Name = localArgs.GroupBoxInfo!.Name,
                        Note = localArgs.GroupBoxInfo.Note,
                        MinLevel = localArgs.GroupBoxInfo.MinLevel,
                        MaxLevel = localArgs.GroupBoxInfo.MaxLevel,
                        MaxWarriors = localArgs.GroupBoxInfo.MaxWarriors,
                        MaxWizards = localArgs.GroupBoxInfo.MaxWizards,
                        MaxMonks = localArgs.GroupBoxInfo.MaxMonks,
                        MaxPriests = localArgs.GroupBoxInfo.MaxPriests,
                        MaxRogues = localArgs.GroupBoxInfo.MaxRogues
                    };

                    client.Aisling.Display();

                    return default;
                case ClientGroupSwitch.ViewGroupBox:
                    if (target.GroupBox is null)
                    {
                        Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Group, Topics.Qualifiers.Cheating)
                              .LogWarning(
                                  "{@Aisling} attempted to view {@Target}'s group box, but they do not have one",
                                  localClient.Aisling,
                                  target);

                        return default;
                    }

                    var classCounts = new byte[6];

                    if (target.Group is not null)
                        foreach (var member in target.Group)
                        {
                            if (member.UserStatSheet.BaseClass == BaseClass.Diacht)
                                continue;

                            classCounts[(int)member.UserStatSheet.BaseClass]++;
                        }
                    else if (target.UserStatSheet.BaseClass != BaseClass.Diacht)
                        classCounts[(int)target.UserStatSheet.BaseClass]++;

                    var groupBoxInfo = new DisplayGroupBoxInfo
                    {
                        Name = target.GroupBox.Name,
                        Note = target.GroupBox.Note,
                        MinLevel = target.GroupBox.MinLevel,
                        MaxLevel = target.GroupBox.MaxLevel,
                        MaxWarriors = target.GroupBox.MaxWarriors,
                        CurrentWarriors = classCounts[(int)BaseClass.Warrior],
                        MaxWizards = target.GroupBox.MaxWizards,
                        CurrentWizards = classCounts[(int)BaseClass.Wizard],
                        MaxRogues = target.GroupBox.MaxRogues,
                        CurrentRogues = classCounts[(int)BaseClass.Rogue],
                        MaxPriests = target.GroupBox.MaxPriests,
                        CurrentPriests = classCounts[(int)BaseClass.Priest],
                        MaxMonks = target.GroupBox.MaxMonks,
                        CurrentMonks = classCounts[(int)BaseClass.Monk]
                    };

                    client.SendDisplayGroupInvite(ServerGroupSwitch.ShowGroupBox, args.TargetName, groupBoxInfo);

                    return default;
                case ClientGroupSwitch.RemoveGroupBox:
                    client.Aisling.GroupBox = null;

                    return default;
                case ClientGroupSwitch.RequestToJoin:
                    if (target.GroupBox is null)
                    {
                        Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Group, Topics.Qualifiers.Cheating)
                              .LogWarning(
                                  "{@Aisling} attempted to interact with {@Target}'s group box, but they do not have one",
                                  localClient.Aisling,
                                  target);

                        return default;
                    }

                    GroupService.RequestToJoin(localClient.Aisling, target);

                    return default;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public ValueTask OnIgnore(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<IgnoreArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnIgnore);

        static ValueTask InnerOnIgnore(IChaosWorldClient localClient, IgnoreArgs localArgs)
        {
            switch (localArgs.IgnoreType)
            {
                case IgnoreType.Request:
                    localClient.SendServerMessage(ServerMessageType.ScrollWindow, localClient.Aisling.IgnoreList.ToString());

                    break;
                case IgnoreType.AddUser:
                    if (!string.IsNullOrEmpty(localArgs.TargetName))
                        localClient.Aisling.IgnoreList.Add(localArgs.TargetName);

                    break;
                case IgnoreType.RemoveUser:
                    if (!string.IsNullOrEmpty(localArgs.TargetName))
                        localClient.Aisling.IgnoreList.Remove(localArgs.TargetName);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return default;
        }
    }

    public ValueTask OnItemDrop(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<ItemDropArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnItemDrop);

        static ValueTask InnerOnItemDrop(IChaosWorldClient localClient, ItemDropArgs localArgs)
        {
            localClient.Aisling.TryDrop(
                localArgs.DestinationPoint,
                localArgs.SourceSlot,
                out _,
                localArgs.Count);

            return default;
        }
    }

    public ValueTask OnItemDroppedOnCreature(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<ItemDroppedOnCreatureArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnItemDroppedOnCreature);

        ValueTask InnerOnItemDroppedOnCreature(IChaosWorldClient localClient, ItemDroppedOnCreatureArgs localArgs)
        {
            var map = localClient.Aisling.MapInstance;

            if (!map.TryGetEntity<Creature>(localArgs.TargetId, out var target))
                return default;

            if (!localClient.Aisling.WithinRange(target, Options.TradeRange))
                return default;

            if (!localClient.Aisling.Inventory.TryGetObject(localArgs.SourceSlot, out var item))
                return default;

            if (item.Count < localArgs.Count)
                return default;

            target.OnItemDroppedOn(localClient.Aisling, localArgs.SourceSlot, localArgs.Count);

            return default;
        }
    }

    public ValueTask OnMapDataRequest(IChaosWorldClient client, in Packet packet)
    {
        return ExecuteHandler(client, InnerOnMapDataRequest);

        static ValueTask InnerOnMapDataRequest(IChaosWorldClient localClient)
        {
            localClient.SendMapData();

            return default;
        }
    }

    public ValueTask OnMetaDataRequest(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<MetaDataRequestArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnMetaDataRequest);

        ValueTask InnerOnMetaDataRequest(IChaosWorldClient localClient, MetaDataRequestArgs localArgs)
        {
            switch (localArgs.MetaDataRequestType)
            {
                case MetaDataRequestType.DataByName:
                    localClient.SendMetaData(MetaDataRequestType.DataByName, MetaDataStore, localArgs.Name);

                    break;
                case MetaDataRequestType.AllCheckSums:
                    localClient.SendMetaData(MetaDataRequestType.AllCheckSums, MetaDataStore);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return default;
        }
    }

    public ValueTask OnPickup(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<PickupArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnPickup);

        ValueTask InnerOnPickup(IChaosWorldClient localClient, PickupArgs localArgs)
        {
            var map = localClient.Aisling.MapInstance;

            if (!localClient.Aisling.WithinRange(localArgs.SourcePoint, Options.PickupRange))
                return default;

            var possibleObjs = map.GetEntitiesAtPoints<GroundEntity>(localArgs.SourcePoint)
                                  .OrderByDescending(obj => obj.Creation)
                                  .ToList();

            if (possibleObjs.Count == 0)
                return default;

            //loop through the items on the ground, try to pick each one up
            //if we pick one up, return (only pick up 1 obj at a time)
            foreach (var obj in possibleObjs)
                switch (obj)
                {
                    case GroundItem groundItem:
                        if (localClient.Aisling.TryPickupItem(groundItem, localArgs.DestinationSlot))
                            return default;

                        break;
                    case Money money:
                        if (localClient.Aisling.TryPickupMoney(money))
                            return default;

                        break;
                }

            return default;
        }
    }

    public ValueTask OnEditableProfile(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<EditableProfileArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnEditableProfile);

        static ValueTask InnerOnEditableProfile(IChaosWorldClient localClient, EditableProfileArgs localArgs)
        {
            localClient.Aisling.Portrait = localArgs.PortraitData;
            localClient.Aisling.ProfileText = localArgs.ProfileMessage;

            return default;
        }
    }

    public ValueTask OnSelfProfileRequest(IChaosWorldClient client, in Packet packet)
    {
        return ExecuteHandler(client, InnerOnProfileRequest);

        static ValueTask InnerOnProfileRequest(IChaosWorldClient localClient)
        {
            localClient.SendSelfProfile();

            return default;
        }
    }

    public ValueTask OnPublicMessage(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<PublicMessageArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnPublicMessage);

        async ValueTask InnerOnPublicMessage(IChaosWorldClient localClient, PublicMessageArgs localArgs)
        {
            if (CommandInterceptor.IsCommand(localArgs.Message))
            {
                Logger.WithTopics(
                          Topics.Entities.Aisling,
                          Topics.Entities.Message,
                          Topics.Actions.Send,
                          Topics.Entities.Command,
                          Topics.Actions.Execute)
                      .WithProperty(localClient)
                      .WithProperty(localClient.Aisling)
                      .LogDebug("Aisling {@AislingName} sent command {@Command}", localClient.Aisling.Name, localArgs.Message);

                await CommandInterceptor.HandleCommandAsync(localClient.Aisling, localArgs.Message);

                return;
            }

            localClient.Aisling.ShowPublicMessage(localArgs.PublicMessageType, localArgs.Message);
        }
    }

    public ValueTask OnMenuInteraction(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<MenuInteractionArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnMenuInteraction);

        ValueTask InnerOnMenuInteraction(IChaosWorldClient localClient, MenuInteractionArgs localArgs)
        {
            var dialog = localClient.Aisling.ActiveDialog.Get();

            if (dialog == null)
            {
                Logger.WithTopics(
                          Topics.Entities.Aisling,
                          Topics.Entities.Dialog,
                          Topics.Actions.Read,
                          Topics.Qualifiers.Cheating)
                      .WithProperty(localClient.Aisling)
                      .WithProperty(localArgs)
                      .LogWarning(
                          "Aisling {@AislingName} attempted to access a dialog, but there is no active dialog (possibly packeting)",
                          localClient.Aisling.Name);

                return default;
            }

            //get args if the type is not a "menuWithArgs", this type should not have any new args
            if (dialog.Type is not ChaosDialogType.MenuWithArgs)
            {
                if (localArgs.Args != null)
                    dialog.MenuArgs = new ArgumentCollection(dialog.MenuArgs.Append(localArgs.Args.Last()));

                //handle SlotOrLength as an arg (since it is an arg)
                if (localArgs.Slot.HasValue)
                    dialog.MenuArgs.Add(localArgs.Slot.Value.ToString());
            }

            dialog.Next(localClient.Aisling, (byte)localArgs.PursuitId);

            return default;
        }
    }

    public ValueTask OnRaiseStat(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<RaiseStatArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnRaiseStat);

        static ValueTask InnerOnRaiseStat(IChaosWorldClient localClient, RaiseStatArgs localArgs)
        {
            if (localClient.Aisling.UserStatSheet.UnspentPoints > 0)
                if (localClient.Aisling.UserStatSheet.IncrementStat(localArgs.Stat))
                {
                    localClient.Aisling.Script.OnStatIncrease(localArgs.Stat);
                    localClient.SendAttributes(StatUpdateType.Full);
                }

            return default;
        }
    }

    public ValueTask OnRefreshRequest(IChaosWorldClient client, in Packet packet)
    {
        return ExecuteHandler(client, InnerOnRefreshRequest);

        static ValueTask InnerOnRefreshRequest(IChaosWorldClient localClient)
        {
            localClient.Aisling.Refresh();

            return default;
        }
    }

    public ValueTask OnSocialStatus(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<SocialStatusArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnSocialStatus);

        static ValueTask InnerOnSocialStatus(IChaosWorldClient localClient, SocialStatusArgs localArgs)
        {
            localClient.Aisling.Options.SocialStatus = localArgs.SocialStatus;

            return default;
        }
    }

    public ValueTask OnSpacebar(IChaosWorldClient client, in Packet packet)
    {
        return ExecuteHandler(client, InnerOnSpacebar);

        static ValueTask InnerOnSpacebar(IChaosWorldClient localClient)
        {
            localClient.SendCancelCasting();

            foreach (var skill in localClient.Aisling.SkillBook)
                if (skill.Template.IsAssail)
                    localClient.Aisling.TryUseSkill(skill);

            return default;
        }
    }

    public ValueTask OnSwapSlot(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<SwapSlotArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnSwapSlot);

        static ValueTask InnerOnSwapSlot(IChaosWorldClient localClient, SwapSlotArgs localArgs)
        {
            switch (localArgs.PanelType)
            {
                case PanelType.Inventory:
                    localClient.Aisling.Inventory.TrySwap(localArgs.Slot1, localArgs.Slot2);

                    break;
                case PanelType.SpellBook:
                    localClient.Aisling.SpellBook.TrySwap(localArgs.Slot1, localArgs.Slot2);

                    break;
                case PanelType.SkillBook:
                    localClient.Aisling.SkillBook.TrySwap(localArgs.Slot1, localArgs.Slot2);

                    break;
                case PanelType.Equipment:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return default;
        }
    }

    public ValueTask OnToggleGroup(IChaosWorldClient client, in Packet packet)
    {
        return ExecuteHandler(client, InnerOnToggleGroup);

        static ValueTask InnerOnToggleGroup(IChaosWorldClient localClient)
        {
            //don't need to send the updated option, because they arent currently looking at it
            localClient.Aisling.Options.ToggleGroup();

            if (localClient.Aisling.Group != null)
                localClient.Aisling.Group?.Leave(localClient.Aisling);
            else
                localClient.SendSelfProfile();

            return default;
        }
    }

    public ValueTask OnTurn(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<TurnArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnTurn);

        static ValueTask InnerOnTurn(IChaosWorldClient localClient, TurnArgs localArgs)
        {
            localClient.Aisling.Turn(localArgs.Direction);

            return default;
        }
    }

    public ValueTask OnUnequip(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<UnequipArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnUnequip);

        static ValueTask InnerOnUnequip(IChaosWorldClient localClient, UnequipArgs localArgs)
        {
            localClient.Aisling.UnEquip(localArgs.EquipmentSlot);

            return default;
        }
    }

    public ValueTask OnItemUse(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<ItemUseArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnItemUse);

        static ValueTask InnerOnItemUse(IChaosWorldClient localClient, ItemUseArgs localArgs)
        {
            var exchange = localClient.Aisling.ActiveObject.TryGet<Exchange>();

            if (exchange != null)
            {
                exchange.AddItem(localClient.Aisling, localArgs.SourceSlot);

                return default;
            }

            localClient.Aisling.TryUseItem(localArgs.SourceSlot);

            return default;
        }
    }

    public ValueTask OnOptionToggle(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<OptionToggleArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnOptionToggle);

        static ValueTask InnerOnOptionToggle(IChaosWorldClient localClient, OptionToggleArgs localArgs)
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
    }

    public ValueTask OnSkillUse(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<SkillUseArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnSkillUse);

        static ValueTask InnerOnSkillUse(IChaosWorldClient localClient, SkillUseArgs localArgs)
        {
            localClient.Aisling.TryUseSkill(localArgs.SourceSlot);

            return default;
        }
    }

    public ValueTask OnSpellUse(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<SpellUseArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnSpellUse);

        ValueTask InnerOnSpellUse(IChaosWorldClient localClient, SpellUseArgs localArgs)
        {
            if (localClient.Aisling.SpellBook.TryGetObject(localArgs.SourceSlot, out var spell))
            {
                Creature source = localClient.Aisling;
                var promptResponse = default(string?);
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
                        promptResponse = PacketSerializer.Encoding.GetString(localArgs.ArgsData);

                        break;
                    case SpellType.Targeted:
                        var targetIdSegment = new ArraySegment<byte>(localArgs.ArgsData, 0, 4);
                        var targetPointSegment = new ArraySegment<byte>(localArgs.ArgsData, 4, 4);

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

                localClient.Aisling.TryUseSpell(spell, targetId, promptResponse);
            }

            localClient.Aisling.UserState &= ~UserState.IsChanting;

            return default;
        }
    }

    public ValueTask OnWhisper(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<WhisperArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnWhisper);

        ValueTask InnerOnWhisper(IChaosWorldClient localClient, WhisperArgs localArgs)
        {
            var fromAisling = localClient.Aisling;

            if (localArgs.Message.Length > 100)
                return default;

            if (ChannelService.IsChannel(localArgs.TargetName))
            {
                if (localArgs.TargetName.EqualsI(WorldOptions.Instance.GroupChatName) || localArgs.TargetName.EqualsI("!group"))
                {
                    if (fromAisling.Group == null)
                    {
                        fromAisling.SendOrangeBarMessage("You are not in a group");

                        return default;
                    }

                    fromAisling.Group.SendMessage(fromAisling, localArgs.Message);
                } else if (localArgs.TargetName.EqualsI(WorldOptions.Instance.GuildChatName) || localArgs.TargetName.EqualsI("!guild"))
                {
                    if (fromAisling.Guild == null)
                    {
                        fromAisling.SendOrangeBarMessage("You are not in a guild");

                        return default;
                    }

                    fromAisling.Guild.SendMessage(fromAisling, localArgs.Message);
                } else if (ChannelService.ContainsChannel(localArgs.TargetName))
                    ChannelService.SendMessage(fromAisling, localArgs.TargetName, localArgs.Message);

                return default;
            }

            var targetAisling = Aislings.FirstOrDefault(player => player.Name.EqualsI(localArgs.TargetName));

            if (targetAisling == null)
            {
                fromAisling.SendActiveMessage($"{localArgs.TargetName} is not online");

                return default;
            }

            if (targetAisling.Equals(fromAisling))
            {
                localClient.SendServerMessage(ServerMessageType.Whisper, "Talking to yourself?");

                return default;
            }

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (targetAisling.Options.SocialStatus == SocialStatus.DoNotDisturb)
            {
                localClient.SendServerMessage(
                    ServerMessageType.Whisper,
                    $"{MessageColor.SpanishGray.ToPrefix()}{targetAisling.Name} doesn't want to be bothered");

                return default;
            }

            if (targetAisling.Options.SocialStatus == SocialStatus.DayDreaming)
                localClient.SendServerMessage(
                    ServerMessageType.Whisper,
                    $"{MessageColor.SpanishGray.ToPrefix()}{targetAisling.Name} is daydreaming");

            var maxLength = CONSTANTS.MAX_COMPLETE_MESSAGE_LENGTH - targetAisling.Name.Length - 4;

            if (localArgs.Message.Length > maxLength)
                localArgs.Message = localArgs.Message[..maxLength];

            localClient.SendServerMessage(ServerMessageType.Whisper, $"[{targetAisling.Name}]> {localArgs.Message}");

            //if someone is being ignored, they shouldnt know it
            //let them waste their time typing for no reason
            if (targetAisling.IgnoreList.ContainsI(fromAisling.Name))
            {
                Logger.WithTopics(
                          Topics.Entities.Aisling,
                          Topics.Entities.Message,
                          Topics.Actions.Send,
                          Topics.Qualifiers.Harassment)
                      .WithProperty(fromAisling)
                      .WithProperty(targetAisling)
                      .LogWarning(
                          "Aisling {@FromAislingName} sent whisper {@Message} to aisling {@TargetAislingName}, but they are being ignored (possibly harassment)",
                          fromAisling.Name,
                          localArgs.Message,
                          targetAisling.Name);

                return default;
            }

            Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Message, Topics.Actions.Send)
                  .WithProperty(fromAisling)
                  .WithProperty(targetAisling)
                  .LogInformation(
                      "Aisling {@FromAislingName} sent whisper {@Message} to aisling {@TargetAislingName}",
                      fromAisling.Name,
                      localArgs.Message,
                      targetAisling.Name);

            targetAisling.Client.SendServerMessage(ServerMessageType.Whisper, $"[{fromAisling.Name}]: {localArgs.Message}");

            return default;
        }
    }

    public ValueTask OnWorldListRequest(IChaosWorldClient client, in Packet packet)
    {
        return ExecuteHandler(client, InnerOnWorldListRequest);

        ValueTask InnerOnWorldListRequest(IChaosWorldClient localClient)
        {
            localClient.SendWorldList(Aislings.ToList());

            return default;
        }
    }

    public ValueTask OnWorldMapClick(IChaosWorldClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<WorldMapClickArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnWorldMapClick);

        static ValueTask InnerOnWorldMapClick(IChaosWorldClient localClient, WorldMapClickArgs localArgs)
        {
            var worldMap = localClient.Aisling.ActiveObject.TryGet<WorldMap>();

            //if player is not in a world map, return
            if (worldMap == null)
                return default;

            if (!worldMap.Nodes.TryGetValue(localArgs.CheckSum, out var node))
                return default;

            node.OnClick(localClient.Aisling);

            return default;
        }
    }
    #endregion

    #region Connection / Handler
    public override async ValueTask ExecuteHandler<TArgs>(
        IChaosWorldClient client,
        TArgs args,
        Func<IChaosWorldClient, TArgs, ValueTask> action)
    {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var mapInstance = client.Aisling?.MapInstance;
        IPolyDisposable disposable;

        //if map instance is null, use server synchronization
        if (mapInstance == null)
            disposable = await Sync.WaitAsync();
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
            Logger.WithTopics(
                      Topics.Servers.WorldServer,
                      Topics.Entities.Client,
                      Topics.Entities.Packet,
                      Topics.Actions.Processing)
                  .WithProperty(client)
                  .WithProperty(args!)
                  .LogError(
                      e,
                      "{@ClientType} failed to execute inner handler with args type {@ArgsType}",
                      client.GetType()
                            .Name,
                      args!.GetType()
                           .Name);
        }
    }

    public override async ValueTask ExecuteHandler(IChaosWorldClient client, Func<IChaosWorldClient, ValueTask> action)
    {
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var mapInstance = client.Aisling?.MapInstance;
        IPolyDisposable disposable;

        if (mapInstance == null)
            disposable = await Sync.WaitAsync();
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
            Logger.WithTopics(
                      Topics.Servers.WorldServer,
                      Topics.Entities.Client,
                      Topics.Entities.Packet,
                      Topics.Actions.Processing)
                  .WithProperty(client)
                  .LogError(
                      e,
                      "{@ClientType} failed to execute inner handler",
                      client.GetType()
                            .Name);
        }
    }

    public override ValueTask HandlePacketAsync(IChaosWorldClient client, in Packet packet)
    {
        var opCode = packet.OpCode;
        var handler = ClientHandlers[opCode];

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var trackers = client.Aisling?.Trackers;

        if (handler is not null)
            Logger.WithTopics(Topics.Servers.WorldServer, Topics.Entities.Packet, Topics.Actions.Processing)
                  .WithProperty(client)
                  .LogTrace("Processing message with code {@OpCode} from {@ClientIp}", opCode, client.RemoteIp);
        else
            Logger.WithTopics(
                      Topics.Servers.WorldServer,
                      Topics.Entities.Packet,
                      Topics.Actions.Processing,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(client)
                  .WithProperty(packet.ToString(), "HexData")
                  .LogWarning("Unknown message with code {@OpCode} from {@ClientIp}", opCode, client.RemoteIp);

        if ((trackers != null) && IsManualAction((ClientOpCode)packet.OpCode))
            trackers.LastManualAction = DateTime.UtcNow;

        return handler?.Invoke(client, in packet) ?? default;
    }

    protected override void IndexHandlers()
    {
        if (ClientHandlers == null!)
            return;

        base.IndexHandlers();

        //ClientHandlers[(byte)ClientOpCode.] =
        ClientHandlers[(byte)ClientOpCode.MapDataRequest] = OnMapDataRequest;
        ClientHandlers[(byte)ClientOpCode.ClientWalk] = OnClientWalk;
        ClientHandlers[(byte)ClientOpCode.Pickup] = OnPickup;
        ClientHandlers[(byte)ClientOpCode.ItemDrop] = OnItemDrop;
        ClientHandlers[(byte)ClientOpCode.ExitRequest] = OnExitRequest;
        ClientHandlers[(byte)ClientOpCode.DisplayEntityRequest] = OnDisplayEntityRequest;
        ClientHandlers[(byte)ClientOpCode.Ignore] = OnIgnore;
        ClientHandlers[(byte)ClientOpCode.PublicMessage] = OnPublicMessage;
        ClientHandlers[(byte)ClientOpCode.SpellUse] = OnSpellUse;
        ClientHandlers[(byte)ClientOpCode.ClientRedirected] = OnClientRedirected;
        ClientHandlers[(byte)ClientOpCode.Turn] = OnTurn;
        ClientHandlers[(byte)ClientOpCode.Spacebar] = OnSpacebar;
        ClientHandlers[(byte)ClientOpCode.WorldListRequest] = OnWorldListRequest;
        ClientHandlers[(byte)ClientOpCode.Whisper] = OnWhisper;
        ClientHandlers[(byte)ClientOpCode.OptionToggle] = OnOptionToggle;
        ClientHandlers[(byte)ClientOpCode.ItemUse] = OnItemUse;
        ClientHandlers[(byte)ClientOpCode.Emote] = OnEmote;
        ClientHandlers[(byte)ClientOpCode.GoldDrop] = OnGoldDrop;
        ClientHandlers[(byte)ClientOpCode.ItemDroppedOnCreature] = OnItemDroppedOnCreature;
        ClientHandlers[(byte)ClientOpCode.GoldDroppedOnCreature] = OnGoldDroppedOnCreature;
        ClientHandlers[(byte)ClientOpCode.SelfProfileRequest] = OnSelfProfileRequest;
        ClientHandlers[(byte)ClientOpCode.GroupInvite] = OnGroupInvite;
        ClientHandlers[(byte)ClientOpCode.ToggleGroup] = OnToggleGroup;
        ClientHandlers[(byte)ClientOpCode.SwapSlot] = OnSwapSlot;
        ClientHandlers[(byte)ClientOpCode.RefreshRequest] = OnRefreshRequest;
        ClientHandlers[(byte)ClientOpCode.MenuInteraction] = OnMenuInteraction;
        ClientHandlers[(byte)ClientOpCode.DialogInteraction] = OnDialogInteraction;
        ClientHandlers[(byte)ClientOpCode.BoardInteraction] = OnBoardInteraction;
        ClientHandlers[(byte)ClientOpCode.SkillUse] = OnSkillUse;
        ClientHandlers[(byte)ClientOpCode.WorldMapClick] = OnWorldMapClick;
        ClientHandlers[(byte)ClientOpCode.Click] = OnClick;
        ClientHandlers[(byte)ClientOpCode.Unequip] = OnUnequip;
        ClientHandlers[(byte)ClientOpCode.RaiseStat] = OnRaiseStat;
        ClientHandlers[(byte)ClientOpCode.ExchangeInteraction] = OnExchangeInteraction;
        ClientHandlers[(byte)ClientOpCode.BeginChant] = OnBeginChant;
        ClientHandlers[(byte)ClientOpCode.Chant] = OnChant;
        ClientHandlers[(byte)ClientOpCode.EditableProfile] = OnEditableProfile;
        ClientHandlers[(byte)ClientOpCode.SocialStatus] = OnSocialStatus;
        ClientHandlers[(byte)ClientOpCode.MetaDataRequest] = OnMetaDataRequest;
    }

    protected override void OnConnected(Socket clientSocket)
    {
        var ip = clientSocket.RemoteEndPoint as IPEndPoint;

        Logger.WithTopics(Topics.Servers.WorldServer, Topics.Entities.Client, Topics.Actions.Connect)
              .LogDebug("Incoming connection from {@ClientIp}", ip!.Address);

        var client = ClientFactory.Create(clientSocket);
        client.OnDisconnected += OnDisconnect;

        Logger.WithTopics(Topics.Servers.WorldServer, Topics.Entities.Client, Topics.Actions.Connect)
              .WithProperty(client)
              .LogInformation("Connection established with {@ClientIp}", client.RemoteIp);

        if (!ClientRegistry.TryAdd(client))
        {
            Logger.WithTopics(Topics.Servers.WorldServer, Topics.Entities.Client, Topics.Actions.Connect)
                  .WithProperty(client)
                  .LogError("Somehow two clients got the same id");

            client.Disconnect();
            clientSocket.Disconnect(false);

            return;
        }

        client.BeginReceive();
    }

    private async void OnDisconnect(object? sender, EventArgs e)
    {
        //we dont need to Task.Run this because it's async void
        //when async void reaches async code, control returns to the caller and the method is not awaited

        var client = (IChaosWorldClient)sender!;
        var aisling = client.Aisling;

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var mapInstance = aisling?.MapInstance;

        await using var sync = mapInstance == null ? new NoOpDisposable() : await mapInstance.Sync.WaitAsync();

        try
        {
            Logger.WithTopics(
                      Topics.Servers.WorldServer,
                      Topics.Entities.Client,
                      Topics.Entities.Aisling,
                      Topics.Actions.Disconnect)
                  .WithProperty(client)
                  .LogInformation("Aisling {@AislingName} has disconnected", aisling?.Name ?? "N/A");

            //remove client from client list
            ClientRegistry.TryRemove(client.Id, out _);

            if (aisling != null)
            {
                //if the player has an exchange open, cancel it so items are returned
                var activeExchange = aisling.ActiveObject.TryGet<Exchange>();

                try
                {
                    activeExchange?.Cancel(aisling);
                } catch (Exception ex)
                {
                    Logger.WithTopics(
                              Topics.Servers.WorldServer,
                              Topics.Entities.Client,
                              Topics.Entities.Aisling,
                              Topics.Actions.Disconnect)
                          .WithProperty(client)
                          .LogError(
                              ex,
                              "{@Aisling} failed to cancel exchange {@Exchange} on disconnect",
                              aisling,
                              activeExchange);
                }

                try
                {
                    //leave the group if in one
                    aisling.Group?.Leave(aisling);
                } catch (Exception ex)
                {
                    Logger.WithTopics(
                              Topics.Servers.WorldServer,
                              Topics.Entities.Client,
                              Topics.Entities.Aisling,
                              Topics.Actions.Disconnect)
                          .WithProperty(client)
                          .LogError(ex, "{@Aisling} failed to leave group on disconnect", aisling);
                }

                try
                {
                    //leave guild channel if in one
                    aisling.Guild?.LeaveChannel(aisling);
                } catch (Exception ex)
                {
                    Logger.WithTopics(
                              Topics.Servers.WorldServer,
                              Topics.Entities.Client,
                              Topics.Entities.Aisling,
                              Topics.Actions.Disconnect)
                          .WithProperty(client)
                          .LogError(
                              ex,
                              "{@Aisling} failed to leave guild channel on disconnect (Guild: {@Guild}",
                              aisling,
                              aisling.Guild);
                }

                try
                {
                    //leave chat channels
                    foreach (var channel in aisling.ChannelSettings)
                        try
                        {
                            ChannelService.LeaveChannel(aisling, channel.ChannelName);
                        } catch
                        {
                            //ignored
                        }
                } catch (Exception ex)
                {
                    Logger.WithTopics(
                              Topics.Servers.WorldServer,
                              Topics.Entities.Client,
                              Topics.Entities.Aisling,
                              Topics.Actions.Disconnect)
                          .WithProperty(client)
                          .LogError(ex, "{@Aisling} failed to leave chat channels on disconnect", aisling);
                }

                try
                {
                    //save aisling
                    await SaveUserAsync(client.Aisling);
                } catch (Exception ex)
                {
                    Logger.WithTopics(
                              Topics.Servers.WorldServer,
                              Topics.Entities.Client,
                              Topics.Entities.Aisling,
                              Topics.Actions.Disconnect)
                          .WithProperty(client)
                          .LogError(ex, "{@Aisling} failed to save on disconnect", aisling);
                }

                try
                {
                    //remove aisling from map
                    mapInstance?.RemoveEntity(client.Aisling);
                } catch (Exception ex)
                {
                    Logger.WithTopics(
                              Topics.Servers.WorldServer,
                              Topics.Entities.Client,
                              Topics.Entities.Aisling,
                              Topics.Actions.Disconnect)
                          .WithProperty(client)
                          .LogError(ex, "{@Aisling} failed to remove from map on disconnect", aisling);
                }
            }
        } catch (Exception ex)
        {
            Logger.WithTopics(
                      Topics.Servers.WorldServer,
                      Topics.Entities.Client,
                      Topics.Entities.Aisling,
                      Topics.Actions.Disconnect)
                  .WithProperty(client)

                  // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                  .LogError(ex, "Exception thrown while {@AislingName} was trying to disconnect", client.Aisling?.Name ?? "N/A");
        }
    }

    private bool IsManualAction(ClientOpCode opCode)
        => opCode switch
        {
            ClientOpCode.ClientWalk            => true,
            ClientOpCode.Pickup                => true,
            ClientOpCode.ItemDrop              => true,
            ClientOpCode.ExitRequest           => true,
            ClientOpCode.Ignore                => true,
            ClientOpCode.PublicMessage         => true,
            ClientOpCode.SpellUse              => true,
            ClientOpCode.ClientRedirected      => true,
            ClientOpCode.Turn                  => true,
            ClientOpCode.Spacebar              => true,
            ClientOpCode.WorldListRequest      => true,
            ClientOpCode.Whisper               => true,
            ClientOpCode.OptionToggle          => true,
            ClientOpCode.ItemUse               => true,
            ClientOpCode.Emote                 => true,
            ClientOpCode.SetNotepad            => true,
            ClientOpCode.GoldDrop              => true,
            ClientOpCode.ItemDroppedOnCreature => true,
            ClientOpCode.GoldDroppedOnCreature => true,
            ClientOpCode.SelfProfileRequest    => true,
            ClientOpCode.GroupInvite           => true,
            ClientOpCode.ToggleGroup           => true,
            ClientOpCode.SwapSlot              => true,
            ClientOpCode.RefreshRequest        => true,
            ClientOpCode.MenuInteraction       => true,
            ClientOpCode.DialogInteraction     => true,
            ClientOpCode.BoardInteraction      => true,
            ClientOpCode.SkillUse              => true,
            ClientOpCode.WorldMapClick         => true,
            ClientOpCode.Click                 => true,
            ClientOpCode.Unequip               => true,
            ClientOpCode.RaiseStat             => true,
            ClientOpCode.ExchangeInteraction   => true,
            ClientOpCode.BeginChant            => true,
            ClientOpCode.Chant                 => true,
            ClientOpCode.EditableProfile       => true,
            ClientOpCode.SocialStatus          => true,
            _                                  => false
        };
    #endregion
}