using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record SocialStatusArgs(SocialStatus SocialStatus) : IReceiveArgs;