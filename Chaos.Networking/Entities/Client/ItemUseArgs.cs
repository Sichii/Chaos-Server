using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.UseItem" /> packet <br />
/// </summary>
/// <param name="SourceSlot">The slot of the item the client is trying to use</param>
public sealed record ItemUseArgs(byte SourceSlot) : IReceiveArgs;