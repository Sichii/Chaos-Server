using System.Net;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.Redirect" /> packet
/// </summary>
public sealed record RedirectArgs : IPacketSerializable
{
    /// <summary>
    ///     The endpoint the client should connect to
    /// </summary>
    public required IPEndPoint EndPoint { get; set; }

    /// <summary>
    ///     The id of the redirect
    /// </summary>
    public uint Id { get; set; }

    /// <summary>
    ///     The key to use for encryption
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    ///     The string to use for salt generation for envryption
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     The seed to use for encryption
    /// </summary>
    public byte Seed { get; set; }
}