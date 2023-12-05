using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.Board" /> packet
/// </summary>
public sealed record BoardArgs : ISendArgs
{
    /// <summary>
    ///     If the type is to display a board, this is that board
    /// </summary>
    public BoardInfo? Board { get; set; }

    /// <summary>
    ///     If the type is to display a list of boards, these are those boards
    /// </summary>
    public ICollection<BoardInfo>? Boards { get; set; } = Array.Empty<BoardInfo>();

    /// <summary>
    ///     If the type is to display a post, this is whether or not the Prev button should be clickable
    /// </summary>
    public bool EnablePrevBtn { get; set; }

    /// <summary>
    ///     If the type is to display a post, this is that post
    /// </summary>
    public PostInfo? Post { get; set; }

    /// <summary>
    ///     If the type is a response type, this is the message that should be displayed to the user
    /// </summary>
    public string? ResponseMessage { get; set; }

    /// <summary>
    ///     When paging through a board, this is the post id of the first post to send
    /// </summary>
    public short? StartPostId { get; set; }

    /// <summary>
    ///     If the type is a response type, this is whether the given action was successful or not
    /// </summary>
    public bool? Success { get; set; }

    /// <summary>
    ///     The type of board or board response
    /// </summary>
    public BoardOrResponseType Type { get; set; }
}