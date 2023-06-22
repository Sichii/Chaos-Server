using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.ExitRequest" />
///     packet
/// </summary>
/// <param name="IsRequest">
///     Whether or not this is a request or not. The player has not yet completed logging out if it is
///     a request.
/// </param>
public sealed record ExitRequestArgs(bool IsRequest) : IReceiveArgs;