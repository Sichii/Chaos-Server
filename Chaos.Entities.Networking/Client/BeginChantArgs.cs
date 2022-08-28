using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record BeginChantArgs(byte CastLineCount) : IReceiveArgs;