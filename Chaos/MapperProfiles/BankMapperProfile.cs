using Chaos.Containers;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Objects.Panel;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public class BankMapperProfile : IMapperProfile<Bank, BankSchema>
{
    private readonly ICloningService<Item> CloningService;
    private readonly ITypeMapper Mapper;

    public BankMapperProfile(ICloningService<Item> cloningService, ITypeMapper mapper)
    {
        CloningService = cloningService;
        Mapper = mapper;
    }

    public Bank Map(BankSchema obj)
    {
        var bank = new Bank(Mapper.MapMany<Item>(obj.Items), CloningService);
        bank.AddGold(obj.Gold);

        return bank;
    }

    public BankSchema Map(Bank obj) => new()
    {
        Gold = obj.Gold,
        Items = Mapper.MapMany<ItemSchema>(obj).ToList()
    };
}