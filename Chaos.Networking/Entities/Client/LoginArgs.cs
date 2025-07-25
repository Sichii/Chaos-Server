#region
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.Login" /> packet
/// </summary>
public sealed record LoginArgs : IPacketSerializable
{
    /// <summary>
    ///     A unique identifier for the client that is unique to the installation.
    /// </summary>
    public required uint ClientId1 { get; set; }

    /// <summary>
    ///     A unique identifier for the client that is unique to the installation.
    /// </summary>
    public required ushort ClientId2 { get; set; }

    /// <summary>
    ///     The name of the aisling the client is trying to log in as
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     The password of the aisling the client is trying to log in as
    /// </summary>
    public required string Password { get; set; }
}