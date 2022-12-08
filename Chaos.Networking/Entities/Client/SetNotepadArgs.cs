using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "Not implemented")]
public sealed record SetNotepadArgs(byte Slot, string Message) : IReceiveArgs;