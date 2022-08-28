using Chaos.Common.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record IgnoreArgs(IgnoreType IgnoreType, string? TargetName) : IReceiveArgs;