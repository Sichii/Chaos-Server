using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record WhisperArgs(string TargetName, string Message) : IReceiveArgs;