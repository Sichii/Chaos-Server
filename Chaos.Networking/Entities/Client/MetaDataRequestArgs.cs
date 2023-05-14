using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.MetaDataRequest" /> packet
/// </summary>
/// <param name="MetaDataRequestType">The type of request the client is making</param>
/// <param name="Name">
///     If specified, the name of the metadata the client is requesting data for <br />
///     Should only be part of requests made with the MetaDataRequestType.DataByName type
/// </param>
public sealed record MetaDataRequestArgs(MetaDataRequestType MetaDataRequestType, string? Name = null) : IReceiveArgs;