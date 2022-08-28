using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record WhisperArgs(string TargetName, string Message) : IReceiveArgs;