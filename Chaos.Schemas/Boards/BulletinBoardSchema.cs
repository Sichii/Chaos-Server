namespace Chaos.Schemas.Boards;

/// <summary>
///     Represents the serializable schema of a bulletin board
/// </summary>
public sealed record BulletinBoardSchema
{
    /// <summary>
    ///     A key unique to this bulletin board.
    /// </summary>
    public string Key { get; set; } = null!;
    /// <summary>
    ///     A collection of names of aislings who can moderate this bulletin board
    /// </summary>
    public ICollection<string> Moderators { get; set; } = Array.Empty<string>();
    /// <summary>
    ///     The name of the bulletin board
    /// </summary>
    public string Name { get; set; } = null!;
    /// <summary>
    ///     A collection of posts on the bulletin board
    /// </summary>
    public ICollection<PostSchema> Posts { get; set; } = Array.Empty<PostSchema>();
    /// <summary>
    ///     A collection of names of scripts to attach to this object
    /// </summary>
    public ICollection<string> ScriptKeys { get; set; } = Array.Empty<string>();
}