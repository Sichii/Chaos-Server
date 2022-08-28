using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record WorldMapArgs : ISendArgs
{
    public string FieldName { get; set; } = null!;
    public byte ImageIndex { get; set; }
    public ICollection<WorldMapNodeInfo> Nodes { get; set; } = new List<WorldMapNodeInfo>();
}