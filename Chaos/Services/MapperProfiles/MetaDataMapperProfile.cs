using Chaos.MetaData.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class MetaDataMapperProfile : IMapperProfile<IMetaDataDescriptor, MetaDataInfo>
{
    /// <inheritdoc />
    public IMetaDataDescriptor Map(MetaDataInfo obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public MetaDataInfo Map(IMetaDataDescriptor obj) => new()
    {
        Name = obj.Name,
        CheckSum = obj.CheckSum,
        Data = obj.Data
    };
}