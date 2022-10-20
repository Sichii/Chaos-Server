using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record GroupInviteArgs : ISendArgs
{
    public GroupRequestType GroupRequestType { get; set; }
    public string SourceName { get; set; } = null!;
}