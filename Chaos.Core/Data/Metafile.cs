namespace Chaos.Core.Data;

public record Metafile(
    string Name,
    byte[] Data,
    ICollection<MetafileNode> Nodes,
    uint CheckSum
);