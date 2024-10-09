using Chaos.Networking.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the group box in the <see cref="ServerOpCode.DisplayGroupInvite" /> packet
/// </summary>
public sealed record DisplayGroupBoxInfo
{
    /// <summary>
    ///     The current number of monks in the group
    /// </summary>
    public byte CurrentMonks { get; init; }

    /// <summary>
    ///     The current number of priests in the group
    /// </summary>
    public byte CurrentPriests { get; init; }

    /// <summary>
    ///     The current number of rogues in the group
    /// </summary>
    public byte CurrentRogues { get; init; }

    /// <summary>
    ///     The current number of warriors in the group
    /// </summary>
    public byte CurrentWarriors { get; init; }

    /// <summary>
    ///     The current number of wizards in the group
    /// </summary>
    public byte CurrentWizards { get; init; }

    /// <summary>
    ///     The maximum number of levels in the group
    /// </summary>
    public byte MaxLevel { get; init; }

    /// <summary>
    ///     The maximum number of monks in the group
    /// </summary>
    public byte MaxMonks { get; init; }

    /// <summary>
    ///     The maximum number of priests in the group
    /// </summary>
    public byte MaxPriests { get; init; }

    /// <summary>
    ///     The maximum number of rogues in the group
    /// </summary>
    public byte MaxRogues { get; init; }

    /// <summary>
    ///     The maximum number of warriors in the group
    /// </summary>
    public byte MaxWarriors { get; init; }

    /// <summary>
    ///     The maximum number of wizards in the group
    /// </summary>
    public byte MaxWizards { get; init; }

    /// <summary>
    ///     The minimum number of levels in the group
    /// </summary>
    public byte MinLevel { get; init; }

    /// <summary>
    ///     The name of the group
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     The note of the group
    /// </summary>
    public required string Note { get; init; }
}