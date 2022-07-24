using AutoMapper;
using Chaos.Caches.Interfaces;
using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Templates;

namespace Chaos.Mappers;

public class BankMapper : Profile
{
    public BankMapper()
    {
        CreateMap<Item, SerializableBankItem>(MemberList.None)
            .ForMember(
                s => s.ScriptKeys,
                o => o.MapFrom(i => i.ScriptKeys))
            .ForMember(
                s => s.TemplateKey,
                o => o.MapFrom(i => i.Template.TemplateKey));
        
        CreateMap<Bank, SerializableBank>(MemberList.None)
            .ForMember(
                s => s.Gold,
                o => o.MapFrom(b => b.Gold))
            .ForMember(
                s => s.Items,
                o => o.MapFrom(
                    (
                        src,
                        dest,
                        prop,
                        rc
                    ) =>
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