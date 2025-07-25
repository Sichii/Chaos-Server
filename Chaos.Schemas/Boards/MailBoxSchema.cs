namespace Chaos.Schemas.Boards;

/// <summary>
///     Represents the serializable schema of a mailbox
/// </summary>
public sealed record MailBoxSchema
{
    /// <summary>
    ///     A key unique to this mailbox.
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    ///     A collection of posts in the mailbox
    /// </summary>
    public ICollection<PostSchema> Posts { get; set; } = [];
}