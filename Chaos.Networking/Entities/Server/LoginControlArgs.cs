using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.LoginControls" /> packet
/// </summary>
public sealed record LoginControlArgs : ISendArgs
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