using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record WorldListArgs : ISendArgs
{
    public ICollection<WorldListMemberInfo> WorldList { get; set; } = new List<WorldListMemberInfo>();
}