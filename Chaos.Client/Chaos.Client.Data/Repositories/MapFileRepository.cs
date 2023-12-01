using Chaos.Client.Common.Abstractions;
using Chaos.Extensions.Client.Common;
using DALib.Data;
using DALib.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Chaos.Client.Data.Repositories;

public class MapFileRepository : RepositoryBase
{
    /// <inheritdoc />
    public MapFileRepository(IMemoryCache cache)
        : base(cache) { }

    public MapFile Get(string key, int width, int height)
    {
        const string ENTRY_PREFIX = "MAPFILE_";
        const string SUBFOLDER = "maps";

        key = key.WithExtension(".map");
        var entryKey = ENTRY_PREFIX + key;

        return Cache.SafeGetOrCreate(
            entryKey,
            entry =>
            {
                entry.SetPriority(CacheItemPriority.Normal)
                     .SetSlidingExpiration(TimeSpan.FromMinutes(15));

                var path = $"{SUBFOLDER}/{key}";

                return MapFile.FromFile(path, width, height);
            });
    }
}