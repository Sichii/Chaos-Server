using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record WhisperArgs(string TargetName, string Message) : IReceiveArgs;