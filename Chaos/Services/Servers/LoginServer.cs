using System.Net;
using System.Net.Sockets;
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Identity;
using Chaos.Cryptography;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities;
using Chaos.Networking.Entities.Client;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;
using Chaos.Security.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Servers;

public sealed class LoginServer : ServerBase<ILoginClient>, ILoginServer<ILoginClient>
{
    private readonly IAccessManager AccessManager;
    private readonly ISimpleCacheProvider CacheProvider;
    private readonly IClientProvider ClientProvider;
    private readonly IMetaDataCache MetaDataCache;
    private readonly Notice Notice;
    private readonly ISaveManager<Aisling> UserSaveManager;
    public ConcurrentDictionary<uint, CreateCharRequestArgs> CreateCharRequests { get; }
    private new LoginOptions Options { get; }

    public LoginServer(
        ISaveManager<Aisling> userSaveManager,
        IClientRegistry<ILoginClient> clientRegistry,
        IClientProvider clientProvider,
        ISimpleCacheProvider cacheProvider,
        IRedirectManager redirectManager,
        IPacketSerializer packetSerializer,
        IOptions<LoginOptions> options,
        ILogger<LoginServer> logger,
        IMetaDataCache metaDataCache,
        IAccessManager accessManager
    )
        : base(
            redirectManager,
            packetSerializer,
            clientRegistry,
            options,
            logger)
    {
        Options = options.Value;
        UserSaveManager = userSaveManager;
        ClientProvider = clientProvider;
        CacheProvider = cacheProvider;
        MetaDataCache = metaDataCache;
        AccessManager = accessManager;
        Notice = new Notice(options.Value.NoticeMessage);
        CreateCharRequests = new ConcurrentDictionary<uint, CreateCharRequestArgs>();

        IndexHandlers();
    }

    #region OnHandlers
    public ValueTask OnClientRedirected(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<ClientRedirectedArgs>(in packet);

        ValueTask InnerOnclientRedirect(ILoginClient localClient, ClientRedirectedArgs localArgs)
        {
            var reserved = Options.ReservedRedirects
                                  .FirstOrDefault(rr => (rr.Id == localArgs.Id) && rr.Name.EqualsI(localArgs.Name));

            if (reserved != null)
            {
                Logger.LogDebug("Received external {@Redirect}", reserved);
                localClient.Crypto = new Crypto(localArgs.Seed, localArgs.Key, string.Empty);
                localClient.SendLoginNotice(false, Notice);
            } else if (RedirectManager.TryGetRemove(localArgs.Id, out var redirect))
            {
                Logger.LogDebug("Received internal {@Redirect}", redirect);
                localClient.Crypto = new Crypto(localArgs.Seed, localArgs.Key, localArgs.Name);
                localClient.SendLoginNotice(false, Notice);
            } else
            {
                Logger.LogWarning("{@Client} tried to redirect with invalid {@Args}", localClient, localArgs);
                localClient.Disconnect();
            }

            return default;
        }

        return ExecuteHandler(client, args, InnerOnclientRedirect);
    }

    public ValueTask OnCreateCharFinalize(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<CreateCharFinalizeArgs>(in packet);

        async ValueTask InnerOnCreateCharFinalize(ILoginClient localClient, CreateCharFinalizeArgs localArgs)
        {
            if (CreateCharRequests.TryGetValue(localClient.Id, out var requestArgs))
            {
                (var hairStyle, var gender, var hairColor) = localArgs;

                var mapInstanceCache = CacheProvider.GetCache<MapInstance>();
                var startingMap = mapInstanceCache.Get(Options.StartingMapInstanceId);

                var user = new Aisling(
                    requestArgs.Name,
                    gender,
                    hairStyle,
                    hairColor,
                    startingMap,
                    Options.StartingPoint);

                await UserSaveManager.SaveAsync(user);
                Logger.LogDebug("New character created with name \"{Name}\" by {@Client}", user.Name, localClient);
                localClient.SendLoginMessage(LoginMessageType.Confirm);
            } else
                localClient.SendLoginMessage(LoginMessageType.ClearNameMessage, "Unable to create character, bad request.");
        }

        return ExecuteHandler(client, args, InnerOnCreateCharFinalize);
    }

    public ValueTask OnCreateCharRequest(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<CreateCharRequestArgs>(in packet);

        async ValueTask InnerOnCreateCharRequest(ILoginClient localClient, CreateCharRequestArgs localArgs)
        {
            var result = await AccessManager.SaveNewCredentialsAsync(localClient.RemoteIp, localArgs.Name, localArgs.Password);

            if (result.Success)
            {
                CreateCharRequests.AddOrUpdate(localClient.Id, localArgs, (_, _) => localArgs);
                localClient.SendLoginMessage(LoginMessageType.Confirm, string.Empty);
            } else
            {
                Logger.LogDebug(
                    "Failed to create character with name \"{Name}\" by {@Client} for reason \"{Reason}\"",
                    localArgs.Name,
                    localClient,
                    result.FailureMessage);

                localClient.SendLoginMessage(GetLoginMessageType(result.Code), result.FailureMessage);
            }
        }

        return ExecuteHandler(client, args, InnerOnCreateCharRequest);
    }

    public ValueTask OnHomepageRequest(ILoginClient client, in ClientPacket packet)
    {
        static ValueTask InnerOnHomepageRequest(ILoginClient localClient)
        {
            localClient.SendLoginControls(LoginControlsType.Homepage, "https://www.darkages.com");

            return default;
        }

        return ExecuteHandler(client, InnerOnHomepageRequest);
    }

    public ValueTask OnLogin(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<LoginArgs>(in packet);

        async ValueTask InnerOnLogin(ILoginClient localClient, LoginArgs localArgs)
        {
            (var name, var password) = localArgs;

            var result = await AccessManager.ValidateCredentialsAsync(localClient.RemoteIp, name, password);

            if (!result.Success)
            {
                Logger.LogDebug("Failed to validate credentials for {@Client} for reason \"{Reason}\"", localClient, result.FailureMessage);
                localClient.SendLoginMessage(LoginMessageType.WrongPassword, result.FailureMessage);

                return;
            }

            Logger.LogDebug("Validated credentials for {@Client}", localClient);

            var redirect = new Redirect(
                EphemeralRandomIdGenerator<uint>.Shared.NextId,
                Options.WorldRedirect,
                ServerType.World,
                localClient.Crypto.Key,
                localClient.Crypto.Seed,
                name);

            Logger.LogDebug("Redirecting {@Client} to {@Server}", localClient, Options.WorldRedirect);

            RedirectManager.Add(redirect);
            localClient.SendLoginMessage(LoginMessageType.Confirm);
            localClient.SendRedirect(redirect);
        }

        return ExecuteHandler(client, args, InnerOnLogin);
    }

    public ValueTask OnMetaDataRequest(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<MetaDataRequestArgs>(in packet);

        ValueTask InnerOnMetaDataRequest(ILoginClient localClient, MetaDataRequestArgs localArgs)
        {
            (var metadataRequestType, var name) = localArgs;

            localClient.SendMetaData(metadataRequestType, MetaDataCache, name);

            return default;
        }

        return ExecuteHandler(client, args, InnerOnMetaDataRequest);
    }

    public ValueTask OnNoticeRequest(ILoginClient client, in ClientPacket packet)
    {
        ValueTask InnerOnNoticeRequest(ILoginClient localClient)
        {
            localClient.SendLoginNotice(true, Notice);

            return default;
        }

        return ExecuteHandler(client, InnerOnNoticeRequest);
    }

    public ValueTask OnPasswordChange(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<PasswordChangeArgs>(in packet);

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
                Logger.LogInformation(
                    "Failed to change password for {@Client} for username \"{UserName}\" for reason \"{Reason}\"",
                    localClient,
                    name,
                    result.FailureMessage);

                localClient.SendLoginMessage(GetLoginMessageType(result.Code), result.FailureMessage);

                return;
            }

            Logger.LogInformation("Changed password for {@Client} for username \"{UserName}\"", localClient, name);
        }

        return ExecuteHandler(client, args, InnerOnPasswordChange);
    }
    #endregion

    #region Connection / Handler
    public override ValueTask HandlePacketAsync(ILoginClient client, in ClientPacket packet)
    {
        var handler = ClientHandlers[(byte)packet.OpCode];

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

    protected override async void OnConnection(IAsyncResult ar)
    {
        var serverSocket = (Socket)ar.AsyncState!;
        var clientSocket = serverSocket.EndAccept(ar);

        serverSocket.BeginAccept(OnConnection, serverSocket);

        var ip = clientSocket.RemoteEndPoint as IPEndPoint;
        Logger.LogDebug("Incoming connection from {Ip}", ip);

        try
        {
            await FinalizeConnectionAsync(clientSocket);
        } catch (Exception e)
        {
            Logger.LogError(e, "Failed to finalize connection");
        }
    }

    private async Task FinalizeConnectionAsync(Socket clientSocket)
    {
        var ipAddress = ((IPEndPoint)clientSocket.RemoteEndPoint!).Address;

        if (!await AccessManager.ShouldAllowAsync(ipAddress))
        {
            Logger.LogDebug("Rejected connection from {IpAddress}", ipAddress);

            return;
        }

        var client = ClientProvider.CreateClient<ILoginClient>(clientSocket);

        Logger.LogDebug("Connection established with {@Client}", client);

        if (!ClientRegistry.TryAdd(client))
        {
            Logger.LogError("Somehow two clients got the same id. (Id: {Id})", client.Id);
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

    private LoginMessageType GetLoginMessageType(CredentialValidationResult.FailureCode code) => code switch
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