using AutoMapper;
using Chaos.Models.Templates;
using Chaos.Site.Models;
using Chaos.Storage.Abstractions;

namespace Chaos.Site.Services;

public sealed class MonsterDtoRepository : IEnumerable<MonsterDto>
{
    private List<MonsterDto> MonsterDtos { get; }

    public MonsterDtoRepository(ISimpleCache<MonsterTemplate> monsterTemplateCache, IMapper mapper)
    {
        monsterTemplateCache.ForceLoad();

        MonsterDtos = mapper.Map<IEnumerable<MonsterDto>>(monsterTemplateCache)
                            .ToList();
    }

    /// <inheritdoc />
    public IEnumerator<MonsterDto> GetEnumerator() => MonsterDtos.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}