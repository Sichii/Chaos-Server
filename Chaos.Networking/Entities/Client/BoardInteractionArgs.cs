using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.BoardInteraction" /> packet
/// </summary>
public sealed record BoardInteractionArgs : IPacketSerializable
{
    /// <summary>
    ///     If populated, the board id relevant to the request
    /// </summary>
    public ushort? BoardId { get; set; }

    /// <summary>
    ///     The type of request being made by the client. This request type determines which properties are actually populated
    /// </summary>
    public required BoardRequestType BoardRequestType { get; set; }

    /// <summary>
    ///     If populated, this represents the what action a player took while looking at a board
    /// </summary>
    public BoardControls? Controls { get; set; }

    /// <summary>
    ///     If populated, this specifies the message contents of this private mail
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    ///     If populated, the id of the post being selected
    /// </summary>
    public short? PostId { get; set; }

    /// <summary>
    ///     If populated, the id of the post id to begin paging from (counts down from short.MaxValue)
    /// </summary>
    public short? StartPostId { get; set; }

    /// <summary>
    ///     If populated, this specifies the subject of this private mail
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    ///     If populated, this specifies the recipient of this private mail
    /// </summary>
    public string? To { get; set; }
}