using System.Text.Json;
using Chaos.Site.Extensions;
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
        var columnDefs = typeof(MonsterDto).GetAgGridProperties(nameof(MonsterDto.Name));
        ColumnDefsJson = JsonSerializer.Serialize(columnDefs);
    }

    public void OnGet() { }

    public JsonResult OnPostSearchMonsters([FromBody] GetRowsParams rowParams)
    {
        var query = QueryService.CreateQuery(MonsterDtoRepository, rowParams);

        return new JsonResult(query);
    }
}