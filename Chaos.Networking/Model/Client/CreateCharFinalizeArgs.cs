using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record CreateCharFinalizeArgs(byte HairStyle, Gender Gender, DisplayColor HairColor) : IReceiveArgs;