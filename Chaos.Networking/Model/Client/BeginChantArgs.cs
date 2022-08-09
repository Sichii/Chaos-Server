using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record BeginChantArgs(byte CastLineCount) : IReceiveArgs;