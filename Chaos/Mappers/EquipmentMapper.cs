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
    private readonly ISimpleCache<string, ItemTemplate> ItemTemplateCache;
    private readonly ILogger Logger;

    public EquipmentMapper(
        ISimpleCache<string, ItemTemplate> itemTemplateCache,
        IItemScriptFactory itemScriptFactory,
        ILogger<EquipmentMapper> logger
    )
    {
        ItemTemplateCache = itemTemplateCache;
        ItemScriptFactory = itemScriptFactory;
        Logger = logger;

        CreateMap<SerialiableEquipment, Item>(MemberList.None)
            .ConstructUsing(s => new Item(ItemTemplateCache.GetObject(s.TemplateKey)))
            .ForMember(
                dest => dest.Elapsed,
                o => o.MapFrom(src => TimeSpan.FromMilliseconds(src.ElapsedMs)))
            .AfterMap(
                (_, dest) =>
                {
                    var scriptKeys = dest.Template.ScriptKeys
                                         .Concat(dest.ScriptKeys)
                                         .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    dest.Script = ItemScriptFactory.CreateScript(scriptKeys, dest);
                });

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

        CreateMap<IEnumerable<SerialiableEquipment>, IEquipment>(MemberList.None)
            .DisableCtorValidation()
            .AfterMap(
                (src, dest, rc) =>
                {
                    foreach (var sItem in src)
                    {
                        var item = rc.Mapper.Map<Item>(sItem);
                        dest.TryAdd(item.Slot, item);
                    }
                });

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

        CreateMap<Item, KeyValuePair<EquipmentSlot, ItemArg>>()
            .ConstructUsing((src, rc) => new KeyValuePair<EquipmentSlot, ItemArg>((EquipmentSlot)src.Slot, rc.Mapper.Map<ItemArg>(src)));

        /* Mapping is bugged? maybe the above map will be enough
        CreateMap<IEquipment, Dictionary<EquipmentSlot, ItemArg>>(MemberList.None)
            //.ConstructUsing(src => new Dictionary<EquipmentSlot, ItemArg>())
            //.IgnoreAllUnmapped()
            .AfterMap((src, dest, rc) =>
            {
                foreach (var item in src)
                {
                    var arg = rc.Mapper.Map<ItemArg>(item);
                    dest.Add((EquipmentSlot)arg.Slot, arg);
                }
            });
            */
    }
}