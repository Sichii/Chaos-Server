using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Objects.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;

namespace Chaos.Commands;

[Command("create")]
public class CreateCommand : ICommand<Aisling>
{
    private readonly ISimpleCache SimpleCache;
    private readonly IItemFactory ItemFactory;
    public CreateCommand(IItemFactory itemFactory, ISimpleCache simpleCache)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public void Execute(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGet<string>(0, out var itemTemplateKey))
            return;

        var template = SimpleCache.Get<ItemTemplate>(itemTemplateKey);
        
        if(!args.TryGet<int>(1, out var quantity))
            quantity = 1;

        if (template.Stackable)
        {
            var item = ItemFactory.Create(itemTemplateKey);
            item.Count = quantity;
            source.Inventory.TryAddToNextSlot(item);
        } else
            for (var i = 0; i < quantity; i++)
            {
                var item = ItemFactory.Create(itemTemplateKey);
                source.Inventory.TryAddToNextSlot(item);
            }
    }
}