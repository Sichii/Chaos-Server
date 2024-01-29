using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Other.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Services.Factories;

public sealed class MerchantFactory(
    ILoggerFactory loggerFactory,
    IScriptProvider scriptProvider,
    ISimpleCache simpleCache,
    IItemFactory itemFactory,
    ISkillFactory skillFactory,
    ISpellFactory spellFactory,
    IStockService stockService) : IMerchantFactory
{
    private readonly IItemFactory ItemFactory = itemFactory;
    private readonly ILoggerFactory LoggerFactory = loggerFactory;
    private readonly IScriptProvider ScriptProvider = scriptProvider;
    private readonly ISimpleCache SimpleCache = simpleCache;
    private readonly ISkillFactory SkillFactory = skillFactory;
    private readonly ISpellFactory SpellFactory = spellFactory;
    private readonly IStockService StockService = stockService;

    /// <inheritdoc />
    public Merchant Create(
        string templateKey,
        MapInstance mapInstance,
        IPoint point,
        ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= Array.Empty<string>();
        var template = SimpleCache.Get<MerchantTemplate>(templateKey);
        var logger = LoggerFactory.CreateLogger<Merchant>();

        var merchant = new Merchant(
            template,
            mapInstance,
            point,
            logger,
            SkillFactory,
            SpellFactory,
            ItemFactory,
            StockService,
            ScriptProvider,
            extraScriptKeys);

        return merchant;
    }
}