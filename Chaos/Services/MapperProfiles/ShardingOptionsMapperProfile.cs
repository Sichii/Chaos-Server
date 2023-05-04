using Chaos.Models.Data;
using Chaos.Schemas.Data;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class ShardingOptionsMapperProfile : IMapperProfile<ShardingOptions, ShardingOptionsSchema>
{
    /// <inheritdoc />
    public ShardingOptions Map(ShardingOptionsSchema obj) => new()
    {
        ShardingType = obj.ShardingType,
        Limit = obj.Limit,
        ExitLocation = obj.ExitLocation
    };

    /// <inheritdoc />
    public ShardingOptionsSchema Map(ShardingOptions obj) => new()
    {
        ShardingType = obj.ShardingType,
        Limit = obj.Limit,
        ExitLocation = obj.ExitLocation
    };
}