using System.Text.Json;
using Chaos.Site.Extensions;
using Chaos.Site.Models;
using Chaos.Site.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chaos.Site.Pages;

[IgnoreAntiforgeryToken]
public class Items(ItemDtoRepository itemDtoRepository, QueryService queryService) : PageModel
{
    private readonly ItemDtoRepository ItemDtoRepository = itemDtoRepository;
    private readonly QueryService QueryService = queryService;
    public static string ColumnDefsJson { get; private set; }

    static Items()
    {
        var columnDefs = typeof(ItemDto).GetAgGridProperties(nameof(ItemDto.Name));
        ColumnDefsJson = JsonSerializer.Serialize(columnDefs);
    }

    public void OnGet() { }

    public JsonResult OnPostSearchItems([FromBody] GetRowsParams rowParams)
    {
        var query = QueryService.CreateQuery(ItemDtoRepository, rowParams);

        return new JsonResult(query);
    }
}