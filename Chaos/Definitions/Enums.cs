namespace Chaos.Definitions;

public enum MoveType
{
    Walk,
    Warp
}

[Flags]
public enum Quest : ulong
{
    None = 0,
    MaribelRobes = 1
    //add more quest flags here, double each time
}

[Flags]
public enum Status : ulong
{
    None = 0,
    Dead = 1
    //add more statuses here, double each time
}

[Flags]
public enum UserState : ulong
{
    None = 0,
    IsChanting = 1,
    Exchanging = 2
    //add more user states here, double each time
}

[Flags]
public enum MapFlags : ulong
{
    None = 0,
    Hostile = 1,
    NonHostile = 2,
    NoSpells = 4,
    NoSkills = 8,
    NoChat = 16,
    Snowing = 32,
    PvP = 64
    //and whatever else we decide
}

public enum PursuitId : ushort
{
    None = 0,
    ReviveSelf = 1,
    ReviveUser = 2,
    Teleport = 3,
    SummonUser = 4,
    SummonAll = 5,
    KillUser = 6,
    LouresCitizenship = 7,
    BecomeWarrior = 8,
    BecomeWizard = 9,
    BecomePriest = 10,
    BecomeMonk = 11,
    BecomeRogue = 12,
    GiveTatteredRobe = 13,
    ForceGive = 14
}

public enum ReactorTileType
{
    Walk = 0,
    DropMoney = 1,
    DropItem = 2
}

public enum TargetsType : byte
{
    //skills(usually)
    None = 0,
    Self = 1,
    Front = 2,
    Surround = 3,
    Cleave = 4,
    StraightProjectile = 5,

    //spells(usually)
    Cluster1 = 252,
    Cluster2 = 253,
    Cluster3 = 254,
    Screen = 255
}