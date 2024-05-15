using AutoMapper;
using Chaos.Models.Templates;
using Chaos.Site.Models;
using Chaos.Storage.Abstractions;

namespace Chaos.Site.Services;

public sealed class SkillDtoRepository : IEnumerable<SkillDto>
{
    private List<SkillDto> SkillDtos { get; }

    public SkillDtoRepository(ISimpleCache<SkillTemplate> skillTemplateCache, IMapper mapper)
    {
        skillTemplateCache.ForceLoad();

        SkillDtos = mapper.Map<IEnumerable<SkillDto>>(skillTemplateCache)!.ToList();
    }

    /// <inheritdoc />
    public IEnumerator<SkillDto> GetEnumerator() => SkillDtos.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}