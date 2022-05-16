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
    private readonly IItemScriptFactory ItemScriptFactory;
    private readonly ISimpleCache<string, ItemTemplate> ItemTemplateCache;
    private readonly ILogger Logger;

    public ItemMapper(
        ISimpleCache<string, ItemTemplate> itemTemplateCache,
        IItemScriptFactory itemScriptFactory,
        ILogger<ItemMapper> logger
    )
    {
        ItemTemplateCache = itemTemplateCache;
        ItemScriptFactory = itemScriptFactory;
        Logger = logger;

        //item merge-clone
        CreateMap<Item, Item>(MemberList.None)
            .DisableCtorValidation()
            .ForMember(
                dest => dest.UniqueId,
                o => o.Ignore())
            .ForMember(
                dest => dest.Template,
                o => o.Ignore())
            .ForMember(
                dest => dest.Script,
                o => o.Ignore());

        CreateMap<SerializableItem, Item>(MemberList.None)
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

        CreateMap<Item, SerializableItem>(MemberList.None)
            .ForMember(
                s => s.ElapsedMs,
                o => o.MapFrom(i => i.Elapsed.TotalMilliseconds))
            .ForMember(
                s => s.TemplateKey,
                o => o.MapFrom(i => i.Template.TemplateKey));

        CreateMap<Item, ItemArg>(MemberList.None)
            .ForMember(
                a => a.Class,
                o => o.MapFrom(i => i.Template.BaseClass))
            .ForMember(
                a => a.Cost,
                o => o.MapFrom(i => i.Template.Cost))
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

        CreateMap<ICollection<SerializableItem>, IInventory>(MemberList.None)
            .DisableCtorValidation()
            .AfterMap(
                (src, dest, rc) =>
                {
                    foreach (var sItem in src)
                    {
                        var item = rc.Mapper.Map<Item>(sItem);
                        dest.TryAddDirect(item.Slot, item);
                    }
                });

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