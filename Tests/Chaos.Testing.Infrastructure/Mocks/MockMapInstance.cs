#region
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Map;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Services.Other.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockMapInstance
{
    public static MapInstance Create(
        string templateKey = "test_map",
        string name = "Test Map",
        byte width = 10,
        byte height = 10,
        Action<MapInstance>? setup = null)
    {
        var template = new MapTemplate
        {
            TemplateKey = templateKey,
            Width = width,
            Height = height,
            Bounds = new Rectangle(
                0,
                0,
                width,
                height),
            Tiles = new Tile[width, height],
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        };

        var simpleCacheMock = new Mock<ISimpleCache>();
        var shardGeneratorMock = new Mock<IShardGenerator>();
        var mapTraversalServiceMock = new Mock<IMapTraversalService>();
        var aislingStoreMock = new Mock<IAsyncStore<Aisling>>();
        var loggerMock = new Mock<ILogger<MapInstance>>();
        var serverCtx = new CancellationTokenSource();

        var mapInstance = new MapInstance(
            template,
            simpleCacheMock.Object,
            mapTraversalServiceMock.Object,
            MockScriptProvider.Instance.Object,
            name,
            $"{templateKey}1",
            aislingStoreMock.Object,
            serverCtx,
            loggerMock.Object);

        setup?.Invoke(mapInstance);

        return mapInstance;
    }

    /// <summary>
    ///     Sets a tile on the map to be a wall using the first wall-flagged foreground in the Sotp table
    /// </summary>
    public static void SetWall(MapInstance map, IPoint point)
    {
        var wallIndex = Array.FindIndex(Tile.Sotp, f => f.HasFlag(TileFlags.Wall));

        if (wallIndex < 0)
            throw new InvalidOperationException("No wall tile found in Sotp resource");

        map.Template.Tiles[point.X, point.Y] = new Tile(0, (ushort)(wallIndex + 1), 0);
    }
}