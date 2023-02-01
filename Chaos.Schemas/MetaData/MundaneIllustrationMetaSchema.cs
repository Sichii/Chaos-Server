using System.Text.Json.Serialization;

namespace Chaos.Schemas.MetaData;

public record MundaneIllustrationMetaSchema
{
    [JsonRequired]
    public string ImageName { get; set; } = null!;
    [JsonRequired]
    public string Name { get; set; } = null!;
}