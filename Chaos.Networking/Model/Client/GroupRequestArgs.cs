using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record GroupRequestArgs(GroupRequestType GroupRequestType, string TargetName) : IReceiveArgs;