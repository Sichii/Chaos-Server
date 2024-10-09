using Chaos.Networking.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the group box in the <see cref="ClientOpCode.GroupInvite" /> packet
/// </summary>
public sealed record CreateGroupBoxInfo
{
    /// <summary>
    ///     The max level allowed in the group
    /// </summary>
    public byte MaxLevel { get; set; }

    /// <summary>
    ///     The max number of monks allowed in the group
    /// </summary>
    public byte MaxMonks { get; set; }

    /// <summary>
    ///     The max number of priests allowed in the group
    /// </summary>
    public byte MaxPriests { get; set; }

    /// <summary>
    ///     The max number of rogues allowed in the group
    /// </summary>
    public byte MaxRogues { get; set; }

    /// <summary>
    ///     The max number of warriors allowed in the group
    /// </summary>
    public byte MaxWarriors { get; set; }

    /// <summary>
    ///     The max number of wizards allowed in the group
    /// </summary>
    public byte MaxWizards { get; set; }

    /// <summary>
    ///     The min level allowed in the group
    /// </summary>
    public byte MinLevel { get; set; }

    /// <summary>
    ///     The name of the group. (The message displayed in the box above the player's head)
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     The recruiting message displayed when someone clicked the group box.
    /// </summary>
    public required string Note { get; set; }
}