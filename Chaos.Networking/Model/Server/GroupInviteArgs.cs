using Chaos.Networking.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record GroupInviteArgs : ISendArgs
{
    public GroupRequestType GroupRequestType { get; set; }
    public string SourceName { get; set; } = null!;
}