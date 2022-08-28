using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record DisplayChantArgs(string ChantMessage) : IReceiveArgs;