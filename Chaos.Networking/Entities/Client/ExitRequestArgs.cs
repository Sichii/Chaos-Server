using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record ExitRequestArgs(bool IsRequest) : IReceiveArgs;