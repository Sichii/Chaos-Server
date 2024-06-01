using System.Text.Json;
using Chaos.Site.Extensions;
using Chaos.Site.Models;
using Chaos.Site.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chaos.Site.Pages;

[IgnoreAntiforgeryToken]
public class Spells(SpellDtoRepository spellDtoRepository, QueryService queryService) : PageModel
{
    private readonly QueryService QueryService = queryService;
    private readonly SpellDtoRepository SpellDtoRepository = spellDtoRepository;
    public static string ColumnDefsJson { get; private set; }

    static Spells()
    {
        var columnDefs = typeof(SpellDto).GetAgGridProperties(nameof(SpellDto.Name));
        ColumnDefsJson = JsonSerializer.Serialize(columnDefs);
    }

    public void OnGet() { }

    public JsonResult OnPostSearchSpells([FromBody] GetRowsParams rowParams)
    {
        var query = QueryService.CreateQuery(SpellDtoRepository, rowParams);

        return new JsonResult(query);
    }
}