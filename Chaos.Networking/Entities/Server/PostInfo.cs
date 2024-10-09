using Chaos.Networking.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of a post in the <see cref="ServerOpCode.DisplayBoard" /> packet
/// </summary>
public sealed record PostInfo
{
    /// <summary>
    ///     The name of the author of the post
    /// </summary>
    public string Author { get; set; } = null!;

    /// <summary>
    ///     The day of the month the post was created
    /// </summary>
    public int DayOfMonth { get; set; }

    /// <summary>
    ///     Whether or not the post is highlighted
    /// </summary>
    public bool IsHighlighted { get; set; }

    /// <summary>
    ///     The body of the post
    /// </summary>
    public string Message { get; set; } = null!;

    /// <summary>
    ///     The month the post was created
    /// </summary>
    public int MonthOfYear { get; set; }

    /// <summary>
    ///     The id of the post
    /// </summary>
    public short PostId { get; set; }

    /// <summary>
    ///     The subject/title of the post
    /// </summary>
    public string Subject { get; set; } = null!;
}