using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;

namespace Chaos.Services.Factories.Abstractions;

public interface IMerchantFactory
{
    public Merchant Create(
        string templateKey,
        MapInstance mapInstance,
        IPoint point,
        ICollection<string>? extraScriptKeys = null);
}