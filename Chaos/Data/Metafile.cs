namespace Chaos.Data;

public sealed record Metafile(
    string Name,
    byte[] Data,
    ICollection<MetafileNode> Nodes,
    uint CheckSum
);