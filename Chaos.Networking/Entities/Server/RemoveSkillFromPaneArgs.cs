using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record RemoveSkillFromPaneArgs : ISendArgs
{
    public byte Slot { get; set; }
}