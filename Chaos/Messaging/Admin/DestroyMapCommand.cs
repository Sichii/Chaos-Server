#region
using Chaos.Collections;
using Chaos.Collections.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Messaging.Admin;

[Command("destroyMap", helpText: "<mapInstanceId>")]
public class DestroyMapCommand : ICommand<Aisling>
{
    private readonly ISimpleCache Cache;
    public DestroyMapCommand(ISimpleCache cache) => Cache = cache;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var mapInstanceId))
            return default;

        var mapInstance = Cache.Get<MapInstance>(mapInstanceId);
        mapInstance.Destroy();

        return default;
    }
}