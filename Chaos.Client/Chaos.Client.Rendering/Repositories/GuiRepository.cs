using System.Collections.Frozen;
using Chaos.Client.Common.Abstractions;
using Chaos.Client.Data;
using Chaos.Client.Data.Models;
using Chaos.Extensions.Client.Common;
using Chaos.Extensions.Common;
using DALib.Drawing;
using DALib.Utility;
using Microsoft.Extensions.Caching.Memory;
using SkiaSharp;

namespace Chaos.Client.Rendering.Repositories;

public class GuiRepository : RepositoryBase
{
    public IDictionary<int, Palette> GuiPalettes { get; }
    public IDictionary<string, UserControlInfo> UserControlInfos { get; }

    /// <inheritdoc />
    public GuiRepository(IMemoryCache cache)
        : base(cache)
    {
        GuiPalettes = Palette.FromArchive("gui", DataContext.Archives.Setoa)
                             .ToFrozenDictionary();

        UserControlInfos = UserControlInfo.FromArchive(DataContext.Archives.Setoa)
                                          .ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }

    public SKImage Get(string imageName, int frameIndex)
    {
        const string ENTRY_PREFIX = "GUI";

        var entryKey = $"{ENTRY_PREFIX}_{imageName}_{frameIndex}";

        var frames = Cache.SafeGetOrCreate(
            entryKey,
            entry =>
            {
                entry.SetPriority(CacheItemPriority.Normal)
                     .SetSlidingExpiration(TimeSpan.FromMinutes(15))
                     .RegisterPostEvictionCallback(HandleDisposableEntries);

                var extension = Path.GetExtension(imageName);

                if (extension.EqualsI(".spf"))
                {
                    var spf = SpfFile.FromArchive(imageName, DataContext.Archives.Setoa);
                    var renderer = spf.Select(frame => Graphics.RenderImage(frame, spf.PrimaryColors));

                    return new SKImageCollection(renderer);
                }

                if (extension.EqualsI(".epf"))
                {
                    var epf = EpfFile.FromArchive(imageName, DataContext.Archives.Setoa);
                    var palette = GuiPalettes[0];
                    var renderer = epf.Select(frame => Graphics.RenderImage(frame, palette));

                    return new SKImageCollection(renderer);
                }

                throw new NotSupportedException($"Image type {extension} is not supported.");
            });

        return frames[frameIndex];
    }
}