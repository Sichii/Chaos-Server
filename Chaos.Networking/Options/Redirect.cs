using System.Net;
using Chaos.Common.Definitions;
using Chaos.Networking.Abstractions;

namespace Chaos.Networking.Options;

public record Redirect : IRedirect
{
    public IPEndPoint EndPoint { get; }
    public uint Id { get; }
    public byte[] Key { get; }
    public string Name { get; }
    public byte Seed { get; }
    public ServerType Type { get; }

    public Redirect(
        uint id,
        IRedirectInfo serverInfo,
        ServerType type,
        byte[] key,
        byte seed,
        string? name = null
    )
    {
        Id = id;
        Type = type;
        Key = key;
        Seed = seed;
        Name = name ?? "Login";

        var address = serverInfo.Address;

        if (IPAddress.IsLoopback(address))
            address = IPAddress.Loopback;

        EndPoint = new IPEndPoint(address, serverInfo.Port);
    }

    public override string ToString() => $"Id: {Id}, Name: {Name}, Type: {Type}";
}