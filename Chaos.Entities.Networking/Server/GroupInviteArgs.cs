using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record GroupInviteArgs : ISendArgs
{
    public GroupRequestType GroupRequestType { get; set; }
    public string SourceName { get; set; } = null!;
}