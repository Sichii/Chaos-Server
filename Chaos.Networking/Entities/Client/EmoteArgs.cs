using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record EmoteArgs(BodyAnimation BodyAnimation) : IReceiveArgs;