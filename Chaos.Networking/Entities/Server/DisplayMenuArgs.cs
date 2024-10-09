using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.DisplayMenu" /> packet
/// </summary>
public sealed record DisplayMenuArgs : IPacketSerializable
{
    /// <summary>
    ///     If this is a "WithArgs" menu type, this is the argument passed to the client with this dialog
    /// </summary>
    public string? Args { get; set; }

    /// <summary>
    ///     The color associated with the source of the menu. (for items and aislings)
    /// </summary>
    public required DisplayColor Color { get; set; }

    /// <summary>
    ///     The entity type of the source of the menu. (item, creature, aisling, etc)
    /// </summary>
    public required EntityType EntityType { get; set; }

    /// <summary>
    ///     If this menu type shows a shop, this is the collection of items that are available for purchase
    /// </summary>
    public ICollection<ItemInfo>? Items { get; set; }

    /// <summary>
    ///     The type of menu
    /// </summary>
    public required MenuType MenuType { get; set; }

    /// <summary>
    ///     The name of the source entity associated with the menu
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     If the menu has options, this is a collection of strings that represent selectable options in the dialog's menu
    /// </summary>
    public ICollection<(string Text, ushort Pursuit)>? Options { get; set; }

    /// <summary>
    ///     If the menu is part of a pursuit chain, this is the id of that pursuit
    /// </summary>
    public ushort PursuitId { get; set; }

    /// <summary>
    ///     Whether or not the menu should show an illustration of the source entity
    /// </summary>
    public bool ShouldIllustrate { get; set; }

    /// <summary>
    ///     If this menu type shows a list of skills to learn, this is the collection of skills that are available for learning
    /// </summary>
    public ICollection<SkillInfo>? Skills { get; set; }

    /// <summary>
    ///     If this menu type shows a list of entities the player has, this is a collection of the slots to display
    /// </summary>
    public ICollection<byte>? Slots { get; set; }

    /// <summary>
    ///     The id of the source entity associated with the menu. This isn't really required for any practical purpose
    /// </summary>
    public uint? SourceId { get; set; }

    /// <summary>
    ///     If this menu type shows a list of spells to learn, this is the collection of spells that are available for learning
    /// </summary>
    public ICollection<SpellInfo>? Spells { get; set; }

    /// <summary>
    ///     The sprite of the source entity associated with the menu
    /// </summary>
    public required ushort Sprite { get; set; }

    /// <summary>
    ///     The text of the menu
    /// </summary>
    public required string Text { get; set; }
}