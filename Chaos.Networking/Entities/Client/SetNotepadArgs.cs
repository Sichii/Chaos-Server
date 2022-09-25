using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record SetNotepadArgs(byte Slot, string Message) : IReceiveArgs;