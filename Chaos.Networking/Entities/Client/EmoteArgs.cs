using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record EmoteArgs(BodyAnimation BodyAnimation) : IReceiveArgs;