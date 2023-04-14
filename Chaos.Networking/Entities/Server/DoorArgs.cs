using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.Door" /> packet
/// </summary>
public sealed record DoorArgs : ISendArgs
{
    /// <summary>
    ///     The doors being sent to the client to display
    /// </summary>
    public ICollection<DoorInfo> Doors { get; set; } = new List<DoorInfo>();
}