using Chaos.Collections;
using Chaos.Common.Identity;
using Chaos.Common.Utilities;
using Chaos.Models.World;
using Chaos.Storage.Abstractions;

namespace Chaos.Models.WorldMap;

public sealed record WorldMapNode
{
    private static readonly KeyMapper<ushort> KeyMapper = new(new SequentialIdGenerator<ushort>());
    private readonly ISimpleCache SimpleCache;
    public Location Destination { get; init; }
    public string NodeKey { get; init; }
    public Point ScreenPosition { get; init; }
    public string Text { get; init; }
    public ushort UniqueId { get; init; }

    public WorldMapNode(
        ISimpleCache simpleCache,
        string nodeKey,
        Location destination,
        Point screenPosition,
        string text)
    {
        SimpleCache = simpleCache;
        Destination = destination;
        NodeKey = nodeKey;
        UniqueId = KeyMapper.GetId(nodeKey);
        ScreenPosition = screenPosition;
        Text = text;
    }

    public void OnClick(Aisling aisling)
    {
        var destinationMap = SimpleCache.Get<MapInstance>(Destination.Map);
        aisling.TraverseMap(destinationMap, Destination, fromWolrdMap: true);

        aisling.ActiveObject.Set(null);
    }
}