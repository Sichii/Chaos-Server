using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record LoginArgs(string Name, string Password) : IReceiveArgs;