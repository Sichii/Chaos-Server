using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record ClientRedirectedArgs(
    byte Seed,
    byte[] Key,
    string Name,
    uint Id
) : IReceiveArgs
{
    public override string ToString() => $"Id: {Id}, Name: {Name}";
}