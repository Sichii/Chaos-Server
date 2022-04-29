using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Chaos.Clients.Interfaces;
using Chaos.Containers;
using Chaos.Core.Definitions;
using Chaos.Core.Extensions;
using Chaos.Cryptography;
using Chaos.DataObjects;
using Chaos.Exceptions;
using Chaos.Factories.Interfaces;
using Chaos.Managers.Interfaces;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Interfaces;
using Chaos.Networking.Model;
using Chaos.Networking.Model.Client;
using Chaos.Options;
using Chaos.Packets;
using Chaos.Packets.Definitions;
using Chaos.Packets.Interfaces;
using Chaos.Servers.Interfaces;
using Chaos.WorldObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Servers;

public class LoginServer : ServerBase, ILoginServer
{
    private readonly IClientFactory<ILoginClient> ClientFactory;
    private readonly IUserFactory UserFactory;
    private readonly ICredentialManager CredentialManager;
    private readonly ICacheManager<string, MapInstance> MapInstanceManager;
    private readonly ICacheManager<string, Metafile> MetafileManager;
    private readonly Notice Notice;
    private readonly ISaveManager<User> UserSaveManager;
    public ConcurrentDictionary<uint, ILoginClient> Clients { get; }
    public ConcurrentDictionary<uint, CreateCharRequestArgs> CreateCharRequests { get; }
    protected new LoginClientHandler?[] ClientHandlers { get; }
    protected override LoginOptions Options { get; }

    public LoginServer(
        ISaveManager<User> userSaveManager,
        IClientFactory<ILoginClient> clientFactory,
        IUserFactory userFactory,
        ICredentialManager credentialManager,
        ICacheManager<string, MapInstance> mapInstanceManager,
        ICacheManager<string, Metafile> metafileManager,
        IRedirectManager redirectManager,
        IPacketSerializer packetSerializer,
        IOptionsSnapshot<LoginOptions> options,
        Encoding encoding,
        ILogger<LoginServer> logger)
        : base(redirectManager, packetSerializer, options, logger)
    {
        UserSaveManager = userSaveManager;
        ClientFactory = clientFactory;
        UserFactory = userFactory;
        CredentialManager = credentialManager;
        MapInstanceManager = mapInstanceManager;
        MetafileManager = metafileManager;
        Notice = new Notice(options.Value.NoticeMessage, encoding);
        Clients = new ConcurrentDictionary<uint, ILoginClient>();
        CreateCharRequests = new ConcurrentDictionary<uint, CreateCharRequestArgs>();
        ClientHandlers = new LoginClientHandler[byte.MaxValue];
        Options = options.Value;

        IndexHandlers();
    }

    #region Connection / Handler
    protected delegate ValueTask LoginClientHandler(ILoginClient client, ref ClientPacket packet);
    
    public override ValueTask HandlePacketAsync<TClient>(TClient client, ref ClientPacket packet)
    {
        if (client is ILoginClient loginClient)
        {
            var handler = ClientHandlers[(byte)packet.OpCode];

            return handler?.Invoke(loginClient, ref packet) ?? default;
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

            ClientHandlers[i] = new LoginClientHandler(old);
        }

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

        if (!Clients.TryAdd(client.Id, client))
        {
            Logger.LogError("Somehow two clients got the same id. (Id: {Id})", client.Id);
            client.Disconnect();

            return;
        }
        
        client.OnDisconnected += (sender, args) =>
        {
            var sClient = (ILoginClient)sender!;
            Clients.TryRemove(sClient.Id, out _);
        };
        client.BeginReceive();
        client.SendAcceptConnection();
    }
    #endregion

    public ValueTask OnClientRedirected(ILoginClient client, ref ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<ClientRedirectedArgs>(ref packet);

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

    public ValueTask OnCreateCharFinalize(ILoginClient client, ref ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<CreateCharFinalizeArgs>(ref packet);

        return OnCreateCharFinalizeAsync(client, args);
    }

    private async ValueTask OnCreateCharFinalizeAsync(ILoginClient client, CreateCharFinalizeArgs args)
    {
        if (CreateCharRequests.TryGetValue(client.Id, out var requestArgs))
        {
            (var hairStyle, var gender, var hairColor) = args;
            var user = UserFactory.CreateUser(requestArgs.Name, gender, hairStyle, hairColor);
            var startingMap = MapInstanceManager.GetObject(Options.StartingMapInstanceId);
            user.MapInstance = startingMap;

            await UserSaveManager.SaveAsync(user);
            Logger.LogDebug("New character created ({Name})", user.Name);
            client.SendLoginMessage(LoginMessageType.Confirm);
        } else
            client.SendLoginMessage(LoginMessageType.ClearNameMessage, "Unable to create character, bad request.");
    }

    public ValueTask OnCreateCharRequest(ILoginClient client, ref ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<CreateCharRequestArgs>(ref packet);

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
            client.SendLoginMessage(LoginMessageType.ClearNameMessage, $"Failed to create character. Reason: Username-{e.Reason}");
        } catch (PasswordCredentialException e)
        {
            client.SendLoginMessage(LoginMessageType.ClearPswdMessage, $"Failed to create character. Reason: Password-{e.Reason}");
        }
    }

    public ValueTask OnHomepageRequest(ILoginClient client, ref ClientPacket packet)
    {
        client.SendLoginControls(LoginControlsType.Homepage, "https://www.darkages.com");

        return default;
    }

    public ValueTask OnLogin(ILoginClient client, ref ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<LoginArgs>(ref packet);

        return OnLoginAsync(client, args);
    }

    private async ValueTask OnLoginAsync(ILoginClient client, LoginArgs args)
    {
        (var name, var password) = args;

        try
        {
            if (!await CredentialManager.ValidateCredentialsAsync(name, password))
            {
                client.SendLoginMessage(LoginMessageType.WrongPassword,
                    $"Login failed. Reason: Password-{PasswordCredentialException.ReasonType.WrongPassword}");

                return;
            }
        } catch (UsernameCredentialException e)
        {
            client.SendLoginMessage(LoginMessageType.ClearNameMessage, $"Login failed. Reason: Username-{e.Reason}");

            return;
        }

        var redirect = new Redirect(client.CryptoClient, Options.WorldRedirect, ServerType.World, name);
        RedirectManager.Add(redirect);
        client.SendLoginMessage(LoginMessageType.Confirm);
        client.SendRedirect(redirect);
    }

    public ValueTask OnMetafileRequest(ILoginClient client, ref ClientPacket packet)
    {
        (var metafileRequestType, var name) = PacketSerializer.Deserialize<MetafileRequestArgs>(ref packet);

        client.SendMetafile(metafileRequestType, MetafileManager, name);

        return default;
    }

    public ValueTask OnNoticeRequest(ILoginClient client, ref ClientPacket packet)
    {
        client.SendLoginNotice(true, Notice);

        return default;
    }

    public ValueTask OnPasswordChange(ILoginClient client, ref ClientPacket packet)
    {
        var args = PacketSerializer.Deserialize<PasswordChangeArgs>(ref packet);

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


}