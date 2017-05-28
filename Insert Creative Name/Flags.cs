using System;

namespace Insert_Creative_Name
{
    [Flags]
    internal enum EncryptMethod
    {
        None = 0,
        Normal = 1,
        MD5Key = 2
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
        Shout = 3,
        Wisdom = 4,
        Magic = 5,
        Exchange = 6,
        FastMove = 7,
        GuildChat = 8
    }
    [Flags]
    public enum BaseClass : byte
    {
        Peasant = 0,
        Warrior = 1,
        Rogue = 2,
        Wizard = 3,
        Priest = 4,
        Monk = 5
    }
    [Flags]
    public enum AdvClass : byte
    {
        None = 0,
        Gladiator = 1,
        Druid = 2,
        Archer = 3,
        Bard = 4,
        Summoner = 5
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
        Nature = 9
    }
    [Flags]
    public enum MailFlag : byte
    {
        HasParcel = 1,
        HasLetter = 16
    }
    [Flags]
    public enum EquipmentSlot : byte
    {
        Weapon = 1,
        Armor = 2,
        Shield = 3,
        Helmet = 4,
        Earrings = 5,
        Necklace = 6,
        LeftRing = 7,
        RightRing = 8,
        LeftGaunt = 9,
        RightGaunt = 10,
        Belt = 11,
        Greaves = 12,
        Boots = 13,
        Accessory1 = 14,
        Overcoat = 15,
        OverHelm = 16,
        Accessory2 = 17,
        Accessory3 = 18
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
        Swimming = GameMasterA | GameMasterB,
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
        None = 0,
        //add more statuses here, double each time
    }
    public enum Quest : ulong
    {
        None = 0,
        //add more quest flags here, double each time
    }
}
