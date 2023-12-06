using Chaos.Collections;
using Chaos.Collections.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Storage.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("forceLoadMap", helpText: "<mapInstanceId|all>")]
public class ForceLoadMapCommand(ISimpleCache<MapInstance> mapCache) : ICommand<Aisling>
{
    private readonly ISimpleCache<MapInstance> MapCache = mapCache;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (args.TryGetNext<string>(out var mapInstanceId))
            MapCache.Get(mapInstanceId);
        else
            MapCache.ForceLoad();

        return default;
    }
}