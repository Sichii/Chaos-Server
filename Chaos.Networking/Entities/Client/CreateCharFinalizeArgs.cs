using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.CreateCharFinalize" /> packet
/// </summary>
/// <param name="HairStyle">The hairstyle to use for the new character</param>
/// <param name="Gender">The gender of the new character</param>
/// <param name="HairColor">The color of the new character's hair</param>
public sealed record CreateCharFinalizeArgs(byte HairStyle, Gender Gender, DisplayColor HairColor) : IReceiveArgs;