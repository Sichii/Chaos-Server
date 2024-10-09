using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of a world list entry in the <see cref="ServerOpCode.WorldList" /> packet
/// </summary>
public sealed record WorldListMemberInfo
{
    /// <summary>
    ///     The character's primary class
    /// </summary>
    public BaseClass BaseClass { get; set; }

    /// <summary>
    ///     The color the name will show up as on the list
    /// </summary>
    public WorldListColor Color { get; set; }

    /// <summary>
    ///     Whether or not the character is a master
    /// </summary>
    public bool IsMaster { get; set; }

    /// <summary>
    ///     The character's name
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     The character's social status
    /// </summary>
    public SocialStatus SocialStatus { get; set; }

    /// <summary>
    ///     The character's title, if any
    /// </summary>
    public string? Title { get; set; }
}