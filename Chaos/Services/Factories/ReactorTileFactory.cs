using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Services.Factories;

public sealed class ReactorTileFactory : IReactorTileFactory
{
    private readonly ISimpleCache Cache;
    private readonly IScriptProvider ScriptProvider;

    public ReactorTileFactory(IScriptProvider scriptProvider, ISimpleCache cache)
    {
        ScriptProvider = scriptProvider;
        Cache = cache;
    }

    /// <inheritdoc />
    public ReactorTile Create(
        MapInstance mapInstance,
        IPoint point,
        bool shouldBlockPathfinding,
        ICollection<string> scriptKeys,
        IDictionary<string, IScriptVars> scriptVars,
        Creature? owner = null
    ) =>
        new(
            mapInstance,
            point,
            shouldBlockPathfinding,
            ScriptProvider,
            scriptKeys,
            scriptVars,
            owner);

    /// <inheritdoc />
    public ReactorTile Create(
        string templateKey,
        MapInstance mapInstance,
        IPoint point,
        ICollection<string>? extraScriptKeys = null,
        Creature? owner = null
    )
    {
        extraScriptKeys ??= Array.Empty<string>();
        var template = Cache.Get<ReactorTileTemplate>(templateKey);

        return new TemplatedReactorTile(
            template,
            mapInstance,
            point,
            ScriptProvider,
            extraScriptKeys,
            owner);
    }
}