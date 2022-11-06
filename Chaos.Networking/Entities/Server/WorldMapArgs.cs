using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record WorldMapArgs : ISendArgs
{
    public byte FieldIndex { get; set; }
    public string FieldName { get; set; } = null!;
    public ICollection<WorldMapNodeInfo> Nodes { get; set; } = new List<WorldMapNodeInfo>();
}