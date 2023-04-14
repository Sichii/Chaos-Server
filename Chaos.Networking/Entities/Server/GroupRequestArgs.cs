using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.GroupRequest" />
///     packet
/// </summary>
public sealed record GroupRequestArgs : ISendArgs
{
    /// <summary>
    ///     The type of the request
    /// </summary>
    public GroupRequestType GroupRequestType { get; set; }
    /// <summary>
    ///     The name of the source of the request
    /// </summary>
    public string SourceName { get; set; } = null!;
}