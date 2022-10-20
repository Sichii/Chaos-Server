using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record CreateCharRequestArgs(string Name, string Password) : IReceiveArgs;

//TODO: group box