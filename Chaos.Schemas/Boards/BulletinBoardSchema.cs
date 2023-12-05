namespace Chaos.Schemas.Boards;

/// <summary>
///     Represents the serializable schema of a bulletin board
/// </summary>
public sealed record BulletinBoardSchema
{
    /// <summary>
    ///     A collection of posts on this bulletin board
    /// </summary>
    public ICollection<PostSchema> Posts { get; set; } = Array.Empty<PostSchema>();

    /// <summary>
    ///     A key unique to this bulletin board.
    /// </summary>
    public string TemplateKey { get; set; } = null!;
}