using Chaos.MetaData.Abstractions;

namespace Chaos.Services.Storage.Abstractions;

public interface IMetaDataCache : IEnumerable<IMetaDataDescriptor>
{
    uint GetCheckSum(string name);
    IMetaDataDescriptor GetMetaData(string name);

    void Load();
}