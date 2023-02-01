namespace Chaos.Networking.Metadata.Abstractions;

public interface IMetaDataDescriptor
{
    uint CheckSum { get; set; }
    byte[] Data { get; set; }
    string Name { get; set; }

    void Compress();
}