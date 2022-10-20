using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record WorldListArgs : ISendArgs
{
    public ICollection<WorldListMemberInfo> WorldList { get; set; } = new List<WorldListMemberInfo>();
}