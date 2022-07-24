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

public class EquipmentMapper : Profile
{
    private readonly IItemScriptFactory ItemScriptFactory;
    private readonly ISimpleCache<ItemTemplate> ItemTemplateCache;
    private readonly ILogger Logger;

    public EquipmentMapper()
    {
        CreateMap<Item, SerialiableEquipment>(MemberList.None)
            .ForMember(
                s => s.ElapsedMs,
                o => o.MapFrom(i => i.Elapsed.TotalMilliseconds))
            .ForMember(
                s => s.TemplateKey,
                o => o.MapFrom(i => i.Template.TemplateKey))
            .ForMember(
                dest => dest.Slot,
                o => o.MapFrom(src => (EquipmentSlot)src.Slot));
        
        CreateMap<IEquipment, ICollection<SerialiableEquipment>>(MemberList.None)
            .ConstructUsing(src => new List<SerialiableEquipment>())
            .AfterMap(
                (src, dest, rc) =>
                {
                    foreach (var item in src)
                    {
                        var sItem = rc.Mapper.Map<SerialiableEquipment>(item);
                        dest.Add(sItem);
                    }
                });

        CreateMap<Item, KeyValuePair<EquipmentSlot, ItemInfo>>()
            .ConstructUsing((src, rc) => new KeyValuePair<EquipmentSlot, ItemInfo>((EquipmentSlot)src.Slot, rc.Mapper.Map<ItemInfo>(src)));
    }
}