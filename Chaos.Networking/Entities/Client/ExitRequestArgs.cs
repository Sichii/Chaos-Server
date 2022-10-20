using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record ExitRequestArgs(bool IsRequest) : IReceiveArgs;