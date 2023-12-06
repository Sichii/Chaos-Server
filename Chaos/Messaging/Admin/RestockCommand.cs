using Chaos.Collections.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Services.Other.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("restock", helpText: "<shopName>")]
public class RestockCommand(IStockService stockService) : ICommand<Aisling>
{
    private readonly IStockService StockService = stockService;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var shopName))
            return default;

        StockService.Restock(shopName, 100);

        return default;
    }
}