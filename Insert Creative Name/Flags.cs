using System;

namespace Insert_Creative_Name
{
    [Flags]
    internal enum EncryptMethod
    {
        None,
        Normal,
        MD5Key,
    }
    [Flags]
    internal enum WaitEventResult
    {
        Signaled = 0,
        Abandoned = 128,
        Timeout = 258,
    }
    [Flags]
    internal enum Direction : byte
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3,
        Invalid = 255,
    }
    [Flags]
    internal enum ActionPane
    {
        Temuair,
        Medenia,
        World,
    }
    [Flags]
    internal enum Stat
    {
        STR = 1,
        DEX = 2,
        INT = 4,
        WIS = 8,
        CON = 16
    }
    [Flags]
    internal enum UserOption
    {
        Request = 0,
        Whisper = 1,
        Group = 2,
        LShout = 3,
        Wisdom = 4,
        Magic = 5,
        Exchange = 6,
        FastMove = 7,
        GuildChat = 8
    }
    [Flags]
    public enum TemClass : byte
    {
        Peasant = 0,
        Warrior = 1,
        Rogue = 2,
        Wizard = 3,
        Priest = 4,
        Monk = 5
    }
    [Flags]
    public enum Element : byte
    {
        None = 0,
        Fire = 1,
        Water = 2,
        Wind = 3,
        Earth = 4,
        Holy = 5,
        Darkness = 6,
        Wood = 7,
        Metal = 8,
        Nature = 9,
        Any = 10
    }
    [Flags]
    public enum MailFlags : byte
    {
        HasParcel = 1,
        HasLetter = 16
    }
    [Flags]
    public enum Nation : byte
    {
        None = 1,
        Suomi = 2,
        Loures = 3,
        Mileth = 4,
        Tagor = 5,
        Rucesion = 6,
        Noes = 7
    }
    [Flags]
    internal enum StatUpdateFlags : byte
    {
        None = 0,
        UnreadMail = 1,
        Unknown = 2,
        Secondary = 4,
        Experience = 8,
        Vitality = 16,
        Primary = 32,
        GameMasterA = 64,
        GameMasterB = 128,
        Swimming = GameMasterB | GameMasterA,
        Full = Primary | Vitality | Experience | Secondary,
    }
    [Flags]
    public enum SocialStatus : byte
    {
        Awake = 0,
        DoNotDisturb = 1,
        DayDreaming = 2,
        NeedGroup = 3,
        Grouped = 4,
        LoneHunter = 5,
        GroupHunting = 6,
        NeedHelp = 7
    }
    [Flags]
    public enum Status : ulong
    {
        None = 0U
        //add more statuses here, double each time
    }
}
