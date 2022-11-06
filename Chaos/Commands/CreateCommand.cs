using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Factories.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Commands;

[Command("create")]
public class CreateCommand : ICommand<Aisling>
{
    private readonly IItemFactory ItemFactory;

    public CreateCommand(IItemFactory itemFactory) => ItemFactory = itemFactory;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGet<string>(0, out var itemTemplateKey))
            return default;

        var amount = 1;

        if (args.TryGet<int>(0, out var amountArg))
            amount = amountArg;

        var item = ItemFactory.Create(itemTemplateKey);
        item.Count = amount;
        source.TryGiveItems(item);

        return default;
    }
}