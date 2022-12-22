using Chaos.Common.Abstractions;
using Chaos.Containers;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Services.Factories.Abstractions;

public interface IReactorTileFactory
{
    ReactorTile Create(
        MapInstance mapInstance,
        IPoint point,
        bool shouldBlockPathfinding,
        ICollection<string> scriptKeys,
        IDictionary<string, IScriptVars> scriptVars,
        Creature? owner = null
    );

    ReactorTile Create(
        string templateKey,
        MapInstance mapInstance,
        IPoint point,
        ICollection<string>? extraScriptKeys = null,
        Creature? owner = null
    );
}