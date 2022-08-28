using Chaos.Common.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record EmoteArgs(BodyAnimation BodyAnimation) : IReceiveArgs;