using AutoMapper;
using Chaos.Models.Templates;
using Chaos.Site.Models;
using Chaos.Storage.Abstractions;

namespace Chaos.Site.Services;

public class ItemDtoRepository : IEnumerable<ItemDto>
{
    private List<ItemDto> ItemDtos { get; }

    public ItemDtoRepository(ISimpleCache<ItemTemplate> itemTemplateCache, IMapper mapper)
    {
        itemTemplateCache.ForceLoad();

        ItemDtos = mapper.Map<IEnumerable<ItemDto>>(itemTemplateCache)
                         .ToList();
    }

    /// <inheritdoc />
    public IEnumerator<ItemDto> GetEnumerator() => ItemDtos.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}