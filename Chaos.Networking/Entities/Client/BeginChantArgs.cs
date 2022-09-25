using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record BeginChantArgs(byte CastLineCount) : IReceiveArgs;