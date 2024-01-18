using AutoMapper;
using Chaos.Models.Data;
using Chaos.Models.Templates;
using Chaos.Site.Extensions;
using Chaos.Site.Models;

namespace Chaos.Site.Services.MapperProfiles;

public sealed class ItemMapperProfile : Profile
{
    public ItemMapperProfile()
    {
        CreateMap<Attributes, ItemDto>()
            .ReverseMap();

        CreateMap<ItemTemplate, ItemDto>()
            .ForMember(rhs => rhs.Sprite, opt => opt.MapFrom(lhs => lhs.PanelSprite))
            .IncludeNullableMember(lhs => lhs.Modifiers)
            .ReverseMap();
    }
}