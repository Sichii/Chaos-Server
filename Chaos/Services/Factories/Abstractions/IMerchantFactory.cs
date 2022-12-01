using Chaos.Containers;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Services.Factories.Abstractions;

public interface IMerchantFactory
{
    public Merchant Create(
        string templateKey,
        MapInstance mapInstance,
        IPoint point,
        ICollection<string>? extraScriptKeys = null
    );
}