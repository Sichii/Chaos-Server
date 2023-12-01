using Chaos.Client.Common.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Chaos.Client.Data.Repositories;

public class CharacterConfigRepository : RepositoryBase
{
    /// <inheritdoc />
    public CharacterConfigRepository(IMemoryCache cache)
        : base(cache) { }
}