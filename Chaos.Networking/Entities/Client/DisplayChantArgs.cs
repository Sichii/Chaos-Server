using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record DisplayChantArgs(string ChantMessage) : IReceiveArgs;