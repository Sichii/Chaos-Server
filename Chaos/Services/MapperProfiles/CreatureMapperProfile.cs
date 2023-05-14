using Chaos.Definitions;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class CreatureMapperProfile : IMapperProfile<Creature, CreatureInfo>
{
    /// <inheritdoc />
    public Creature Map(CreatureInfo obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public CreatureInfo Map(Creature obj) => new()
    {
        Id = obj.Id,
        X = obj.X,
        Y = obj.Y,
        Sprite = obj.Visibility is not VisibilityType.Normal ? (ushort)492 : obj.Sprite,
        CreatureType = obj.Type,
        Direction = obj.Direction,
        Name = obj.Name
    };
}