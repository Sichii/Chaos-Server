using System.Text.Json.Serialization;

namespace Chaos.Schemas.Data;

public sealed record ItemRequirementSchema
{
    public int AmountRequired { get; init; } = 1;
    [JsonRequired]
    public string ItemTemplateKey { get; init; } = null!;
}