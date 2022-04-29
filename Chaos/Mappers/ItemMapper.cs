using System.Collections.Generic;
using AutoMapper;
using Chaos.Containers;
using Chaos.Core.Definitions;
using Chaos.DataObjects.Serializable;
using Chaos.Extensions;
using Chaos.Managers.Interfaces;
using Chaos.Networking.Model.Server;
using Chaos.PanelObjects;
using Chaos.Templates;

namespace Chaos.Mappers;

public class ItemMapper : Profile
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly ICacheManager<string, ItemTemplate> ItemTemplateManager;

    public ItemMapper(ICacheManager<string, ItemTemplate> itemTemplateManager)
    {
        ItemTemplateManager = itemTemplateManager;
        //TODO: add scriptManager

        CreateMap<SerializableItem, Item>(MemberList.None)
            .ConstructUsing(s => new Item(ItemTemplateManager.GetObject(s.TemplateKey)));

        CreateMap<Item, SerializableItem>(MemberList.None)
            .ForMember(s => s.ScriptKey,
                o =>
                {
                    o.Condition(i => i.Script != null);
                    o.MapFrom(i => i.Script!.ScriptKey);
                })
            .ForMember(s => s.TemplateKey,
                o => o.MapFrom(i => i.Template.TemplateKey));

        CreateMap<Item, ItemArg>(MemberList.None)
            .ForMember(a => a.Class,
                o => o.MapFrom(i => i.Template.BaseClass))
            .ForMember(a => a.Cost,
                o => o.MapFrom(i => i.Template.Cost))
            .ForMember(a => a.GameObjectType,
                o => o.MapFrom(i => GameObjectType.Item))
            .ForMember(a => a.MaxDurability,
                o => o.MapFrom(i => i.Template.MaxDurability ?? 0))
            .ForMember(a => a.Name,
                o => o.MapFrom(i => i.DisplayName))
            .ForMember(a => a.Sprite,
                o => o.MapFrom(i => i.Template.Sprite))
            .ForMember(a => a.Stackable,
                o => o.MapFrom(i => i.Template.Stackable));
        
        CreateMap<IEnumerable<SerializableItem>, Inventory>(MemberList.None)
            .DisableCtorValidation()
            .IgnoreAllUnmapped()
            .AfterMap((src, dest, rc) =>
            {
                foreach (var sItem in src)
                {
                    var item = rc.Mapper.Map<Item>(sItem);
                    dest.TryAddDirect(item.Slot, item);
                }
            });

        
        CreateMap<Inventory, List<SerializableItem>>(MemberList.None)
            .ConstructUsing(src => new List<SerializableItem>())
            .IgnoreAllUnmapped()
            .AfterMap((src, dest, rc) =>
            {
                foreach (var item in src)
                {
                    var sItem = rc.Mapper.Map<SerializableItem>(item);
                    dest.Add(sItem);
                }
            });
    }
}