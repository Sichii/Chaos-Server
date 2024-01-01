using System.Net;
using System.Net.Sockets;
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Identity;
using Chaos.Cryptography;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities;
using Chaos.Networking.Entities.Client;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;
using Chaos.Security.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Servers;

public sealed class LoginServer : ServerBase<ILoginClient>, ILoginServer<ILoginClient>
{
    private readonly IAccessManager AccessManager;
    private readonly IAsyncStore<Aisling> AislingStore;
    private readonly ISimpleCacheProvider CacheProvider;
    private readonly IFactory<ILoginClient> ClientFactory;
    private readonly IFactory<MailBox> MailBoxFactory;
    private readonly IStore<MailBox> MailStore;
    private readonly IMetaDataStore MetaDataStore;
    private readonly Notice Notice;
    public ConcurrentDictionary<uint, CreateCharRequestArgs> CreateCharRequests { get; }
    private new LoginOptions Options { get; }

    public LoginServer(
        IAsyncStore<Aisling> aislingStore,
        IClientRegistry<ILoginClient> clientRegistry,
        IFactory<ILoginClient> clientFactory,
        ISimpleCacheProvider cacheProvider,
        IRedirectManager redirectManager,
        IPacketSerializer packetSerializer,
        IOptions<LoginOptions> options,
        ILogger<LoginServer> logger,
        IMetaDataStore metaDataStore,
        IAccessManager accessManager,
        IStore<MailBox> mailStore,
        IFactory<MailBox> mailBoxFactory)
        : base(
            redirectManager,
            packetSerializer,
            clientRegistry,
            options,
            logger)
    {
        Options = options.Value;
        AislingStore = aislingStore;
        ClientFactory = clientFactory;
        CacheProvider = cacheProvider;
        MetaDataStore = metaDataStore;
        AccessManager = accessManager;
        MailStore = mailStore;
        MailBoxFactory = mailBoxFactory;
        Notice = new Notice(options.Value.NoticeMessage);
        CreateCharRequests = new ConcurrentDictionary<uint, CreateCharRequestArgs>();

        IndexHandlers();
    }

    #region OnHandlers
    public ValueTask OnClientRedirected(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<ClientRedirectedArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnclientRedirect);

        ValueTask InnerOnclientRedirect(ILoginClient localClient, ClientRedirectedArgs localArgs)
        {
            var reservedRedirect
                = Options.ReservedRedirects.FirstOrDefault(rr => (rr.Id == localArgs.Id) && rr.Name.EqualsI(localArgs.Name));

            if (reservedRedirect != null)
            {
                Logger.WithTopics(Topics.Servers.LoginServer, Topics.Entities.Client, Topics.Actions.Redirect)
                      .WithProperty(localClient)
                      .WithProperty(reservedRedirect)
                      .LogDebug("Received external redirect {@RedirectID}", reservedRedirect.Id);

                localClient.Crypto = new Crypto(localArgs.Seed, localArgs.Key, string.Empty);
                localClient.SendLoginNotice(false, Notice);
            } else if (RedirectManager.TryGetRemove(localArgs.Id, out var redirect))
            {
                Logger.WithTopics(Topics.Servers.LoginServer, Topics.Entities.Client, Topics.Actions.Redirect)
                      .WithProperty(localClient)
                      .WithProperty(redirect)
                      .LogDebug("Received internal redirect {@RedirectId}", redirect.Id);

                localClient.Crypto = new Crypto(redirect.Seed, redirect.Key, redirect.Name);
                localClient.SendLoginNotice(false, Notice);
            } else
            {
                Logger.WithTopics(
                          Topics.Servers.LoginServer,
                          Topics.Entities.Client,
                          Topics.Actions.Redirect,
                          Topics.Qualifiers.Cheating)
                      .WithProperty(localClient)
                      .WithProperty(localArgs)
                      .LogWarning("{@ClientIp} tried to redirect with invalid redirect details", localClient.RemoteIp);

                localClient.Disconnect();
            }

            return default;
        }
    }

    public ValueTask OnCreateCharFinalize(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<CreateCharFinalizeArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnCreateCharFinalize);

        async ValueTask InnerOnCreateCharFinalize(ILoginClient localClient, CreateCharFinalizeArgs localArgs)
        {
            if (CreateCharRequests.TryGetValue(localClient.Id, out var requestArgs))
            {
                (var hairStyle, var gender, var hairColor) = localArgs;

                var mapInstanceCache = CacheProvider.GetCache<MapInstance>();
                var startingMap = mapInstanceCache.Get(Options.StartingMapInstanceId);

                var aisling = new Aisling(
                    requestArgs.Name,
                    gender,
                    hairStyle,
                    hairColor,
                    startingMap,
                    Options.StartingPoint);

                var mailBox = MailBoxFactory.Create(aisling.Name);

                await AislingStore.SaveAsync(aisling);
                MailStore.Save(mailBox);

                Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Create)
                      .WithProperty(localClient)
                      .LogInformation("New character created with name {@Name}", aisling.Name);

                localClient.SendLoginMessage(LoginMessageType.Confirm);
            } else
                localClient.SendLoginMessage(LoginMessageType.ClearNameMessage, "Unable to create character, bad request.");
        }
    }

    public ValueTask OnCreateCharRequest(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<CreateCharRequestArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnCreateCharRequest);

        async ValueTask InnerOnCreateCharRequest(ILoginClient localClient, CreateCharRequestArgs localArgs)
        {
            var result = await AccessManager.SaveNewCredentialsAsync(localClient.RemoteIp, localArgs.Name, localArgs.Password);

            if (result.Success)
            {
                CreateCharRequests.AddOrUpdate(localClient.Id, localArgs, (_, _) => localArgs);
                localClient.SendLoginMessage(LoginMessageType.Confirm, string.Empty);
            } else
            {
                Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Create)
                      .WithProperty(localClient)
                      .LogDebug("Failed to create character with name {@Name} for reason {@Reason}", localArgs.Name, result.FailureMessage);

                localClient.SendLoginMessage(GetLoginMessageType(result.Code), result.FailureMessage);
            }
        }
    }

    public ValueTask OnHomepageRequest(ILoginClient client, in ClientPacket packet)
    {
        return ExecuteHandler(client, InnerOnHomepageRequest);

        static ValueTask InnerOnHomepageRequest(ILoginClient localClient)
        {
            localClient.SendLoginControls(LoginControlsType.Homepage, "https://www.darkages.com");

            return default;
        }
    }

    public ValueTask OnLogin(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<LoginArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnLogin);

        async ValueTask InnerOnLogin(ILoginClient localClient, LoginArgs localArgs)
        {
            (var name, var password) = localArgs;

            var result = await AccessManager.ValidateCredentialsAsync(localClient.RemoteIp, name, password);

            if (!result.Success)
            {
                Logger.WithTopics(Topics.Entities.Client, Topics.Actions.Login, Topics.Actions.Validation)
                      .WithProperty(localClient)
                      .WithProperty(password)
                      .LogDebug("Failed to validate credentials for {@Name} for reason {@Reason}", name, result.FailureMessage);

                localClient.SendLoginMessage(LoginMessageType.WrongPassword, result.FailureMessage);

                return;
            }

            Logger.WithTopics(Topics.Entities.Client, Topics.Actions.Login, Topics.Actions.Validation)
                  .WithProperty(client)
                  .LogDebug("Validated credentials for {@Name}", name);

            var redirect = new Redirect(
                EphemeralRandomIdGenerator<uint>.Shared.NextId,
                Options.WorldRedirect,
                ServerType.World,
                localClient.Crypto.Key,
                localClient.Crypto.Seed,
                name);

            Logger.WithTopics(
                      Topics.Servers.LoginServer,
                      Topics.Entities.Client,
                      Topics.Actions.Login,
                      Topics.Actions.Redirect)
                  .LogDebug("Redirecting {@ClientIp} to {@ServerIp}", localClient.RemoteIp, Options.WorldRedirect.Address.ToString());

            RedirectManager.Add(redirect);
            localClient.SendLoginMessage(LoginMessageType.Confirm);
            localClient.SendRedirect(redirect);
        }
    }

    public ValueTask OnMetaDataRequest(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<MetaDataRequestArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnMetaDataRequest);

        ValueTask InnerOnMetaDataRequest(ILoginClient localClient, MetaDataRequestArgs localArgs)
        {
            (var metadataRequestType, var name) = localArgs;

            localClient.SendMetaData(metadataRequestType, MetaDataStore, name);

            return default;
        }
    }

    public ValueTask OnNoticeRequest(ILoginClient client, in ClientPacket packet)
    {
        return ExecuteHandler(client, InnerOnNoticeRequest);

        ValueTask InnerOnNoticeRequest(ILoginClient localClient)
        {
            localClient.SendLoginNotice(true, Notice);

            return default;
        }
    }

    public ValueTask OnPasswordChange(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<PasswordChangeArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnPasswordChange);

        async ValueTask InnerOnPasswordChange(ILoginClient localClient, PasswordChangeArgs localArgs)
        {
            (var name, var currentPassword, var newPassword) = localArgs;

            var result = await AccessManager.ChangePasswordAsync(
                localClient.RemoteIp,
                name,
                currentPassword,
                newPassword);

            if (!result.Success)
            {
                Logger.WithTopics(
                          Topics.Entities.Client,
                          Topics.Entities.Aisling,
                          Topics.Actions.Update,
                          Topics.Actions.Validation)
                      .WithProperty(client)
                      .LogInformation(
                          "Failed to change password for aisling {@AislingName} for reason {@Reason}",
                          name,
                          result.FailureMessage);

                localClient.SendLoginMessage(GetLoginMessageType(result.Code), result.FailureMessage);

                return;
            }

            Logger.WithTopics(
                      Topics.Entities.Client,
                      Topics.Entities.Aisling,
                      Topics.Actions.Update,
                      Topics.Actions.Validation)
                  .WithProperty(client)
                  .LogInformation("Changed password for aisling {@AislingName}", name);

            localClient.SendLoginMessage(LoginMessageType.Confirm);
        }
    }
    #endregion

    #region Connection / Handler
    public override ValueTask HandlePacketAsync(ILoginClient client, in ClientPacket packet)
    {
        var opCode = packet.OpCode;
        var handler = ClientHandlers[(byte)opCode];

        if (handler is not null)
            Logger.WithTopics(Topics.Servers.LoginServer, Topics.Entities.Packet, Topics.Actions.Processing)
                  .WithProperty(client)
                  .LogTrace("Processing message with code {@OpCode} from {@ClientIp}", opCode, client.RemoteIp);
        else if (opCode is ClientOpCode.ExitRequest or ClientOpCode.RequestProfile)
        {
            //ignored
            //these occasionally happen in the LoginServer for some unknown reason
            //ExitRequest might be from a double click from exiting
            //RequestProfile I have no idea tho
        } else
            Logger.WithTopics(
                      Topics.Servers.LoginServer,
                      Topics.Entities.Packet,
                      Topics.Actions.Processing,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(client)
                  .WithProperty(packet.ToString(), "HexData")
                  .LogWarning("Unknown message with code {@OpCode} from {@ClientIp}", opCode, client.RemoteIp);

        return handler?.Invoke(client, in packet) ?? default;
    }

    protected override void IndexHandlers()
    {
        if (ClientHandlers == null!)
            return;

        base.IndexHandlers();

        ClientHandlers[(byte)ClientOpCode.CreateCharRequest] = OnCreateCharRequest;
        ClientHandlers[(byte)ClientOpCode.CreateCharFinalize] = OnCreateCharFinalize;
        ClientHandlers[(byte)ClientOpCode.ClientRedirected] = OnClientRedirected;
        ClientHandlers[(byte)ClientOpCode.HomepageRequest] = OnHomepageRequest;
        ClientHandlers[(byte)ClientOpCode.Login] = OnLogin;
        ClientHandlers[(byte)ClientOpCode.MetaDataRequest] = OnMetaDataRequest;
        ClientHandlers[(byte)ClientOpCode.NoticeRequest] = OnNoticeRequest;
        ClientHandlers[(byte)ClientOpCode.PasswordChange] = OnPasswordChange;
    }

    protected override async void OnConnected(Socket clientSocket)
    {
        var ip = clientSocket.RemoteEndPoint as IPEndPoint;

        Logger.WithTopics(Topics.Servers.LoginServer, Topics.Entities.Client, Topics.Actions.Connect)
              .LogDebug("Incoming connection from {@ClientIp}", ip!.Address);

        try
        {
            await FinalizeConnectionAsync(clientSocket);
        } catch (Exception e)
        {
            Logger.WithTopics(Topics.Servers.LoginServer, Topics.Entities.Client, Topics.Actions.Connect)
                  .LogError(e, "Failed to finalize connection");
        }
    }

    private async Task FinalizeConnectionAsync(Socket clientSocket)
    {
        var ipAddress = ((IPEndPoint)clientSocket.RemoteEndPoint!).Address;

        if (!await AccessManager.ShouldAllowAsync(ipAddress))
        {
            Logger.WithTopics(
                      Topics.Servers.LoginServer,
                      Topics.Entities.Client,
                      Topics.Actions.Connect,
                      Topics.Actions.Disconnect)
                  .LogDebug("Rejected connection from {@ClientIp}", ipAddress);

            await clientSocket.DisconnectAsync(false);

            return;
        }

        var client = ClientFactory.Create(clientSocket);

        Logger.WithTopics(Topics.Servers.LoginServer, Topics.Entities.Client, Topics.Actions.Connect)
              .WithProperty(client)
              .LogInformation("Connection established with {@ClientIp}", client.RemoteIp);

        if (!ClientRegistry.TryAdd(client))
        {
            Logger.WithTopics(Topics.Servers.LoginServer, Topics.Entities.Client, Topics.Actions.Connect)
                  .WithProperty(client)
                  .LogError("Somehow two clients got the same id");

            client.Disconnect();

            return;
        }

        client.OnDisconnected += OnDisconnect;

        client.BeginReceive();
        client.SendAcceptConnection();
    }

    private void OnDisconnect(object? sender, EventArgs e)
    {
        var client = (ILoginClient)sender!;
        ClientRegistry.TryRemove(client.Id, out _);
    }

    private LoginMessageType GetLoginMessageType(CredentialValidationResult.FailureCode code)
        => code switch
        {
            CredentialValidationResult.FailureCode.InvalidUsername    => LoginMessageType.ClearNameMessage,
            CredentialValidationResult.FailureCode.InvalidPassword    => LoginMessageType.ClearPswdMessage,
            CredentialValidationResult.FailureCode.PasswordTooLong    => LoginMessageType.ClearPswdMessage,
            CredentialValidationResult.FailureCode.PasswordTooShort   => LoginMessageType.ClearPswdMessage,
            CredentialValidationResult.FailureCode.UsernameTooLong    => LoginMessageType.ClearNameMessage,
            CredentialValidationResult.FailureCode.UsernameTooShort   => LoginMessageType.ClearNameMessage,
            CredentialValidationResult.FailureCode.UsernameNotAllowed => LoginMessageType.ClearNameMessage,
            CredentialValidationResult.FailureCode.TooManyAttempts    => LoginMessageType.ClearPswdMessage,
            _                                                         => throw new ArgumentOutOfRangeException()
        };
    #endregion
}