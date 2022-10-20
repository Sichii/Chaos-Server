using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record BeginChantArgs(byte CastLineCount) : IReceiveArgs;