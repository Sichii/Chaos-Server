using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record LoginArgs(string Name, string Password) : IReceiveArgs;