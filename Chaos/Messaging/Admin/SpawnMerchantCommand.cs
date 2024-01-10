using Chaos.Collections.Common;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("spawnMerchant", helpText: "<templateKey> <direction?>")]
public class SpawnMerchantCommand(IMerchantFactory merchantFactory) : ICommand<Aisling>
{
    private readonly IMerchantFactory MerchantFactory = merchantFactory;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var merchantTemplateKey))
            return default;

        var merchant = MerchantFactory.Create(merchantTemplateKey, source.MapInstance, source);

        if (args.TryGetNext<Direction>(out var direction))
            merchant.Direction = direction;

        source.MapInstance.AddEntity(merchant, source);

        return default;
    }
}