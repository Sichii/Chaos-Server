using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.MetafileRequest" /> packet
/// </summary>
/// <param name="MetafileRequestType">The type of request the client is making</param>
/// <param name="Name">
///     If specified, the name of the metafile the client is requesting data for <br />
///     Should only be part of requests made with the MetafileRequestType.DataByName type
/// </param>
public sealed record MetafileRequestArgs(MetafileRequestType MetafileRequestType, string? Name = null) : IReceiveArgs;