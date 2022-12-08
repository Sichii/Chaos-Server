using Chaos.Common.Collections;
using Chaos.Containers;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Services.Factories;

public sealed class ReactorTileFactory : IReactorTileFactory
{
    private readonly IScriptProvider ScriptProvider;
    public ReactorTileFactory(IScriptProvider scriptProvider) => ScriptProvider = scriptProvider;

    /// <inheritdoc />
    public ReactorTile Create(
        MapInstance mapInstance,
        IPoint point,
        bool shouldBlockPathfinding,
        ICollection<string> scriptKeys,
        IDictionary<string, DynamicVars> scriptVars
    ) =>
        new(
            mapInstance,
            point,
            shouldBlockPathfinding,
            ScriptProvider,
            scriptKeys,
            scriptVars);
}