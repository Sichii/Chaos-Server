using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Site.Definitions;

namespace Chaos.Site.Models;

public class FilterModel
{
    public FilterModel[]? Conditions { get; set; }

    [JsonPropertyName("filterTo")]
    public JsonElement? FilterToValue { get; set; }

    public FilterType FilterType { get; set; }

    [JsonPropertyName("filter")]
    public JsonElement? FilterValue { get; set; }

    [JsonPropertyName("type")]
    public FilterOperator Operation { get; set; }

    public FilterGroupOperator? Operator { get; set; }
}