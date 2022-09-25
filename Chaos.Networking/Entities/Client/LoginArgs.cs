using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record LoginArgs(string Name, string Password) : IReceiveArgs;