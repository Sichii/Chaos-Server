using Microsoft.Extensions.Caching.Memory;

namespace Chaos.Client.Rendering;

public class RenderContext
{
    private static readonly IMemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());

    private RenderContext() { }

    public static RenderContext Instance { get; } = new();
}