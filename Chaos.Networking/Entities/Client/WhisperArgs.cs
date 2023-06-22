using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Whisper" />
///     packet
/// </summary>
/// <param name="TargetName">The name of the target the client is trying to send a whisper to</param>
/// <param name="Message">The message the client is trying to whisper to the target</param>
public sealed record WhisperArgs(string TargetName, string Message) : IReceiveArgs;