using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record SetNotepadArgs(byte Slot, string Message) : IReceiveArgs;