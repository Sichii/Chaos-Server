namespace Chaos.Networking.Entities.Server;

/// <summary>
///     A base class used for serialization of any kind of WorldEntity
/// </summary>
public record WorldEntityInfo
{
    /// <summary>
    ///     The id of the entity
    /// </summary>
    public uint Id { get; set; }
}