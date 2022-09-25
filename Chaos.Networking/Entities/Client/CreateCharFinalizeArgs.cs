using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record CreateCharFinalizeArgs(byte HairStyle, Gender Gender, DisplayColor HairColor) : IReceiveArgs;