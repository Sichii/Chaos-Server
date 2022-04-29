using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record WhisperArgs(string TargetName, string Message) : IReceiveArgs;