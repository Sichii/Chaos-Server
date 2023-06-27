using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of a post in the <see cref="ServerOpCode.Board" /> packet
/// </summary>
public sealed record PostInfo
{
    /// <summary>
    ///     The name of the author of the post
    /// </summary>
    public string Author { get; set; } = null!;
    /// <summary>
    ///     The date the post was created
    /// </summary>
    public DateTime CreationDate { get; set; }
    /// <summary>
    ///     Whether or not the post is highlighted
    /// </summary>
    public bool IsHighlighted { get; set; }
    /// <summary>
    ///     The body of the post
    /// </summary>
    public string Message { get; set; } = null!;
    /// <summary>
    ///     The id of the post
    /// </summary>
    public short PostId { get; set; }
    /// <summary>
    ///     The subject/title of the post
    /// </summary>
    public string Subject { get; set; } = null!;
}