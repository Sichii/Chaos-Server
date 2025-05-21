#region
using Chaos.Networking.Abstractions.Definitions;
#endregion

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of a board in the <see cref="ServerOpCode.DisplayBoard" /> packet
/// </summary>
public sealed record BoardInfo
{
    /// <summary>
    ///     The id of the board
    /// </summary>
    public ushort BoardId { get; set; }

    /// <summary>
    ///     The name of the board
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     A collection representing the posts on the board
    /// </summary>
    public ICollection<PostInfo> Posts { get; set; } = [];
}