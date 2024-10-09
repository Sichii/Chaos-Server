using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.WorldList" /> packet
/// </summary>
public sealed record WorldListArgs : IPacketSerializable
{
    /// <summary>
    ///     A collection of information about characters in the current country
    /// </summary>
    public ICollection<WorldListMemberInfo> CountryList { get; set; } = Array.Empty<WorldListMemberInfo>();

    /// <summary>
    ///     The number of players in the world
    /// </summary>
    public ushort WorldMemberCount { get; set; }
}