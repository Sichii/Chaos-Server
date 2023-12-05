namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the defails of a metadata in the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.MetaData" /> packet
/// </summary>
public sealed record MetaDataInfo
{
    /// <summary>
    ///     The checksum of the metadata
    /// </summary>
    public uint CheckSum { get; set; }

    /// <summary>
    ///     The data of the metadata
    /// </summary>
    public byte[] Data { get; set; } = Array.Empty<byte>();

    /// <summary>
    ///     The name of the metadata
    /// </summary>
    public string Name { get; set; } = null!;
}