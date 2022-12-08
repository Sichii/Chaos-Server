using Chaos.Geometry;

namespace ReactorTilePatch;

public sealed class ReactorTile
{
    public required List<string> ScriptKeys { get; set; }
    public required Dictionary<string, Dictionary<string, string>> ScriptVars { get; set; }
    public required bool ShouldBlockPathfinding { get; set; }
    public required Point Source { get; set; }

    public static ReactorTile FromWarp(WarpTile warpTile) => new()
    {
        Source = warpTile.Source,
        ScriptKeys = new List<string> { "warp" },
        ShouldBlockPathfinding = true,
        ScriptVars = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "warp",
                new Dictionary<string, string>
                {
                    { "destination", warpTile.Destination.ToString() }
                }
            }
        }
    };

    public static ReactorTile FromWorldMap(WorldMapTile worldMapTile) => new()
    {
        Source = worldMapTile.Source,
        ScriptKeys = new List<string> { "showWorldMap" },
        ShouldBlockPathfinding = true,
        ScriptVars = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "showWorldMap",
                new Dictionary<string, string>
                {
                    { "worldMapKey", worldMapTile.WorldMapKey }
                }
            }
        }
    };
}