using Chaos.Common.Collections;
using Chaos.Containers;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Factories.Abstractions;

public interface IReactorTileFactory
{
    ReactorTile Create(
        MapInstance mapInstance,
        IPoint point,
        bool shouldBlockPathfinding,
        ICollection<string> scriptKeys,
        IDictionary<string, DynamicVars> scriptVars
    );
}