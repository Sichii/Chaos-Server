#region
using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Models.World;
using Chaos.Services.Other;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Models.WorldMap;

public sealed record WorldMapNode
{
    private static readonly KeyMapper<ushort> KeyMapper = new WorldMapNodeKeyMapper();
    private readonly ISimpleCache SimpleCache;
    public Location Destination { get; init; }
    private ConcurrentDictionary<uint, DateTime> LastClicked { get; } = new();
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
        if (!ShouldRegisterClick(aisling.Id))
            return;

        LastClicked[aisling.Id] = DateTime.UtcNow;

        var destinationMap = SimpleCache.Get<MapInstance>(Destination.Map);
        aisling.TraverseMap(destinationMap, Destination, fromWorldMap: true);

        aisling.ActiveObject.Set(null);
    }

    public bool ShouldRegisterClick(uint fromId)
        => LastClicked.IsEmpty
           || (DateTime.UtcNow.Subtract(LastClicked.Values.Max())
                       .TotalMilliseconds
               > 1500);
}