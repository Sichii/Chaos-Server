using System.Text.Json.Serialization;

namespace Chaos.Schemas.Data;

public sealed record ItemRequirementSchema
{
    public int AmountRequired { get; set; } = 1;
    [JsonRequired]
    public string ItemTemplateKey { get; set; } = null!;
}