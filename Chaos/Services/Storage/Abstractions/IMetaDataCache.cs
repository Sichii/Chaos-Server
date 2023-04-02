using Chaos.MetaData.Abstractions;

namespace Chaos.Services.Storage.Abstractions;

public interface IMetaDataCache : IEnumerable<IMetaDataDescriptor>
{
    uint GetCheckSum(string name);
    IMetaDataDescriptor GetMetafile(string name);

    void Load();
}