using Chaos.Client.Common.Abstractions;
using Chaos.Client.Data;
using Chaos.Extensions.Client.Common;
using DALib.Drawing;
using Microsoft.Extensions.Caching.Memory;
using SkiaSharp;

namespace Chaos.Client.Rendering.Repositories;

public class ForegroundRepository : RepositoryBase
{
    private readonly PaletteLookup FgPaletteLookup;

    /// <inheritdoc />
    public ForegroundRepository(IMemoryCache cache)
        : base(cache)
        => FgPaletteLookup = PaletteLookup.FromArchive("stc", DataContext.Archives.Ia);

    public SKImage Get(int id)
    {
        const string ENTRY_PREFIX = "FG_TILE_";

        var entryKey = ENTRY_PREFIX + id;

        return Cache.SafeGetOrCreate(
            entryKey,
            entry =>
            {
                entry.SetPriority(CacheItemPriority.Normal)
                     .SetSlidingExpiration(TimeSpan.FromMinutes(15));

                var palette = FgPaletteLookup.GetPaletteForId(id);
                var hpf = HpfFile.FromArchive($"stc{id:D5}.hpf", DataContext.Archives.Ia);

                return Graphics.RenderImage(hpf, palette);
            });
    }
}