using Chaos.DarkAges.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.UserId" /> packet
/// </summary>
public sealed record UserIdArgs : IPacketSerializable
{
    /// <summary>
    ///     The character's primary class
    /// </summary>
    public BaseClass BaseClass { get; set; }

    /// <summary>
    ///     The character's direction
    /// </summary>
    public Direction Direction { get; set; }

    /// <summary>
    ///     The character's id (this is used to attach the viewport to the character)
    /// </summary>
    public uint Id { get; set; }
}