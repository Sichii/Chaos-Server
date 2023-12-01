using Chaos.Client.Data.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Chaos.Client.Data;

public static class DataContext
{
    private static readonly IMemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());

    public static ArchiveRepository Archives { get; }
    public static MapFileRepository MapsFiles { get; }
    public static MetaFileRepository MetaFiles { get; }

    static DataContext()
    {
        Archives = new ArchiveRepository(MemoryCache);
        MapsFiles = new MapFileRepository(MemoryCache);
        MetaFiles = new MetaFileRepository(MemoryCache);
    }
}