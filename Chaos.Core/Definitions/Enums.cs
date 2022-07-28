namespace Chaos.Core.Definitions;

#region Server
public enum ServerType : byte
{
    Lobby = 0,
    Login = 1,
    World = 2
}

public enum ServerTableRequestType : byte
{
    ServerId = 0,
    RequestTable = 1
}

public enum MetafileRequestType : byte
{
    DataByName = 0,
    AllCheckSums = 1
}
#endregion

#region Custom Flags
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
#endregion

#region Messages
public enum ServerMessageType : byte
{
    Whisper = 0, //blue text
    OrangeBar1 = 1, //orange bar only
    OrangeBar2 = 2,
    ActiveMessage = 3, //orange bar + shiftF
    OrangeBar3 = 4,
    AdminMessage = 5, //in USDA, admins use this orange-bar channel
    OrangeBar5 = 6,
    UserOptions = 7, //user options get sent through this byte
    ScrollWindow = 8, //window with a scroll bar (like sense)
    NonScrollWindow = 9, //window without a scroll bar (identify item)
    WoodenBoard = 10,
    GroupChat = 11, //puke green text (group)
    GuildChat = 12, //olive green text
    ClosePopup = 17, //closes current popup
    TopRight = 18 //white text top right
}

public enum LoginMessageType : byte
{
    Confirm = 0,
    ClearNameMessage = 3,
    ClearPswdMessage = 5,
    CharacterDoesntExist = 14,
    WrongPassword = 15
}

public enum LoginControlsType : byte
{
    Homepage = 3
}

public enum PublicMessageType : byte
{
    Normal = 0,
    Shout = 1,
    Chant = 2
}

public enum MessageColor : byte
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
    Brown = 116
}
#endregion

#region Legend
public enum MarkIcon : byte
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

[Flags]
public enum MarkColor : byte
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
public enum DisplayColor : byte
{
    /// <summary>
    ///     Actually Lavender
    /// </summary>
    None,
    Black,
    Red,
    Orange,
    Blonde,
    Cyan,
    Blue,
    Mulberry,
    Olive,
    Green,
    Fire,
    Brown,
    Grey,
    Navy,
    Tan,
    White,
    Pink,
    Chartreuse,
    Golden,
    Lemon,
    Royal,
    Platinum,
    Lilac,
    Fuchsia,
    Magenta,
    Peacock,
    NeonPink,
    Arctic,
    Mauve,
    NeonOrange,
    Sky,
    NeonGreen,
    Pistachio,
    Corn,
    Cerulean,
    Chocolate,
    Ruby,
    Hunter,
    Crimson,
    Ocean,
    Ginger,
    Mustard,
    Apple,
    Leaf,
    Cobalt,
    Strawberry,
    Unusual,
    Sea,
    Harlequin,
    Amethyst,
    NeonRed,
    NeonYellow,
    Rose,
    Salmon,
    Scarlet,
    Honey
}

public enum NameTagStyle : byte
{
    NeutralHover = 0,
    Hostile = 1,
    FriendlyHover = 2,
    Neutral = 3
}

public enum LanternSize : byte
{
    None = 0,
    Small = 1,
    Large = 2
}

public enum RestPosition : byte
{
    None = 0,
    Kneel = 1,
    Lay = 2,
    Sprawl = 3
}

public enum BodyColor : byte
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

public enum BodySprite : byte
{
    None = 0,
    Male = 16,
    Female = 32,
    MaleGhost = 48,
    FemaleGhost = 64,
    MaleInvis = 80,
    FemaleInvis = 96,
    MaleJester = 112,
    MaleHead = 128,
    FemaleHead = 144,
    BlankMale = 160,
    BlankFemale = 176
}

[Flags]
public enum Gender : byte
{
    Male = 1,
    Female = 2,
    Unisex = Male | Female
}
#endregion

#region Attributes
public enum Stat
{
    STR = 1,
    DEX = 2,
    INT = 4,
    WIS = 8,
    CON = 16
}

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

public enum MailFlag : byte
{
    None = 0,
    Parcel = 1,
    Letter = 16
}

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
public enum StatUpdateType : byte
{
    None = 0,
    /// <summary>
    ///     mail
    /// </summary>
    UnreadMail = 1,
    Unknown = 2,
    /// <summary>
    ///     Blind, Mail, Elements, Ressists, AC, DMG, HIT
    /// </summary>
    Secondary = 4,
    /// <summary>
    ///     Exp, Gold
    /// </summary>
    ExpGold = 8,
    /// <summary>
    ///     Current HP, Current MP
    /// </summary>
    Vitality = 16,
    /// <summary>
    ///     Level, Max HP/MP, Current stats, Weight, Unspent
    /// </summary>
    Primary = 32,
    GameMasterA = 64,
    GameMasterB = 128,
    Swimming = GameMasterA | GameMasterB,
    Full = Primary | Vitality | ExpGold | Secondary
}
#endregion

#region Profile
public enum BaseClass : byte
{
    Peasant = 0,
    Warrior = 1,
    Rogue = 2,
    Wizard = 3,
    Priest = 4,
    Monk = 5,
    Admin = 6
}

public enum AdvClass : byte
{
    None = 0,
    Gladiator = 1,
    Druid = 2,
    Archer = 3,
    Bard = 4,
    Summoner = 5
}

public enum EquipmentSlot : byte
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

public enum EquipmentType : byte
{
    NotEquipment,
    Weapon,
    Armor,
    OverArmor,
    Shield,
    Helmet,
    OverHelmet,
    Earrings,
    Necklace,
    Ring,
    Gauntlet,
    Belt,
    Greaves,
    Boots,
    Accessory
}
#endregion

#region Options
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

public enum UserOption
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
public enum WorldListColor : byte
{
    Guilded = 84,
    WithinLevelRange = 151,
    White = 255
}

public enum EffectColor : byte
{
    None = 0,
    Blue = 1,
    Green = 2,
    Yellow = 3,
    Orange = 4,
    Red = 5,
    White = 6
}

public enum PanelType : byte
{
    Inventory = 0,
    SpellBook = 1,
    SkillBook = 2,
    Equipment = 3
}

public enum MenuType : byte
{
    Menu = 0,
    MenuWithArgs = 1,
    TextEntry = 2,
    ShowItems = 4,
    ShowOwnedItems = 5,
    ShowSpells = 6,
    ShowSkills = 7,
    ShowLearnedSpells = 8,
    ShowLearnedSkills = 9,
    Dialog = 255
}

public enum DialogArgsType : byte
{
    None = 0,
    MenuResponse = 1,
    TextResponse = 2
}

public enum DialogType : byte
{
    Normal = 0,
    ItemMenu = 2,
    TextEntry = 4,
    Speak = 5,
    CreatureMenu = 6,
    Protected = 9,
    CloseDialog = 10
}

public enum DialogOption : sbyte
{
    Previous = -1,
    Close = 0,
    Next = 1
}

public enum BoardType : byte
{
    BoardList = 1,
    PublicBoard = 2,
    PublicPost = 3,
    PrivateBoard = 4,
    PrivatePost = 5,
    Message1 = 6,
    Message2 = 7
}

public enum BoardRequestType : byte
{
    BoardList = 1,
    ViewBoard = 2,
    ViewPost = 3,
    NewPost = 4,
    Delete = 5,
    SendMail = 6,
    Highlight = 7
}
#endregion

#region Skill/Spell
public enum SpellType : byte
{
    None = 0,
    Prompt = 1,
    Targeted = 2,
    Prompt4Nums = 3,
    Prompt3Nums = 4,
    NoTarget = 5,
    Prompt2Nums = 6,
    Prompt1Num = 7
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

public enum BodyAnimation : byte
{
    None = 0,
    Assail = 1,
    HandsUp = 6,
    Smile = 9,
    Cry = 10,
    Frown = 11,
    Wink = 12,
    Surprise = 13,
    Tongue = 14,
    Pleasant = 15,
    Snore = 16,
    Mouth = 17,
    BlowKiss = 21,
    Wave = 22,
    RockOn = 23,
    Peace = 24,
    Stop = 25,
    Ouch = 26,
    Impatient = 27,
    Shock = 28,
    Pleasure = 29,
    Love = 30,
    SweatDrop = 31,
    Whistle = 32,
    Irritation = 33,
    Silly = 34,
    Cute = 35,
    Yelling = 36,
    Mischievous = 37,
    Evil = 38,
    Horror = 39,
    PuppyDog = 40,
    StoneFaced = 41,
    Tears = 42,
    FiredUp = 43,
    Confused = 44,
    PriestCast = 128,
    TwoHandAtk = 129,
    Jump = 130,
    Kick = 131,
    Punch = 132,
    RoundHouseKick = 133,
    Stab = 134,
    DoubleStab = 135,
    WizardCast = 136,
    PlayNotes = 137,
    HandsUp2 = 138,
    Swipe = 139,
    HeavySwipe = 140,
    JumpAttack = 141,
    BowShot = 142,
    HeavyBowShot = 143,
    LongBowShot = 144,
    Summon = 145
}
#endregion

#region Game
public enum ClickType : byte
{
    TargetId = 1,
    TargetPoint = 3,
    Unknown = 86
}

public enum ExchangeRequestType : byte
{
    StartExchange = 0,
    AddItem = 1,
    AddStackableItem = 2,
    SetGold = 3,
    Cancel = 4,
    Accept = 5
}

public enum ExchangeResponseType : byte
{
    StartExchange = 0,
    RequestAmount = 1,
    AddItem = 2,
    SetGold = 3,
    Cancel = 4,
    Accept = 5
}

public enum GameObjectType : byte
{
    Merchant = 1,
    Item = 2,
    Misc = 4
}

public enum IgnoreType : byte
{
    Request = 1,
    AddUser = 2,
    RemoveUser = 3
}

public enum CreatureType : byte
{
    Normal = 0,
    WalkThrough = 1,
    Merchant = 2,
    WhiteSquare = 3,
    Aisling = 4
}

public enum GroupRequestType : byte
{
    FormalInvite = 1,
    TryInvite = 2,
    AcceptInvite = 3,
    Groupbox = 4,
    RemoveGroupBox = 6
}

public enum LightLevel : byte
{
    Darkest = 0,
    Darker = 1,
    Dark = 2,
    Light = 3,
    Lighter = 4,
    Lightest = 5
}
#endregion