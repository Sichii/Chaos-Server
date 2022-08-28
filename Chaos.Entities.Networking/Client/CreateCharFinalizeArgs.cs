using Chaos.Common.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record CreateCharFinalizeArgs(byte HairStyle, Gender Gender, DisplayColor HairColor) : IReceiveArgs;