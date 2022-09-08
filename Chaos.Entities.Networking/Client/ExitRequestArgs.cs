using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record ExitRequestArgs(bool IsRequest) : IReceiveArgs;