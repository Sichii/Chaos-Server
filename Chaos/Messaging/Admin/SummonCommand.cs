using Chaos.Collections;
using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Storage.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("summon", helpText: "<targetName>")]
public class SummonCommand : ICommand<Aisling>
{
    private readonly ISimpleCacheProvider CacheProvider;
    public SummonCommand(ISimpleCacheProvider cacheProvider) => CacheProvider = cacheProvider;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var name))
            return default;

        var mapCache = CacheProvider.GetCache<MapInstance>();

        var aisling = mapCache.SelectMany(map => map.GetEntities<Aisling>())
                              .FirstOrDefault(aisling => aisling.Name.EqualsI(name));

        if (aisling == null)
            source.SendOrangeBarMessage($"{name} is not online");
        else
            aisling.TraverseMap(source.MapInstance, source, true);

        return default;
    }
}