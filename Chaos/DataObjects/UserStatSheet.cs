using Chaos.Core.Definitions;

namespace Chaos.DataObjects;

public record UserStatSheet : StatSheet
{
    public AdvClass AdvClass { get; set; } = AdvClass.None;
    public BaseClass BaseClass { get; set; } = BaseClass.Peasant;
    public int CurrentWeight { get; set; } = 0;
    public bool Master { get; set; }
    public int MaxWeight { get; set; } = 50;
    public Nation Nation { get; set; } = Nation.None;
    public int ToNextAbility { get; set; }
    public int ToNextLevel { get; set; } = 100;
    public int TotalAbility { get; set; }
    public int TotalExp { get; set; }
    public int UnspentPoints { get; set; }
}