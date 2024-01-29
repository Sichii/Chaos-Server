using System.Text.Json;
using Chaos.Site.Models;
using Chaos.Site.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chaos.Site.Pages;

[IgnoreAntiforgeryToken]
public class Monsters(MonsterDtoRepository monsterDtoRepository, QueryService queryService) : PageModel
{
    private readonly MonsterDtoRepository MonsterDtoRepository = monsterDtoRepository;
    private readonly QueryService QueryService = queryService;
    public static string ColumnDefsJson { get; private set; }

    static Monsters()
    {
        var columnDefs = typeof(MonsterDto).GetProperties()
                                           .Select(
                                               prop =>
                                               {
                                                   var propType = prop.PropertyType;

                                                   if (Nullable.GetUnderlyingType(propType) is { } underlyingType)
                                                       propType = underlyingType;

                                                   var identifier = prop.Name == nameof(MonsterDto.Name);

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

    public JsonResult OnPostMonsterPage([FromBody] GetRowsParams rowParams)
    {
        var query = QueryService.CreateQuery(MonsterDtoRepository, rowParams);

        return new JsonResult(query);
    }
}