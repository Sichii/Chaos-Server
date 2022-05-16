using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record EmoteArgs(BodyAnimation BodyAnimation) : IReceiveArgs;