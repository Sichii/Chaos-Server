using Chaos.Collections;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public sealed class UserOptionsMapperProfile : IMapperProfile<UserOptions, UserOptionsSchema>
{
    public UserOptions Map(UserOptionsSchema obj) => new()
    {
        ShowBodyAnimations = obj.ShowBodyAnimations,
        ListenToHitSounds = obj.ListenToHitSounds,
        Option3 = obj.Option3,
        Option4 = obj.Option4,
        Option5 = obj.Option5,
        AllowExchange = obj.AllowExchange,
        //option 7 not used
        Option8 = obj.Option8,
        //other options
        AllowGroup = obj.AllowGroup
    };

    public UserOptionsSchema Map(UserOptions obj) => new()
    {
        ShowBodyAnimations = obj.ShowBodyAnimations,
        ListenToHitSounds = obj.ListenToHitSounds,
        Option3 = obj.Option3,
        Option4 = obj.Option4,
        Option5 = obj.Option5,
        AllowExchange = obj.AllowExchange,
        //option 7 not used
        Option8 = obj.Option8,
        //other options
        AllowGroup = obj.AllowGroup
    };
}