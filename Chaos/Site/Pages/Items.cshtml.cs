using System.Text.Json;
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
        var columnDefs = typeof(ItemDto).GetProperties()
                                        .Select(
                                            prop =>
                                            {
                                                var propType = prop.PropertyType;

                                                if (Nullable.GetUnderlyingType(propType) is { } underlyingType)
                                                    propType = underlyingType;

                                                var identifier = prop.Name == nameof(ItemDto.Name);

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

    public JsonResult OnPostItemPage([FromBody] GetRowsParams rowParams)
    {
        var query = QueryService.CreateQuery(ItemDtoRepository, rowParams);

        return new JsonResult(query);
    }
}