using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.RaiseStat" /> packet <br />
/// </summary>
/// <param name="Stat">The stat the client is trying to raise</param>
public sealed record RaiseStatArgs(Stat Stat) : IReceiveArgs;