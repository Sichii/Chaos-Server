using System.Text.Json;
using AutoMapper;
using Chaos.Site.Models;
using Chaos.Site.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chaos.Site.Pages;

[IgnoreAntiforgeryToken]
public class Skills(SkillDtoRepository skillDtoRepository, QueryService queryService, IMapper mapper) : PageModel
{
    private readonly IMapper Mapper = mapper;
    private readonly QueryService QueryService = queryService;
    private readonly SkillDtoRepository SkillDtoRepository = skillDtoRepository;
    public static string ColumnDefsJson { get; private set; }

    static Skills()
    {
        var columnDefs = typeof(SkillDto).GetProperties()
                                         .Select(
                                             prop =>
                                             {
                                                 var propType = prop.PropertyType;

                                                 if (Nullable.GetUnderlyingType(propType) is { } underlyingType)
                                                     propType = underlyingType;

                                                 var identifier = prop.Name == nameof(SkillDto.Name);

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

    public JsonResult OnPostSkillPage([FromBody] GetRowsParams rowParams)
    {
        var query = QueryService.CreateQuery(SkillDtoRepository, rowParams);
        var ret = Mapper.Map<IEnumerable<SkillDto>>(query);

        return new JsonResult(ret);
    }
}