using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record BeginChantArgs(byte CastLineCount) : IReceiveArgs;