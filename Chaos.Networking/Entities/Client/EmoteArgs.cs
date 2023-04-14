using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Emote" /> packet
/// </summary>
/// <param name="BodyAnimation">The body animation the client is requesting to be displayed. </param>
public sealed record EmoteArgs(BodyAnimation BodyAnimation) : IReceiveArgs;