using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Other.Abstractions;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public sealed class MerchantFactory : IMerchantFactory
{
    private readonly IItemFactory ItemFactory;
    private readonly ILoggerFactory LoggerFactory;
    private readonly IScriptProvider ScriptProvider;
    private readonly ISimpleCache SimpleCache;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;
    private readonly IStockService StockService;

    public MerchantFactory(
        ILoggerFactory loggerFactory,
        IScriptProvider scriptProvider,
        ISimpleCache simpleCache,
        IItemFactory itemFactory,
        ISkillFactory skillFactory,
        ISpellFactory spellFactory,
        IStockService stockService)
    {
        LoggerFactory = loggerFactory;
        ScriptProvider = scriptProvider;
        SimpleCache = simpleCache;
        ItemFactory = itemFactory;
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
        StockService = stockService;
    }

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