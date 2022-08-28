using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record ExitRequestArgs(bool IsRequest) : IReceiveArgs;