using System.Text.Json;
using Chaos.Site.Extensions;
using Chaos.Site.Models;
using Chaos.Site.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chaos.Site.Pages;

[IgnoreAntiforgeryToken]
public class Skills(SkillDtoRepository skillDtoRepository, QueryService queryService) : PageModel
{
    private readonly QueryService QueryService = queryService;
    private readonly SkillDtoRepository SkillDtoRepository = skillDtoRepository;
    public static string ColumnDefsJson { get; private set; }

    static Skills()
    {
        var columnDefs = typeof(SkillDto).GetAgGridProperties(nameof(SkillDto.Name));
        ColumnDefsJson = JsonSerializer.Serialize(columnDefs);
    }

    public void OnGet() { }

    public JsonResult OnPostSearchSkills([FromBody] GetRowsParams rowParams)
    {
        var query = QueryService.CreateQuery(SkillDtoRepository, rowParams);

        return new JsonResult(query);
    }
}