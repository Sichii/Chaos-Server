#region
using Chaos.Collections;
using Chaos.Collections.Common;
using Chaos.Common.Abstractions;
using Chaos.Geometry;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Other.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockMerchant
{
    private static int Counter;

    public static Merchant Create(MapInstance? mapInstance = null, string? name = null, Action<Merchant>? setup = null)
    {
        mapInstance ??= MockMapInstance.Create();
        name ??= $"TestMerchant{Interlocked.Increment(ref Counter)}";

        var template = new MerchantTemplate
        {
            Name = name,
            TemplateKey = name.ToLowerInvariant(),
            Sprite = 1,
            WanderIntervalMs = 1000,
            DefaultStock = new Dictionary<string, int>(),
            ItemsForSale = new CounterCollection(),
            ItemsToBuy = [],
            SkillsToTeach = [],
            SpellsToTeach = [],
            RestockIntervalHrs = 1,
            RestockPct = 100,
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase)
        };

        var loggerMock = new Mock<ILogger<Merchant>>();
        var skillFactoryMock = new Mock<ISkillFactory>();
        var spellFactoryMock = new Mock<ISpellFactory>();
        var itemFactoryMock = new Mock<IItemFactory>();
        var stockServiceMock = new Mock<IStockService>();

        var merchant = new Merchant(
            template,
            mapInstance,
            new Point(5, 5),
            loggerMock.Object,
            skillFactoryMock.Object,
            spellFactoryMock.Object,
            itemFactoryMock.Object,
            stockServiceMock.Object,
            MockScriptProvider.Instance.Object);

        setup?.Invoke(merchant);

        return merchant;
    }
}