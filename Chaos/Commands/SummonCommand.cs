using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Extensions.Common;
using Chaos.Objects.World;
using Chaos.Storage.Abstractions;

namespace Chaos.Commands;

[Command("summon")]
public class SummonCommand : ICommand<Aisling>
{
    private readonly ISimpleCacheProvider CacheProvider;
    public SummonCommand(ISimpleCacheProvider cacheProvider) => CacheProvider = cacheProvider;

    /// <inheritdoc />
    public void Execute(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGet<string>(0, out var name))
            return;

        var mapCache = CacheProvider.GetCache<MapInstance>();

        var aisling = mapCache.SelectMany(map => map.GetEntities<Aisling>())
                              .FirstOrDefault(aisling => aisling.Name.EqualsI(name));

        if (aisling == null)
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{name} is not online");
        else
            aisling.TraverseMap(source.MapInstance, source);
    }
}