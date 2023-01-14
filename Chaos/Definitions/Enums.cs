namespace Chaos.Definitions;

public enum AoeShape
{
    None,
    Front,
    AllAround,
    FrontalCone,
    FrontalDiamond
}

public enum TargetFilter
{
    None,
    FriendlyOnly,
    HostileOnly
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