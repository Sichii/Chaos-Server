using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record SetNotepadArgs(byte Slot, string Message) : IReceiveArgs;