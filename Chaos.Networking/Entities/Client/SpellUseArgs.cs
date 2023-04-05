using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.UseSpell" /> packet <br />
/// </summary>
/// <param name="SourceSlot">The slot of the spell the client is trying to use</param>
/// <param name="ArgsData">
///     The rest of the packet data because the networking layer doesn't know what type of spell is being used,
///     so it can't determine if it should read a target id and point, or a prompt
/// </param>
public sealed record SpellUseArgs(byte SourceSlot, byte[] ArgsData) : IReceiveArgs;