using Chaos.Containers;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public sealed class MerchantFactory : IMerchantFactory
{
    private readonly ILogger<MerchantFactory> Logger;
    private readonly ILoggerFactory LoggerFactory;
    private readonly IScriptProvider ScriptProvider;
    private readonly ISimpleCache SimpleCache;

    public MerchantFactory(
        ILogger<MerchantFactory> logger,
        ILoggerFactory loggerFactory,
        IScriptProvider scriptProvider,
        ISimpleCache simpleCache
    )
    {
        Logger = logger;
        LoggerFactory = loggerFactory;
        ScriptProvider = scriptProvider;
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public Merchant Create(
        string templateKey,
        MapInstance mapInstance,
        IPoint point,
        ICollection<string>? extraScriptKeys = null
    )
    {
        extraScriptKeys ??= Array.Empty<string>();
        var template = SimpleCache.Get<MerchantTemplate>(templateKey);
        var logger = LoggerFactory.CreateLogger<Merchant>();

        var merchant = new Merchant(
            template,
            mapInstance,
            point,
            logger,
            ScriptProvider,
            extraScriptKeys);

        Logger.LogTrace("Created merchant {Merchant}", merchant);

        return merchant;
    }
}