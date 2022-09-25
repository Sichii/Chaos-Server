using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record DisplayChantArgs(string ChantMessage) : IReceiveArgs;