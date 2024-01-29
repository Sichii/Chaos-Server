using System.Text.Json;
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
        var columnDefs = typeof(SpellDto).GetProperties()
                                         .Select(
                                             prop =>
                                             {
                                                 var propType = prop.PropertyType;

                                                 if (Nullable.GetUnderlyingType(propType) is { } underlyingType)
                                                     propType = underlyingType;

                                                 var identifier = prop.Name == nameof(SpellDto.Name);

                                                 return new
                                                 {
                                                     headerName = prop.Name,
                                                     field = prop.Name.ToLower(),
                                                     filter = propType is { IsValueType: true, IsEnum: false }
                                                         ? "agNumberColumnFilter"
                                                         : "agTextColumnFilter",
                                                     pinned = identifier ? "left" : null
                                                 };
                                             });

        ColumnDefsJson = JsonSerializer.Serialize(columnDefs);
    }

    public void OnGet() { }

    public JsonResult OnPostSpellPage([FromBody] GetRowsParams rowParams)
    {
        var query = QueryService.CreateQuery(SpellDtoRepository, rowParams);

        return new JsonResult(query);
    }
}