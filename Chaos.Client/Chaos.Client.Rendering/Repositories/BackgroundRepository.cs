using Chaos.Client.Common.Abstractions;
using Chaos.Client.Data;
using Chaos.Extensions.Client.Common;
using DALib.Drawing;
using Microsoft.Extensions.Caching.Memory;
using SkiaSharp;

namespace Chaos.Client.Rendering.Repositories;

public class BackgroundRepository : RepositoryBase
{
    private readonly PaletteLookup BgPaletteLookup;
    private bool SnowyTileset;
    private Tileset Tileset;

    /// <inheritdoc />
    public BackgroundRepository(IMemoryCache cache)
        : base(cache)
    {
        BgPaletteLookup = PaletteLookup.FromArchive("mpt", DataContext.Archives.Seo);
        Tileset = Tileset.FromArchive("tilea", DataContext.Archives.Seo);
    }

    public SKImage Get(int id)
    {
        const string ENTRY_PREFIX = "BG_TILE_";
        var suffix = SnowyTileset ? "_SNOWY" : string.Empty;

        var entryKey = ENTRY_PREFIX + id + suffix;

        return Cache.SafeGetOrCreate(
            entryKey,
            entry =>
            {
                entry.SetPriority(CacheItemPriority.Normal)
                     .SetSlidingExpiration(TimeSpan.FromMinutes(15));

                id--;

                var palette = BgPaletteLookup.GetPaletteForId(id + 2);

                return Graphics.RenderTile(Tileset[id], palette);
            });
    }

    public void ToggleSnowTileset()
    {
        SnowyTileset = !SnowyTileset;

        Tileset = Tileset.FromArchive(SnowyTileset ? "tileas" : "tilea", DataContext.Archives.Seo);
    }
}