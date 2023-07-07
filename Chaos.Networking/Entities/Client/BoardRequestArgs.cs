using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.BoardRequest" />
///     packet
/// </summary>
/// <param name="BoardRequestType">The type of request being made by the client. This request type determines which properties are actually populated</param>
/// <param name="BoardId">If populated, the board id relevant to the request</param>
/// <param name="PostId">If populated, the id of the post being selected</param>
/// <param name="StartPostId">If populated, the id of the post id to begin paging from (counts down from short.MaxValue)</param>
/// <param name="To">If populated, this specifies the recipient of this private mail</param>
/// <param name="Subject">If populated, this specifies the subject of this private mail</param>
/// <param name="Message">If populated, this specifies the message contents of this private mail</param>
/// <param name="Controls">If populated, this represents the what action a player took while looking at a board</param>
public sealed record BoardRequestArgs(
    BoardRequestType BoardRequestType,
    ushort? BoardId = null,
    short? PostId = null,
    short? StartPostId = null,
    string? To = null,
    string? Subject = null,
    string? Message = null,
    BoardControls? Controls = null
) : IReceiveArgs;