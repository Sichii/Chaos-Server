using Chaos.Containers;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Interfaces;

namespace Chaos.Extensions;

public static class PathfindingServiceExtensions
{
    public static void RegisterGrid(this IPathfindingService pathfindingService, MapInstance mapInstance)
    {
        var walls = new List<Point>();

        walls.AddRange(
            mapInstance.WarpGroups
                       .Flatten()
                       .Select(warp => Point.From(warp.SourceLocation!)));

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