using Chaos.Models.Data;
using Chaos.Networking.Entities.Server;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class AnimationMapperProfile : IMapperProfile<Animation, AnimationArgs>
{
    /// <inheritdoc />
    public Animation Map(AnimationArgs obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public AnimationArgs Map(Animation obj)
        => new()
        {
            AnimationSpeed = obj.AnimationSpeed,
            SourceAnimation = obj.SourceAnimation,
            SourceId = obj.SourceId,
            TargetAnimation = obj.TargetAnimation,
            TargetId = obj.TargetId,
            TargetPoint = obj.TargetPoint
        };
}