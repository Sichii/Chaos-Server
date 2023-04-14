using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.DisplayVisibleEntities" /> packet
/// </summary>
public sealed record DisplayVisibleEntitiesArgs : ISendArgs
{
    /// <summary>
    ///     The (non-aisling) visible entities being sent to the client to display
    /// </summary>
    public ICollection<VisibleEntityInfo> VisibleObjects { get; set; } = new List<VisibleEntityInfo>();
}