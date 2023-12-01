using Chaos.Client.Rendering.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Chaos.Client.Rendering;

public static class RenderContext
{
    private static readonly IMemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());

    public static BackgroundRepository BackgroundRepository { get; }
    public static ForegroundRepository ForegroundRepository { get; }

    static RenderContext()
    {
        BackgroundRepository = new BackgroundRepository(MemoryCache);
        ForegroundRepository = new ForegroundRepository(MemoryCache);
    }
}