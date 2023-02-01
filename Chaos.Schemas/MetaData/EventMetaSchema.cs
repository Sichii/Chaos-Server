using System.Text.Json.Serialization;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.MetaData;

public sealed record EventMetaSchema
{
    [JsonRequired]
    public LevelCircle Circle { get; set; }
    [JsonRequired]
    public string Id { get; set; } = null!;
    [JsonRequired]
    public string PrerequisiteEventId { get; set; } = null!;
    [JsonRequired]
    public ICollection<BaseClass> QualifyingClasses { get; set; } = new[]
        { BaseClass.Peasant, BaseClass.Warrior, BaseClass.Rogue, BaseClass.Wizard, BaseClass.Priest, BaseClass.Monk };
    [JsonRequired]
    public string Result { get; set; } = null!;
    [JsonRequired]
    public string Rewards { get; set; } = null!;
    [JsonRequired]
    public string Summary { get; set; } = null!;
    [JsonRequired]
    public string Title { get; set; } = null!;
}