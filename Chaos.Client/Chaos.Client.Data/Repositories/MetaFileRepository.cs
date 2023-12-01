using Chaos.Client.Common.Abstractions;
using Chaos.Extensions.Client.Common;
using DALib.Data;
using Microsoft.Extensions.Caching.Memory;

namespace Chaos.Client.Data.Repositories;

public class MetaFileRepository : RepositoryBase
{
    /// <inheritdoc />
    public MetaFileRepository(IMemoryCache cache)
        : base(cache) { }

    public MetaFile Get(string name)
    {
        const string ENTRY_PREFIX = "METAFILE_";
        const string SUBFOLDER = "metafile";

        var entryKey = ENTRY_PREFIX + name;

        return Cache.SafeGetOrCreate(
            entryKey,
            entry =>
            {
                entry.SetPriority(CacheItemPriority.Normal)
                     .SetSlidingExpiration(TimeSpan.FromMinutes(15));

                var path = $"{SUBFOLDER}/{name}";

                return MetaFile.FromFile(path, true);
            });
    }
}