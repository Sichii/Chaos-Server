using System.Net;
using System.Net.Sockets;
using Chaos.Clients.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Identity;
using Chaos.Containers;
using Chaos.Cryptography;
using Chaos.Extensions.Common;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities;
using Chaos.Networking.Entities.Client;
using Chaos.Objects.World;
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

        var reserved = Options.ReservedRedirects
                              .FirstOrDefault(rr => (rr.Id == args.Id) && rr.Name.EqualsI(args.Name));

        if (reserved != null)
        {
            Logger.LogDebug("Received external {@Redirect}", reserved);
            client.Crypto = new Crypto(args.Seed, args.Key, string.Empty);
            client.SendLoginNotice(false, Notice);
        } else if (RedirectManager.TryGetRemove(args.Id, out var redirect))
        {
            Logger.LogDebug("Received internal {@Redirect}", redirect);
            client.Crypto = new Crypto(args.Seed, args.Key, args.Name);
            client.SendLoginNotice(false, Notice);
        } else
        {
            Logger.LogWarning("{@Client} tried to redirect with invalid {@Args}", client, args);
            client.Disconnect();
        }

        return default;
    }

    public ValueTask OnCreateCharFinalize(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<CreateCharFinalizeArgs>(in packet);

        return OnCreateCharFinalizeAsync(client, args);
    }

    private async ValueTask OnCreateCharFinalizeAsync(ILoginClient client, CreateCharFinalizeArgs args)
    {
        if (CreateCharRequests.TryGetValue(client.Id, out var requestArgs))
        {
            (var hairStyle, var gender, var hairColor) = args;

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
            Logger.LogDebug("New character created with name \"{Name}\" by {@Client}", user.Name, client);
            client.SendLoginMessage(LoginMessageType.Confirm);
        } else
            client.SendLoginMessage(LoginMessageType.ClearNameMessage, "Unable to create character, bad request.");
    }

    public ValueTask OnCreateCharRequest(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<CreateCharRequestArgs>(in packet);

        return OnCreateCharRequestAsync(client, args);
    }

    public async ValueTask OnCreateCharRequestAsync(ILoginClient client, CreateCharRequestArgs args)
    {
        var result = await AccessManager.SaveNewCredentialsAsync(client.RemoteIp, args.Name, args.Password);

        if (result.Success)
        {
            CreateCharRequests.AddOrUpdate(client.Id, args, (_, _) => args);
            client.SendLoginMessage(LoginMessageType.Confirm, string.Empty);
        } else
        {
            Logger.LogDebug(
                "Failed to create character with name \"{Name}\" by {@Client} for reason \"{Reason}\"",
                args.Name,
                client,
                result.FailureMessage);

            client.SendLoginMessage(GetLoginMessageType(result.Code), result.FailureMessage);
        }
    }

    public ValueTask OnHomepageRequest(ILoginClient client, in ClientPacket packet)
    {
        client.SendLoginControls(LoginControlsType.Homepage, "https://www.darkages.com");

        return default;
    }

    public ValueTask OnLogin(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<LoginArgs>(in packet);

        return OnLoginAsync(client, args);
    }

    private async ValueTask OnLoginAsync(ILoginClient client, LoginArgs args)
    {
        (var name, var password) = args;

        var result = await AccessManager.ValidateCredentialsAsync(client.RemoteIp, name, password);

        if (!result.Success)
        {
            Logger.LogDebug("Failed to validate credentials for {@Client} for reason \"{Reason}\"", client, result.FailureMessage);
            client.SendLoginMessage(LoginMessageType.WrongPassword, result.FailureMessage);

            return;
        }

        Logger.LogDebug("Validated credentials for {@Client}", client);

        var redirect = new Redirect(
            EphemeralRandomIdGenerator<uint>.Shared.NextId,
            Options.WorldRedirect,
            ServerType.World,
            client.Crypto.Key,
            client.Crypto.Seed,
            name);

        Logger.LogDebug("Redirecting {@Client} to {@Server}", client, Options.WorldRedirect);

        RedirectManager.Add(redirect);
        client.SendLoginMessage(LoginMessageType.Confirm);
        client.SendRedirect(redirect);
    }

    public ValueTask OnMetafileRequest(ILoginClient client, in ClientPacket packet)
    {
        (var metafileRequestType, var name) = PacketSerializer.Deserialize<MetafileRequestArgs>(in packet);

        client.SendMetafile(metafileRequestType, MetaDataCache, name);

        return default;
    }

    public ValueTask OnNoticeRequest(ILoginClient client, in ClientPacket packet)
    {
        client.SendLoginNotice(true, Notice);

        return default;
    }

    public ValueTask OnPasswordChange(ILoginClient client, in ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<PasswordChangeArgs>(in packet);

        return OnPasswordChangeAsync(client, args);
    }

    public async ValueTask OnPasswordChangeAsync(ILoginClient client, PasswordChangeArgs args)
    {
        (var name, var currentPassword, var newPassword) = args;

        var result = await AccessManager.ChangePasswordAsync(
            client.RemoteIp,
            name,
            currentPassword,
            newPassword);

        if (!result.Success)
        {
            Logger.LogInformation(
                "Failed to change password for {@Client} for username \"{UserName}\" for reason \"{Reason}\"",
                client,
                name,
                result.FailureMessage);

            client.SendLoginMessage(GetLoginMessageType(result.Code), result.FailureMessage);

            return;
        }

        Logger.LogInformation("Changed password for {@Client} for username \"{UserName}\"", client, name);
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
        ClientHandlers[(byte)ClientOpCode.MetafileRequest] = OnMetafileRequest;
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