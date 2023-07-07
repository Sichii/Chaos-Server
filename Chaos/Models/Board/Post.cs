namespace Chaos.Models.Board;

public sealed record Post(
    short PostId,
    string Author,
    string Subject,
    string Message,
    DateTime CreationDate,
    bool IsHighlighted
);