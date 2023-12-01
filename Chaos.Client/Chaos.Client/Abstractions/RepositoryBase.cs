using Microsoft.Extensions.Caching.Memory;

namespace Chaos.Client.Abstractions;

public abstract class RepositoryBase
{
    protected IMemoryCache Cache { get; }

    protected RepositoryBase(IMemoryCache cache) => Cache = cache;
}