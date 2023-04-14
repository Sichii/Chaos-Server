using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.WorldList" />
///     packet
/// </summary>
public sealed record WorldListArgs : ISendArgs
{
    /// <summary>
    ///     A collection of information about characters in the world
    /// </summary>
    public ICollection<WorldListMemberInfo> WorldList { get; set; } = new List<WorldListMemberInfo>();
}