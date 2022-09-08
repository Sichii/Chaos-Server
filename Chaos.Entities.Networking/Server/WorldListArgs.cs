using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record WorldListArgs : ISendArgs
{
    public ICollection<WorldListMemberInfo> WorldList { get; set; } = new List<WorldListMemberInfo>();
}