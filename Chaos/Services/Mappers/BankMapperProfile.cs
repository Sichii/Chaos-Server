using Chaos.Containers;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Objects.Panel;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Services.Utility.Abstractions;

namespace Chaos.Services.Mappers;

public class BankMapperProfile : IMapperProfile<Bank, BankSchema>
{
    private readonly ITypeMapper Mapper;
    private readonly ICloningService<Item> CloningService;
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