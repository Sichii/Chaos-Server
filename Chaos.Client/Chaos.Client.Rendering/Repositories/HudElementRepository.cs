using Chaos.Client.Common.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Chaos.Client.Rendering.Repositories;

public class HudElementRepository : RepositoryBase
{
    /// <inheritdoc />
    public HudElementRepository(IMemoryCache cache)
        : base(cache) { }
}