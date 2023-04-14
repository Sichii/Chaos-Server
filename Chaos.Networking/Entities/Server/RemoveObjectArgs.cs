using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.RemoveObject" />
///     packet
/// </summary>
public sealed record RemoveObjectArgs : ISendArgs
{
    /// <summary>
    ///     The id of the object to be removed
    /// </summary>
    public uint SourceId { get; set; }
}