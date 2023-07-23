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
public enum TargetFilter : ulong
{
    None,
    FriendlyOnly = 1,
    HostileOnly = 1 << 1,
    NeutralOnly = 1 << 2,
    NonFriendlyOnly = 1 << 3,
    NonHostileOnly = 1 << 4,
    NonNeutralOnly = 1 << 5,
    AliveOnly = 1 << 6,
    DeadOnly = 1 << 7,
    AislingsOnly = 1 << 8,
    MonstersOnly = 1 << 9,
    MerchantsOnly = 1 << 10,
    NonAislingsOnly = 1 << 11,
    NonMonstersOnly = 1 << 12,
    NonMerchantsOnly = 1 << 13,
    SelfOnly = 1 << 14,
    OthersOnly = 1 << 15,
    GroupOnly = 1 << 16
}

public enum VisibilityType
{
    Normal,
    Hidden,
    TrueHidden,
    GmHidden
}