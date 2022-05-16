using System.Net;
using Chaos.Core.Identity;
using Chaos.Core.Utilities;
using Chaos.Cryptography.Interfaces;
using Chaos.Networking.Options;

namespace Chaos.Networking.Model;

public record Redirect
{
    public ICryptoClient CryptoClient { get; }
    public IPEndPoint EndPoint { get; }
    public uint Id { get; }
    public string Name { get; }
    public ServerType Type { get; }

    public Redirect(
        ICryptoClient cryptoClient,
        RedirectInfo serverInfo,
        ServerType type,
        string? name = null
    )
    {
        Id = ClientId.NextId;
        Type = type;
        CryptoClient = cryptoClient;
        Name = name ?? "Login";

        var address = serverInfo.Address;

        if (IPAddress.IsLoopback(address))
            address = IPAddress.Loopback;

        EndPoint = new IPEndPoint(address, serverInfo.Port);
    }

    public override string ToString() => $"Id: {Id}, Name: {Name}, Type: {Type}";
}