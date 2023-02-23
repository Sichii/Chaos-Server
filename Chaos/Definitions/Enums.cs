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
    MonstersOnly = 1 << 5
}

[Flags]
public enum QuestFlag1 : ulong
{
    None = 0
    //add more quest flags here, double each time
}

[Flags]
public enum QuestFlag2 : ulong { }

[Flags]
public enum QuestFlag3 : ulong { }