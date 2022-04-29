using Chaos.Core.Data;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record WorldMapArgs : ISendArgs
{
    public string FieldName { get; set; } = null!;
    public byte ImageIndex { get; set; }
    public ICollection<WorldMapNode> Nodes { get; set; } = new List<WorldMapNode>();
}