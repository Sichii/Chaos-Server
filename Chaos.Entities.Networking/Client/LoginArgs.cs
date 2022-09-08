using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record LoginArgs(string Name, string Password) : IReceiveArgs;