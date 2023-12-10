using Chaos.MetaData.Abstractions;

namespace Chaos.Services.Storage.Abstractions;

public interface IMetaDataStore : IEnumerable<IMetaDataDescriptor>
{
    IMetaDataDescriptor Get(string name);
}