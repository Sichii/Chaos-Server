using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.HealthBar" />
///     packet
/// </summary>
public sealed record HealthBarArgs : ISendArgs
{
    /// <summary>
    ///     The percent of health to display in the bar
    /// </summary>
    public byte HealthPercent { get; set; }

    /// <summary>
    ///     If specified, the sound to play when displaying the health bar
    /// </summary>
    public byte? Sound { get; set; }

    /// <summary>
    ///     The id of the entity to display the health bar for
    /// </summary>
    public uint SourceId { get; set; }
}