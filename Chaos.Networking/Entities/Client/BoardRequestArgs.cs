using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.BoardRequest" /> packet
/// </summary>
/// <param name="BoardRequestType">The type of request being made</param>
public sealed record BoardRequestArgs(BoardRequestType BoardRequestType) : IReceiveArgs;