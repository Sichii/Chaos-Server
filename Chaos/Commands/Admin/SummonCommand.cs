using Chaos.Collections.Common;
using Chaos.Containers;
using Chaos.Extensions.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Objects.World;
using Chaos.Storage.Abstractions;

namespace Chaos.Commands.Admin;

[Command("summon")]
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