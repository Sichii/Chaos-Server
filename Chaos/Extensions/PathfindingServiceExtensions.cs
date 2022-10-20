using Chaos.Containers;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;

namespace Chaos.Extensions;

public static class PathfindingServiceExtensions
{
    public static void RegisterGrid(this IPathfindingService pathfindingService, MapInstance mapInstance)
    {
        var walls = new List<IPoint>();

        walls.AddRange(mapInstance.GetEntities<WarpTile>());
        walls.AddRange(mapInstance.GetEntities<WorldMapTile>());

        //tiles that are walls are added to pathfinder grid details
        for (var x = 0; x < mapInstance.Template.Width; x++)
            for (var y = 0; y < mapInstance.Template.Height; y++)
                if (mapInstance.Template.Tiles[x, y].IsWall)
                    walls.Add(new Point(x, y));

        pathfindingService.RegisterGrid(
            mapInstance.InstanceId,
            new GridDetails
            {
                Width = mapInstance.Template.Width,
                Height = mapInstance.Template.Height,
                Walls = walls
            });
    }
}