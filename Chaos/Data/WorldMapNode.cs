using Chaos.Containers;
using Chaos.Networking.Entities.Server;
using Chaos.Objects.World;
using Chaos.Storage.Abstractions;

namespace Chaos.Data;

public sealed record WorldMapNode : WorldMapNodeInfo
{
    private readonly ISimpleCache SimpleCache;
    public WorldMapNode(ISimpleCache simpleCache) => SimpleCache = simpleCache;

    public required string NodeKey { get; init; }

    public void OnClick(Aisling aisling)
    {
        var destinationMap = SimpleCache.Get<MapInstance>(Destination.Map);
        destinationMap.AddObject(aisling, Destination);
    }
}