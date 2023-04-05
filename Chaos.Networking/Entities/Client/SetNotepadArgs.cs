using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.SetNotepad" /> packet <br />
/// </summary>
/// <param name="Slot">The slot of the object of which text is being written to</param>
/// <param name="Message">The message the client is trying to write to the object</param>
public sealed record SetNotepadArgs(byte Slot, string Message) : IReceiveArgs;