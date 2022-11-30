using System.Diagnostics;
using System.Net.Sockets;
using Chaos.Clients.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Identity;
using Chaos.Containers;
using Chaos.Cryptography;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Factories.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities.Client;
using Chaos.Networking.Options;
using Chaos.Objects;
using Chaos.Objects.World;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;
using Chaos.Security.Abstractions;
using Chaos.Security.Exceptions;
using Chaos.Servers.Options;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Servers;

public sealed class LoginServer : ServerBase<ILoginClient>, ILoginServer<ILoginClient>
{
    private readonly ISimpleCacheProvider CacheProvider;
    private readonly IClientFactory<ILoginClient> ClientFactory;
    private readonly ICredentialManager CredentialManager;
    private readonly Notice Notice;
    private readonly ISaveManager<Aisling> UserSaveManager;
    public ConcurrentDictionary<uint, CreateCharRequestArgs> CreateCharRequests { get; }
    protected override LoginOptions Options { get; }

    public LoginServer(
        ISaveManager<Aisling> userSaveManager,
        IClientRegistry<ILoginClient> clientRegistry,
        IClientFactory<ILoginClient> clientFactory,
        ICredentialManager credentialManager,
        ISimpleCacheProvider cacheProvider,
        IRedirectManager redirectManager,
        IPacketSerializer packetSerializer,
        IOptionsSnapshot<LoginOptions> options,
        ILogger<LoginServer> logger
    )
        : base(
            redirectManager,
            packetSerializer,
            clientRegistry,
            options,
            logger)
    {
        var opts = options.Value;

        Options = opts;
        UserSaveManager = userSaveManager;
        ClientFactory = clientFactory;
        CredentialManager = credentialManager;
        CacheProvider = cacheProvider;
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
            Logger.LogDebug("Received external redirect. ({Redirect})", reserved);
            client.CryptoClient = new CryptoClient(args.Seed, args.Key, string.Empty);
            client.SendLoginNotice(false, Notice);
        } else if (RedirectManager.TryGetRemove(args.Id, out var redirect))
        {
            Logger.LogDebug("Received internal redirect. ({Redirect}", redirect);
            client.CryptoClient = new CryptoClient(args.Seed, args.Key, args.Name);
            client.SendLoginNotice(false, Notice);
        } else
        {
            Logger.LogWarning("A client tried to redirect with invalid id. ({Args})", args);
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
            Logger.LogDebug("New character created ({Name})", user.Name);
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
        try
        {
            await CredentialManager.SaveNewCredentialsAsync(args.Name, args.Password);

            CreateCharRequests.AddOrUpdate(client.Id, args, (_, _) => args);
            client.SendLoginMessage(LoginMessageType.Confirm, string.Empty);
        } catch (UsernameCredentialException e)
        {
            var reasonMessage = e.Reason switch
            {
                UsernameCredentialException.ReasonType.InvalidFormat     => "Invalid format",
                UsernameCredentialException.ReasonType.TooLong           => "Too long",
                UsernameCredentialException.ReasonType.TooShort          => "Too short",
                UsernameCredentialException.ReasonType.InvalidCharacters => "Invalid characters",
                UsernameCredentialException.ReasonType.Reserved          => "Already exists",
                UsernameCredentialException.ReasonType.NotAllowed        => "Bad phrase",
                UsernameCredentialException.ReasonType.AlreadyExists     => "Already exists",
                UsernameCredentialException.ReasonType.DoesntExist       => throw new UnreachableException("Shouldn't happen"),
                UsernameCredentialException.ReasonType.Unknown           => "Unknown error",
                _                                                        => "Unknown error"
            };

            client.SendLoginMessage(LoginMessageType.ClearNameMessage, $"Failed to create character. Username error: {reasonMessage}");
        } catch (PasswordCredentialException e)
        {
            var reasonMessage = e.Reason switch
            {
                PasswordCredentialException.ReasonType.TooShort      => "Too short",
                PasswordCredentialException.ReasonType.TooLong       => "Too long",
                PasswordCredentialException.ReasonType.WrongPassword => throw new UnreachableException("Shouldn't happen"),
                _                                                    => "Unknown error"
            };

            client.SendLoginMessage(LoginMessageType.ClearPswdMessage, $"Failed to create character. Password error: {reasonMessage}");
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

        try
        {
            if (!await CredentialManager.ValidateCredentialsAsync(name, password))
            {
                client.SendLoginMessage(
                    LoginMessageType.WrongPassword,
                    $"Login failed. Reason: Password-{PasswordCredentialException.ReasonType.WrongPassword}");

                return;
            }
        } catch (UsernameCredentialException e)
        {
            client.SendLoginMessage(LoginMessageType.ClearNameMessage, $"Login failed. Reason: Username-{e.Reason}");

            return;
        }

        var redirect = new Redirect(
            ClientId.NextId,
            Options.WorldRedirect,
            ServerType.World,
            client.CryptoClient.Key,
            client.CryptoClient.Seed,
            name);

        Logger.LogDebug(
            "Redirecting login client to world server at {ServerAddress}:{ServerPort}",
            Options.WorldRedirect.Address,
            Options.WorldRedirect.Port);

        RedirectManager.Add(redirect);
        client.SendLoginMessage(LoginMessageType.Confirm);
        client.SendRedirect(redirect);
    }

    public ValueTask OnMetafileRequest(ILoginClient client, in ClientPacket packet)
    {
        (var metafileRequestType, var name) = PacketSerializer.Deserialize<MetafileRequestArgs>(in packet);

        client.SendMetafile(metafileRequestType, CacheProvider.GetCache<Metafile>(), name);

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
        try
        {
            (var name, var currentPassword, var newPassword) = args;
            await CredentialManager.ChangePasswordAsync(name, currentPassword, newPassword);
        } catch (UsernameCredentialException e)
        {
            client.SendLoginMessage(LoginMessageType.ClearNameMessage, $"Failed to change password. Reason: Username-{e.Reason}");
        } catch (PasswordCredentialException e)
        {
            client.SendLoginMessage(LoginMessageType.ClearPswdMessage, $"Failed to change password. Reason: Password-{e.Reason}");
        }
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
        client.SendAcceptConnection();
    }

    private void OnDisconnect(object? sender, EventArgs e)
    {
        var client = (ILoginClient)sender!;
        ClientRegistry.TryRemove(client.Id, out _);
    }
    #endregion
}