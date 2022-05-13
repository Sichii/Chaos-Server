using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record ClientRedirectedArgs(
    byte Seed,
    byte[] Key,
    string Name,
    uint Id
) : IReceiveArgs
{
    public override string ToString() => $"Id: {Id}, Name: {Name}";
}