using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record ExitRequestArgs(bool IsRequest) : IReceiveArgs;