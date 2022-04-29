using AutoMapper;
using Chaos.Containers;
using Chaos.DataObjects.Serializable;
using Chaos.Managers.Interfaces;
using Chaos.PanelObjects;
using Chaos.Templates;

namespace Chaos.Mappers;

public class BankMapper : Profile
{
    private readonly ICacheManager<string, ItemTemplate> ItemTemplateManager;

    public BankMapper(ICacheManager<string, ItemTemplate> itemTemplateManager)
    {
        ItemTemplateManager = itemTemplateManager;

        CreateMap<SerializableBankItem, Item>(MemberList.None)
            .ConstructUsing(s => new Item(ItemTemplateManager.GetObject(s.TemplateKey)));

        CreateMap<Item, SerializableBankItem>(MemberList.None)
            .ForMember(s => s.ScriptKey,
                o =>
                {
                    o.Condition(i => i.Script != null);
                    o.MapFrom(i => i.Script!.ScriptKey);
                })
            .ForMember(s => s.TemplateKey,
                o => o.MapFrom(i => i.Template.TemplateKey));

        CreateMap<SerializableBank, Bank>(MemberList.None)
            .ForMember(b => b.Gold,
                o => o.MapFrom(s => s.Gold))
            .ForMember(b => b.Items,
                o => o.MapFrom((src, dest, prop, rc) =>
                {
                    foreach (var sItem in src.Items)
                    {
                        var item = rc.Mapper.Map<Item>(sItem);
                        prop.Add(item.DisplayName, item);
                    }

                    return prop;
                }));

        CreateMap<Bank, SerializableBank>(MemberList.None)
            .ForMember(s => s.Gold,
                o => o.MapFrom(b => b.Gold))
            .ForMember(s => s.Items,
                o => o.MapFrom((src, dest, prop, rc) =>
                {
                    foreach (var item in src.Items.Values)
                    {
                        var sItem = rc.Mapper.Map<SerializableBankItem>(item);
                        prop.Add(sItem);
                    }

                    return prop;
                }));
    }
}