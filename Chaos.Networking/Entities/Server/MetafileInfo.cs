namespace Chaos.Networking.Entities.Server;

public sealed record MetafileInfo
{
    public uint CheckSum { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public string Name { get; set; } = null!;
}