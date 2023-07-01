namespace Chaos.Schemas.Boards;

/// <summary>
///     Represents the serializable schema of a post
/// </summary>
public sealed record PostSchema
{
    /// <summary>
    ///     The author of the post
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
    ///     The subject of the post
    /// </summary>
    public string Subject { get; set; } = null!;
}