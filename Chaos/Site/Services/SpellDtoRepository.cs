using AutoMapper;
using Chaos.Models.Templates;
using Chaos.Site.Models;
using Chaos.Storage.Abstractions;

namespace Chaos.Site.Services;

public class SpellDtoRepository : IEnumerable<SpellDto>
{
    private List<SpellDto> SpellDtos { get; }

    public SpellDtoRepository(ISimpleCache<SpellTemplate> spellTemplateCache, IMapper mapper)
    {
        spellTemplateCache.ForceLoad();
        var stuff = new List<SpellDto>();

        foreach (var obj in spellTemplateCache)
        {
            var mapped = mapper.Map<SpellDto>(obj);
            stuff.Add(mapped);
        }

        SpellDtos = stuff;

        //SpellDtos = mapper.Map<IEnumerable<SpellDto>>(spellTemplateCache)
        //                  .ToList();
    }

    /// <inheritdoc />
    public IEnumerator<SpellDto> GetEnumerator() => SpellDtos.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}