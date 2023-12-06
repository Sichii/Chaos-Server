using Microsoft.Extensions.Caching.Memory;

namespace Chaos.Client.Abstractions;

public abstract class RepositoryBase(IMemoryCache cache)
{
    protected IMemoryCache Cache { get; } = cache;
}