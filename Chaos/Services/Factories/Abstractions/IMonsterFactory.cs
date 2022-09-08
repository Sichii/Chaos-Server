using Chaos.Containers;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Services.Factories.Abstractions;

public interface IMonsterFactory
{
    public Monster Create(
        string templateKey,
        MapInstance mapInstance,
        IPoint point,
        ICollection<string>? extraScriptKeys = null
    );
}