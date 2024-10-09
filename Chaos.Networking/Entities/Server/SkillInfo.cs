using Chaos.Networking.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of a skill in the <see cref="ServerOpCode.AddSkillToPane" /> and
///     <see cref="ServerOpCode.DisplayMenu" /> packets
/// </summary>
public sealed record SkillInfo
{
    /// <summary>
    ///     The name of the skill
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     The text that appears when you hover this skill on the skill panel
    /// </summary>
    public string PanelName { get; set; } = null!;

    /// <summary>
    ///     The slot the skill is in
    /// </summary>
    public byte Slot { get; set; }

    /// <summary>
    ///     The sprite of the skill icon
    /// </summary>
    public ushort Sprite { get; set; }
}