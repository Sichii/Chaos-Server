using Chaos.Common.Definitions;

namespace Chaos.Site.Models;

public sealed class SkillDto
{
    public int? AbilityLevel { get; set; }
    public AdvClass? AdvClass { get; set; }
    public BaseClass? Class { get; set; }
    public int? Con { get; set; }
    public TimeSpan? Cooldown { get; set; }
    public string? Description { get; set; }
    public int? Dex { get; set; }
    public int? Gold { get; set; }
    public int? Int { get; set; }
    public string? ItemRequirements { get; set; }
    public int? Level { get; set; }
    public bool LevelsUp { get; set; }
    public int? MaxLevel { get; set; }
    public required string Name { get; set; }
    public bool RequiresMaster { get; set; }
    public string? SkillRequirements { get; set; }
    public string? SpellRequirements { get; set; }
    public int? Sprite { get; set; }
    public int? Str { get; set; }
    internal string TemplateKey { get; set; } = null!;
    public int? Wis { get; set; }
}