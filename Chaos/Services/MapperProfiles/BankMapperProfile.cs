using Chaos.Collections;
using Chaos.Models.Panel;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public sealed class BankMapperProfile(ICloningService<Item> cloningService, ITypeMapper mapper) : IMapperProfile<Bank, BankSchema>
{
    private readonly ICloningService<Item> CloningService = cloningService;
    private readonly ITypeMapper Mapper = mapper;

    public Bank Map(BankSchema obj)
    {
        var bank = new Bank(Mapper.MapMany<Item>(obj.Items), CloningService);
        bank.AddGold(obj.Gold);

        return bank;
    }

    public BankSchema Map(Bank obj)
        => new()
        {
            Gold = obj.Gold,
            Items = Mapper.MapMany<ItemSchema>(obj)
                          .ToList()
        };
}