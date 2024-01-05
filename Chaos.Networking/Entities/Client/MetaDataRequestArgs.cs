using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.MetaDataRequest" /> packet
/// </summary>
public sealed record MetaDataRequestArgs : IPacketSerializable
{
    /// <summary>
    ///     The type of request the client is making
    /// </summary>
    public required MetaDataRequestType MetaDataRequestType { get; set; }

    /// <summary>
    ///     If specified, the name of the metadata the client is requesting data for
    ///     <br />
    ///     Should only be part of requests made with the MetaDataRequestType.DataByName type
    /// </summary>
    public string? Name { get; set; }
}