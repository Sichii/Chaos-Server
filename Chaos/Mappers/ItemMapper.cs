using AutoMapper;
using Chaos.Caches.Interfaces;
using Chaos.Containers.Interfaces;
using Chaos.Factories.Interfaces;
using Chaos.Networking.Model.Server;
using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Mappers;

public class ItemMapper : Profile
{
    public ItemMapper()
    {
        CreateMap<Item, SerializableItem>(MemberList.None)
            .ForMember(
                s => s.ElapsedMs,
                o => o.MapFrom(i => i.Elapsed.TotalMilliseconds))
            .ForMember(
                s => s.TemplateKey,
                o => o.MapFrom(i => i.Template.TemplateKey));

        CreateMap<Item, ItemInfo>(MemberList.None)
            .ForMember(
                a => a.Class,
                o => o.MapFrom(i => i.Template.BaseClass))
            .ForMember(
                a => a.Cost,
                o => o.MapFrom(i => i.Template.Value))
            .ForMember(
                a => a.GameObjectType,
                o => o.MapFrom(i => GameObjectType.Item))
            .ForMember(
                a => a.MaxDurability,
                o => o.MapFrom(i => i.Template.MaxDurability ?? 0))
            .ForMember(
                a => a.Name,
                o => o.MapFrom(i => i.DisplayName))
            .ForMember(
                a => a.Sprite,
                o => o.MapFrom(i => i.Template.ItemSprite.OffsetPanelSprite))
            .ForMember(
                a => a.Stackable,
                o => o.MapFrom(i => i.Template.Stackable));
        
        CreateMap<IInventory, ICollection<SerializableItem>>(MemberList.None)
            .ConstructUsing(src => new List<SerializableItem>())
            .AfterMap(
                (src, dest, rc) =>
                {
                    foreach (var item in src)
                    {
                        var sItem = rc.Mapper.Map<Item, SerializableItem>(item);
                        dest.Add(sItem);
                    }
                });
    }
}