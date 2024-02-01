using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.LoginControl" /> packet
/// </summary>
public sealed record LoginControlArgs : IPacketSerializable
{
    /// <summary>
    ///     The type of login controls to be used
    /// </summary>
    public LoginControlsType LoginControlsType { get; set; }

    /// <summary>
    ///     The payload to be sent to the client, but not necessarily displayed
    /// </summary>
    public string Message { get; set; } = null!;
}