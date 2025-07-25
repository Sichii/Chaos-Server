#region
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.Door" /> packet
/// </summary>
public sealed record DoorArgs : IPacketSerializable
{
    /// <summary>
    ///     The doors being sent to the client to display
    /// </summary>
    public ICollection<DoorInfo> Doors { get; set; } = [];
}