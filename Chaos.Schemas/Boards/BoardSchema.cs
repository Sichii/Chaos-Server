namespace Chaos.Schemas.Boards;

/// <summary>
///     Represents the serializable schema of a board
/// </summary>
public sealed record BoardSchema
{
    /// <summary>
    ///     A key unique to this board. Must match the file name.
    /// </summary>
    public string Key { get; set; } = null!;
    /// <summary>
    ///     The name of the board
    /// </summary>
    public string Name { get; set; } = null!;
    /// <summary>
    ///     A collection of posts on the board
    /// </summary>
    public ICollection<PostSchema> Posts { get; set; } = Array.Empty<PostSchema>();
}