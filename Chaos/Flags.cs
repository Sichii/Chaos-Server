using System;

namespace Chaos
{
    #region Server
    internal enum EncryptMethod
    {
        None = 0,
        Normal = 1,
        MD5Key = 2
    }
    internal enum ServerType : byte
    {
        Lobby = 0,
        Login = 1,
        World = 2
    }
    internal enum WaitEventResult
    {
        Signaled = 0,
        Abandoned = 128,
        Timeout = 258,
    }
    [Flags]
    internal enum MapFlags : uint
    {
        Hostile = 1,
        NonHostile = 2,
        NoSpells = 4,
        NoSkills = 8,
        NoChat = 16,
        Snowing = 32,
        PvP = 64
        //and whatever else we decide
    }
    internal enum Status : ulong
    {
        None = 0,
        //add more statuses here, double each time
    }
    #endregion

    #region Messages
    internal enum ServerMessageType : byte
    {
        Whisper = 0,        //blue text
        OrangeBar1 = 1,     //orange bar only
        OrangeBar2 = 2,
        ActiveMessage = 3,  //orange bar + shiftF
        OrangeBar3 = 4,
        PrangeBar4 = 5,
        OrangeBar5 = 6,
        UserOptions = 7,    //user options get sent through this byte
        ScrollWindow = 8,   //window with a scroll bar (like sense)
        NonScrollWindow = 9,//window without a scroll bar (identify item)
        WoodenBoard = 10,
        GroupChat = 11,     //puke green text (group)
        GuildChat = 12,     //olive green text
        ClosePopup = 17,    //closes current popup
        TopRight = 18       //white text top right
    }

    internal enum LoginMessageType : byte
    {
        Confirm = 0,
        Message = 3
    }

    internal enum ClientMessageType : byte
    {
        Normal = 0,
        Shout = 1,
        Chant = 2
    }

    internal enum MessageColor : byte
    {
        Red = 98,
        Yellow = 99,
        DarkGreen = 100,
        Silver = 101,
        DarkBlue = 102,
        White = 103,
        LighterGray = 104,
        LightGray = 105,
        Gray = 106,
        DarkGray = 107,
        DarkerGray = 108,
        Black = 109,
        HotPink = 111,
        DarkPurple = 112,
        NeonGreen = 113,
        Orange = 115,
        Brown = 116,
    }
    #endregion

    #region Legend
    internal enum MarkIcon : byte
    {
        Yay = 0,
        Warrior = 1,
        Rogue = 2,
        Wizard = 3,
        Priest = 4,
        Monk = 5,
        Heart = 6,
        Victory = 7
    }
    internal enum Quest : ulong
    {
        None = 0,
        //add more quest flags here, double each time
    }
    internal enum MarkColor : byte
    {
        White = 32,
        LightOrange = 50,
        LightYellow = 64,
        Yellow = 68,
        LightGreen = 75,
        Blue = 88,
        LightPink = 96,
        DarkPurple = 100,
        Pink = 105,
        Darkgreen = 125,
        Green = 128,
        Orange = 152,
        Brown = 160,
        Red = 248
    }
    #endregion

    #region DisplayData
    internal enum NameTagStyle : byte
    {
        NeutralHover = 0,
        Hostile = 1,
        FriendlyHover = 2,
        Neutral = 3
    }
    internal enum LanternSize : byte
    {
        None = 0,
        Small = 1,
        Large = 2
    }
    internal enum RestPosition : byte
    {
        None = 0,
        Kneel = 1,
        Lay = 2,
        Sprawl = 3
    }
    internal enum BodyColor : byte
    {
        White = 0,
        Pale = 1,
        Brown = 2,
        Green = 3,
        Yellow = 4,
        Tan = 5,
        Grey = 6,
        LightBlue = 7,
        Orange = 8,
        Purple = 9
    }
    internal enum Direction : byte
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3,
        Invalid = 255,
    }
    internal enum Gender : byte
    {
        Both = 0,
        Male = 1,
        Female = 2
    }
    #endregion

    #region Attributes
    internal enum Stat
    {
        STR = 1,
        DEX = 2,
        INT = 4,
        WIS = 8,
        CON = 16
    }
    internal enum Element : byte
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

    internal enum MailFlag : byte
    {
        None = 0,
        HasParcel = 1,
        HasLetter = 16
    }

    internal enum Nation : byte
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
    #endregion

    #region Profile
    internal enum BaseClass : byte
    {
        Peasant = 0,
        Warrior = 1,
        Rogue = 2,
        Wizard = 3,
        Priest = 4,
        Monk = 5
    }
    internal enum AdvClass : byte
    {
        None = 0,
        Gladiator = 1,
        Druid = 2,
        Archer = 3,
        Bard = 4,
        Summoner = 5
    }
    internal enum EquipmentSlot : byte
    {
        None = 0,
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
    #endregion

    #region Options
    internal enum SocialStatus : byte
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
    #endregion

    #region GuI
    internal enum LightLevel : byte
    {
        Darkest = 0,
        Darker = 1,
        Dark = 2,
        Light = 3,
        Lighter = 4,
        Lightest = 5
    }
    internal enum EffectsBarColor : byte
    {
        None = 0,
        Blue = 1,
        Green = 2,
        Orange = 3,
        Red = 4,
        White = 5
    }
    internal enum Pane : byte
    {
        Inventory = 0,
        SpellBook = 1,
        SkillBook = 2
    }
    #endregion
}
