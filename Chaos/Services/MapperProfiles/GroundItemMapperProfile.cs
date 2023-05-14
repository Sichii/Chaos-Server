using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Networking.Entities.Server;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class GroundItemMapperProfile : IMapperProfile<GroundItem, GroundItemInfo>,
                                       IMapperProfile<Money, GroundItemInfo>
{
    /// <inheritdoc />
    GroundItem IMapperProfile<GroundItem, GroundItemInfo>.Map(GroundItemInfo obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public GroundItemInfo Map(Money obj) => new()
    {
        Id = obj.Id,
        Color = DisplayColor.Default,
        X = obj.X,
        Y = obj.Y,
        Sprite = obj.Visibility is not VisibilityType.Normal ? (ushort)0 : obj.Sprite
    };

    /// <inheritdoc />
    public GroundItemInfo Map(GroundItem obj) => new()
    {
        Id = obj.Id,
        Color = obj.Item.Color,
        X = obj.X,
        Y = obj.Y,
        Sprite = obj.Visibility is not VisibilityType.Normal ? (ushort)0 : obj.Sprite
    };

    /// <inheritdoc />
    Money IMapperProfile<Money, GroundItemInfo>.Map(GroundItemInfo obj) => throw new NotImplementedException();
}