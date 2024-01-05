using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.ConfirmExit" />
///     packet
/// </summary>
public sealed record ConfirmExitArgs : IPacketSerializable
{
    /// <summary>
    ///     Whether or not the server is confirming that the client can exit
    /// </summary>
    public bool ExitConfirmed { get; set; }
}