#region
using AutoMapper;
using Chaos.Models.Data;
using Chaos.Models.Templates;
using Chaos.Site.Extensions;
using Chaos.Site.Models;
#endregion

namespace Chaos.Site.Services.MapperProfiles;

public sealed class ItemMapperProfile : Profile
{
    public ItemMapperProfile()
    {
        CreateMap<Attributes, ItemDto>()
            .ForMember(rhs => rhs.Hp, opt => opt.MapFrom(lhs => lhs.MaximumHp))
            .ForMember(rhs => rhs.Mp, opt => opt.MapFrom(lhs => lhs.MaximumMp))
            .ReverseMap();

        CreateMap<ItemTemplate, ItemDto>()
            .ForMember(rhs => rhs.Sprite, opt => opt.MapFrom(lhs => lhs.PanelSprite))
            .IncludeNullableMember(lhs => lhs.Modifiers)
            .ReverseMap();
    }
}