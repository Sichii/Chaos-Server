using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.ClientException" /> packet
/// </summary>
public sealed record ClientExceptionArgs : IPacketSerializable
{
    /// <summary>
    ///     The exception string sent from the client
    /// </summary>
    public required string ExceptionStr { get; set; }
}