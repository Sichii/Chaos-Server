using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.ServerTableRequest" /> packet <br />
/// </summary>
/// <param name="ServerTableRequestType">The type of request</param>
/// <param name="ServerId">
///     If specified, the id of the login server the client has chosen <br />
///     Should only be used with the ServerTableRequestType.ServerId
/// </param>
public sealed record ServerTableRequestArgs(ServerTableRequestType ServerTableRequestType, byte? ServerId) : IReceiveArgs;