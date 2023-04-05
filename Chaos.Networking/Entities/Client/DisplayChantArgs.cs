using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Chant" /> packet
/// </summary>
/// <param name="ChantMessage">The chant to be displayed</param>
public sealed record DisplayChantArgs(string ChantMessage) : IReceiveArgs;