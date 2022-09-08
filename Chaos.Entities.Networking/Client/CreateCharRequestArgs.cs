using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record CreateCharRequestArgs(string Name, string Password) : IReceiveArgs;

//TODO: group box