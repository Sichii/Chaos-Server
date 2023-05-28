namespace Chaos.Definitions;

public enum AoeShape
{
    None,
    Front,
    AllAround,
    FrontalCone,
    FrontalDiamond
}

[Flags]
public enum TargetFilter
{
    None,
    FriendlyOnly = 1,
    HostileOnly = 1 << 1,
    AliveOnly = 1 << 2,
    DeadOnly = 1 << 3,
    AislingsOnly = 1 << 4,
    MonstersOnly = 1 << 5,
    SelfOnly = 1 << 6,
    GroupOnly = 1 << 7
}

public enum VisibilityType
{
    Normal,
    Hidden,
    TrueHidden,
    GmHidden
}