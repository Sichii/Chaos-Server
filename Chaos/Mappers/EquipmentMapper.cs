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

public class EquipmentMapper : Profile
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly ICacheManager<string, ItemTemplate> ItemTemplateManager;

    public EquipmentMapper(ICacheManager<string, ItemTemplate> itemTemplateManager)
    {
        ItemTemplateManager = itemTemplateManager;

        CreateMap<SerialiableEquipment, Item>(MemberList.None)
            .ConstructUsing(s => new Item(ItemTemplateManager.GetObject(s.TemplateKey)));

        CreateMap<Item, SerialiableEquipment>(MemberList.None)
            .ForMember(s => s.ScriptKey,
                o =>
                {
                    o.Condition(i => i.Script != null);
                    o.MapFrom(i => i.Script!.ScriptKey);
                })
            .ForMember(s => s.TemplateKey,
                o => o.MapFrom(i => i.Template.TemplateKey));

        
        CreateMap<IEnumerable<SerialiableEquipment>, Equipment>(MemberList.None)
            .DisableCtorValidation()
            .IgnoreAllUnmapped()
            .AfterMap((src, dest, rc) =>
            {
                foreach (var sItem in src)
                {
                    var item = rc.Mapper.Map<Item>(sItem);
                    dest.TryAdd(item.Slot, item);
                }
            });

        
        CreateMap<Equipment, List<SerialiableEquipment>>(MemberList.None)
            .ConstructUsing(src => new List<SerialiableEquipment>())
            .IgnoreAllUnmapped()
            .AfterMap((src, dest, rc) =>
            {
                foreach (var item in src)
                {
                    var sItem = rc.Mapper.Map<SerialiableEquipment>(item);
                    dest.Add(sItem);
                }
            });
        
        CreateMap<Equipment, Dictionary<EquipmentSlot, ItemArg>>(MemberList.None)
            .ConstructUsing(src => new Dictionary<EquipmentSlot, ItemArg>())
            .IgnoreAllUnmapped()
            .AfterMap((src, dest, rc) =>
            {
                foreach (var item in src)
                {
                    var arg = rc.Mapper.Map<ItemArg>(item);
                    dest.Add((EquipmentSlot)arg.Slot, arg);
                }
            });
            
    }
}