using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.BeginChant" /> packet
/// </summary>
/// <param name="CastLineCount">The number of cast lines for the spell being chanted</param>
public sealed record BeginChantArgs(byte CastLineCount) : IReceiveArgs;