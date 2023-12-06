using Chaos.Collections.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("create", helpText: "<itemTemplateKey> <amount?1>")]
public class CreateCommand(IItemFactory itemFactory) : ICommand<Aisling>
{
    private readonly IItemFactory ItemFactory = itemFactory;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var itemTemplateKey))
            return default;

        var amount = 1;

        if (args.TryGetNext<int>(out var amountArg))
            amount = amountArg;

        var item = ItemFactory.Create(itemTemplateKey);
        item.Count = amount;
        source.TryGiveItems(item);

        return default;
    }
}