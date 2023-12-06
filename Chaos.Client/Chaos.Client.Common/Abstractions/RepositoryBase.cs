using Microsoft.Extensions.Caching.Memory;

namespace Chaos.Client.Common.Abstractions;

public abstract class RepositoryBase(IMemoryCache cache)
{
    protected IMemoryCache Cache { get; } = cache;

    protected void HandleDisposableEntries(
        object key,
        object? value,
        EvictionReason reason,
        object? state)
    {
        if (value is IDisposable disposable)
            disposable.Dispose();
    }
}