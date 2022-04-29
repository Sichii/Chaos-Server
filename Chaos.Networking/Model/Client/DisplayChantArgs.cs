using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record DisplayChantArgs(string ChantMessage) : IReceiveArgs;