using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Whisper" />
///     packet
/// </summary>
public sealed record WhisperArgs : IPacketSerializable
{
    /// <summary>
    ///     The message the client is trying to whisper to the target
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    ///     The name of the target the client is trying to send a whisper to
    /// </summary>
    public required string TargetName { get; set; }
}