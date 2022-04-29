using Chaos.Core.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record SocialStatusArgs(SocialStatus SocialStatus) : IReceiveArgs;