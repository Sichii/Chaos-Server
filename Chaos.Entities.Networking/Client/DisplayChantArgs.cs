using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record DisplayChantArgs(string ChantMessage) : IReceiveArgs;