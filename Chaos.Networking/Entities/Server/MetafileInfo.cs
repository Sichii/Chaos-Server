namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the defails of a metafile in the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.Metafile" /> packet
/// </summary>
public sealed record MetafileInfo
{
    /// <summary>
    ///     The checksum of the metafile
    /// </summary>
    public uint CheckSum { get; set; }
    /// <summary>
    ///     The data of the metafile
    /// </summary>
    public byte[] Data { get; set; } = Array.Empty<byte>();
    /// <summary>
    ///     The name of the metafile
    /// </summary>
    public string Name { get; set; } = null!;
}