using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record WorldListArgs : ISendArgs
{
    public ICollection<WorldListArg> WorldList { get; set; } = new List<WorldListArg>();
}