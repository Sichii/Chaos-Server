using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record GroupRequestArgs(GroupRequestType GroupRequestType, string TargetName) : IReceiveArgs;