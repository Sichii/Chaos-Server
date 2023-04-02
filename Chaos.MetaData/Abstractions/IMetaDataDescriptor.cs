namespace Chaos.MetaData.Abstractions;

/// <summary>
///     Provides details for a pieces of metadata
/// </summary>
public interface IMetaDataDescriptor
{
    /// <summary>
    ///     A checksum of the contained metadata
    /// </summary>
    uint CheckSum { get; set; }

    /// <summary>
    ///     The compressed data
    /// </summary>
    byte[] Data { get; set; }

    /// <summary>
    ///     The name of the metadata
    /// </summary>
    string Name { get; set; }

    /// <summary>
    ///     Compresses the metadata, stores it in <see cref="Data" /> and generates a checksum, storing it in <see cref="CheckSum" />.
    /// </summary>
    void Compress();
}