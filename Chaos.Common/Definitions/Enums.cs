#pragma warning disable CS1591
namespace Chaos.Common.Definitions;

#region Custom Stuff
public enum ShardingType : byte
{
    None,
    AbsolutePlayerLimit,
    PlayerLimit,
    AbsoluteGroupLimit
}

public enum RandomizationType
{
    Balanced = 0,
    Positive = 1,
    Negative = 2
}

[Flags]
public enum UserState : ulong
{
    None = 0,

    IsChanting = 1

    //add more user states here, double each time
}

public enum LootTableMode
{
    ChancePerItem,
    PickSingleOrDefault
}

public enum ChaosDialogType : byte
{
    Menu,
    MenuWithArgs,
    MenuTextEntry,
    MenuTextEntryWithArgs,
    ShowItems,
    ShowPlayerItems,
    ShowSpells,
    ShowSkills,
    ShowPlayerSpells,
    ShowPlayerSkills,
    Normal,
    DialogMenu,
    DialogTextEntry,
    Speak,
    CreatureMenu,
    Protected,
    CloseDialog
}
#endregion

#region Misc server enums
/// <summary>
///     A custom enum to separate server types for the purposes of redirects.
/// </summary>
public enum ServerType : byte
{
    /// <summary>
    ///     When you first open the client, you are in the lobby where you can choose a server to log in to
    /// </summary>
    Lobby = 0,

    /// <summary>
    ///     After choosing a server, you are redirected to a login server
    /// </summary>
    Login = 1,

    /// <summary>
    ///     After logging in, you are redirected to a world server.
    /// </summary>
    World = 2
}

/// <summary>
///     A byte switch as used for ClientOpCode.ServerTableRequest
/// </summary>
public enum ServerTableRequestType : byte
{
    /// <summary>
    ///     When a user chooses a server from the server list, the Id of that server is sent to us. This is the Id from the
    ///     server table.
    /// </summary>
    ServerId = 0,

    /// <summary>
    ///     Requests the server table, which is a list of servers for the user to choose to join
    /// </summary>
    RequestTable = 1
}

/// <summary>
///     A byte switch as used for ClientOpCode.MetaDataRequest
/// </summary>
public enum MetaDataRequestType : byte
{
    /// <summary>
    ///     The client is requesting the data for a specific metadata file
    /// </summary>
    DataByName = 0,

    /// <summary>
    ///     The client is requesting checksums of all metadata files, to ensure it has everything
    /// </summary>
    AllCheckSums = 1
}
#endregion

#region Messages
/// <summary>
///     A byte switch as used for ServerOpCode.ServerMessage
/// </summary>
public enum ServerMessageType : byte
{
    /// <summary>
    ///     Text appears blue, and appears in the top left
    /// </summary>
    Whisper = 0,

    /// <summary>
    ///     Text is only in the action bar, and will not show up in Shift+F
    /// </summary>
    OrangeBar1 = 1,

    /// <summary>
    ///     Text is only in the action bar, and will not show up in Shift+F
    /// </summary>
    OrangeBar2 = 2,

    /// <summary>
    ///     Texts appears in the action bar and Shift+F
    /// </summary>
    ActiveMessage = 3,

    /// <summary>
    ///     Text is only in the action bar, and will not show up in Shift+F
    /// </summary>
    OrangeBar3 = 4,

    /// <summary>
    ///     Text is only in the action bar, and will not show up in Shift+F. In official this was used for admin world messages
    /// </summary>
    AdminMessage = 5,

    /// <summary>
    ///     Text is only in the action bar, and will not show up in Shift+F
    /// </summary>
    OrangeBar5 = 6,

    /// <summary>
    ///     <see cref="Chaos.Common.Definitions.UserOption" />s are sent via this text channel
    /// </summary>
    UserOptions = 7,

    /// <summary>
    ///     Pops open a window with a scroll bar. In official this was used for Sense
    /// </summary>
    ScrollWindow = 8,

    /// <summary>
    ///     Pops open a window with no scroll bar. In official this was used for perish lore
    /// </summary>
    NonScrollWindow = 9,

    /// <summary>
    ///     Pops open a window with a wooden boarder. In official this was used for signposts and wooden boards
    /// </summary>
    WoodenBoard = 10,

    /// <summary>
    ///     Text appears in a puke-green color. In official this was used for group chat
    /// </summary>
    GroupChat = 11,

    /// <summary>
    ///     Text appears in an olive-green color. In official this was used for guild chat
    /// </summary>
    GuildChat = 12,

    /// <summary>
    ///     Closes opened pop-up windows. <see cref="ScrollWindow" />, <see cref="NonScrollWindow" />,
    ///     <see cref="WoodenBoard" />
    /// </summary>
    ClosePopup = 17,

    /// <summary>
    ///     Text appears white, and persists indefinitely until cleared in the top right corner
    /// </summary>
    PersistentMessage = 18
}

/// <summary>
///     A byte switch as used by ServerOpCode.LoginMessage
/// </summary>
public enum LoginMessageType : byte
{
    /// <summary>
    ///     A generic confirmation window with an ok button
    /// </summary>
    Confirm = 0,

    /// <summary>
    ///     Clears the name field during character creation and presents a message with an ok button
    /// </summary>
    ClearNameMessage = 3,

    /// <summary>
    ///     Clears the password field during character creation and presents a message with an ok button
    /// </summary>
    ClearPswdMessage = 5,

    /// <summary>
    ///     Clears the name and password fields on the login screen and presents a message with an ok button
    /// </summary>
    CharacterDoesntExist = 14,

    /// <summary>
    ///     Clears the password fields on the login screen and presents a message with an ok button
    /// </summary>
    WrongPassword = 15
}

/// <summary>
///     A byte switch used by ServerOpCode.LoginControls
/// </summary>
public enum LoginControlsType : byte
{
    /// <summary>
    ///     Tells the client that the packet contains the homepage url
    /// </summary>
    Homepage = 3
}

/// <summary>
///     A byte switch as used by ServerOpCode.PublicMessage
/// </summary>
public enum PublicMessageType : byte
{
    /// <summary>
    ///     Normal white chat message
    /// </summary>
    Normal = 0,

    /// <summary>
    ///     Yellow shout message
    /// </summary>
    Shout = 1,

    /// <summary>
    ///     Blue chant message
    /// </summary>
    Chant = 2
}

/// <summary>
///     Byte codes for the characters used for each color in the pattern '{=a'. Used by many things.
/// </summary>
public enum MessageColor : byte
{
    /// <summary>
    ///     The default color, keep the existing color
    /// </summary>
    Default = 0,

    /// <summary>
    ///     "{=a"
    /// </summary>
    Gray = 97,

    /// <summary>
    ///     "{=b"
    /// </summary>
    Red = 98,

    /// <summary>
    ///     "{=c" This color does not work in public chat
    /// </summary>
    Yellow = 99,

    /// <summary>
    ///     "{=d"
    /// </summary>
    DarkGreen = 100,

    /// <summary>
    ///     "{=e" This color does not work in public chat
    /// </summary>
    Silver = 101,

    /// <summary>
    ///     "{=f"
    /// </summary>
    Blue = 102,

    /// <summary>
    ///     "{=g"
    /// </summary>
    Gainsboro = 103,

    /// <summary>
    ///     "{=i"
    /// </summary>
    SpanishGray = 105,

    /// <summary>
    ///     "{=j"
    /// </summary>
    Nickel = 106,

    /// <summary>
    ///     "{=k"
    /// </summary>
    Slate = 107,

    /// <summary>
    ///     "{=l"
    /// </summary>
    Charcoal = 108,

    /// <summary>
    ///     "{=m"
    /// </summary>
    DirtyBlack = 109,

    /// <summary>
    ///     "{=n"
    /// </summary>
    Black = 110,

    /// <summary>
    ///     "{=o"
    /// </summary>
    HotPink = 111,

    /// <summary>
    ///     "{=p"
    /// </summary>
    Purple = 112,

    /// <summary>
    ///     "{=q"
    /// </summary>
    NeonGreen = 113,

    /// <summary>
    ///     "{=s"
    /// </summary>
    Orange = 115,

    /// <summary>
    ///     "{=t"
    /// </summary>
    Brown = 116,

    /// <summary>
    ///     "{=u"
    /// </summary>
    White = 117,

    /// <summary>
    ///     "{=x"
    /// </summary>
    Invisible = 120
}
#endregion

#region Legend
/// <summary>
///     A byte representing the icon of a legend mark. Used by ServerOpCode.Profile and ServerOpCode.SelfProfile
/// </summary>
public enum MarkIcon : byte
{
    Yay = 0,
    Warrior = 1,
    Rogue = 2,
    Wizard = 3,
    Priest = 4,
    Monk = 5,
    Heart = 6,
    Victory = 7,
    Invisible = 8
}

/// <summary>
///     A byte representing the color of a legend mark. Used by ServerOpCode.Profile and ServerOpCode.SelfProfile
/// </summary>
public enum MarkColor : byte
{
    Invisible = 0,
    Cyan = 1,
    BrightRed = 2,
    GrayTan = 3,
    BrightGray = 4,
    Gray = 5,
    OffWhite = 13,
    DarkGray = 14,
    White = 16,
    BrightBrightGray = 17,
    GrayGreen = 20,
    LightPink = 32,
    LightOrange = 50,
    LightYellow = 64,
    Yellow = 68,
    LightGreen = 75,
    Blue = 88,
    LightPurple = 96,
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
/// <summary>
///     A byte representing the color of hair and items. Used by many things.
/// </summary>
public enum DisplayColor : byte
{
    /// <summary>
    ///     Actually Lavender
    /// </summary>
    Default,
    Black,
    Apple,
    Carrot,
    Yellow,
    Teal,
    Blue,
    Violet,
    Olive,
    Green,
    Pumpkin,
    Brown,
    Gray,
    Navy,
    Tan,
    White,
    Pink,
    Chartreuse,
    Orange,
    LightBlonde,
    Midnight,
    Sky,
    Mauve,
    Orchid,
    BubbleGum,
    LightBlue,
    HotPink,
    Cyan,
    Lilac,
    Salmon,
    NeonBlue,
    NeonGreen,
    PastelGreen,
    Blonde,
    RoyalBlue,
    Leather,
    Scarlet,
    Forest,
    Scarlet2,
    YaleBlue,
    Tangerine,
    DirtyBlonde,
    Sage,
    Grass,
    Cobalt,
    Blush,
    Glitch,
    Aqua,
    Lime,
    Purple,
    NeonRed,
    NeonYellow,
    PalePink,
    Peach,
    Crimson,
    Mustard,
    Silver,
    Fire,
    Ice,
    Magenta,
    PaleGreen,
    BabyBlue,
    Void,
    GhostBlue,
    Mint,
    Fern,
    GhostPink,
    Flamingo,
    Turqoise,
    MatteBlack,
    Taffy,
    NeonPurple
}

/// <summary>
///     A byte representing the style a player's name is shown in. Used by ServerOpCode.DisplayAisling
/// </summary>
public enum NameTagStyle : byte
{
    NeutralHover = 0,
    Hostile = 1,
    FriendlyHover = 2,
    Neutral = 3
}

/// <summary>
///     A byte representing the size of the lantern effect around a player. Used by ServerOpCode.DisplayAisling
/// </summary>
public enum LanternSize : byte
{
    None = 0,
    Small = 1,
    Large = 2
}

/// <summary>
///     A byte representing the body position a player is in while using a rest cloak. Used by ServerOpCode.DisplayAisling
/// </summary>
public enum RestPosition : byte
{
    None = 0,
    Kneel = 1,
    Lay = 2,
    Sprawl = 3
}

/// <summary>
///     A byte representing the color of the player's skin. Used by ServerOpCode.DisplayAisling
/// </summary>
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

/// <summary>
///     A byte representing the sprite used for a player's body. Used by ServerOpCode.DisplayAisling
/// </summary>
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

/// <summary>
///     A byte representing the gender of the player. Used by many things.
/// </summary>
[Flags]
public enum Gender : byte
{
    None = 0,
    Male = 1,
    Female = 2,
    Unisex = Male | Female
}
#endregion

#region Attributes
/// <summary>
///     A byte switch used for ClientOpCode.RaiseStat
/// </summary>
public enum Stat
{
    STR = 1,
    DEX = 2,
    INT = 4,
    WIS = 8,
    CON = 16
}

/// <summary>
///     A byte representing attack and defense elements on the Shift+G panel. Used by ServerOpCode.Attributes
/// </summary>
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
    Undead = 9
}

/// <summary>
///     A byte representing which type of mail was received. Used by ServerOpCode.Attributes
/// </summary>
public enum MailFlag : byte
{
    None = 0,
    HasMail = 16
}

/// <summary>
///     A byte representing the nation emblem displayed in the player's profile. Used by ServerOpCode.Profile and
///     ServerOpCode.SelfProfile
/// </summary>
public enum Nation : byte
{
    Exile,
    Suomi,
    Ellas,
    Loures,
    Mileth,
    Tagor,
    Rucesion,
    Noes,
    Illuminati,
    Piet,
    Atlantis,
    Abel,
    Undine,
    Purgatory
}

/// <summary>
///     A flag representing what combination of stats are being sent to a user. Used by ServerOpCode.Attributes
/// </summary>
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
/// <summary>
///     A byte representing the 'temuair class' of an aisling. Used in many places.
/// </summary>
public enum BaseClass : byte
{
    Peasant = 0,
    Warrior = 1,
    Rogue = 2,
    Wizard = 3,
    Priest = 4,
    Monk = 5,
    Diacht = 255
}

/// <summary>
///     A byte representing the 'medenia class' of an aisling. Used in many places.
/// </summary>
public enum AdvClass : byte
{
    None = 0,
    Gladiator = 1,
    Druid = 2,
    Archer = 3,
    Bard = 4,
    Summoner = 5
}

/// <summary>
///     A byte representing the slot of a piece of equipment. Used in many places.
/// </summary>
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

/// <summary>
///     A byte representing the type of a piece of equipment.
/// </summary>
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
/// <summary>
///     A byte representing the social status of the user. Used by ServerOpCode.Profile, and ServerOpCode.WorldList
/// </summary>
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

/// <summary>
///     A byte representing an option on the client. Used by ClientOpCode.UserOptionToggle and ServerOpCode.ServerMessage
/// </summary>
public enum UserOption
{
    Request = 0,
    Option1 = 1,
    Option2 = 2,
    Option3 = 3,
    Option4 = 4,
    Option5 = 5,
    Option6 = 6,
    Option7 = 7,
    Option8 = 8
}
#endregion

#region GuI
/// <summary>
///     A byte representing the color of a name in the world list. Used by ServerOpCode.WorldList
/// </summary>
public enum WorldListColor : byte
{
    Guilded = 84,
    WithinLevelRange = 151,
    White = 255
}

/// <summary>
///     A byte representing the color an effect, for the purposes of showing it's remaining duration. Used by
///     ServerOpCode.Effect
/// </summary>
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

/// <summary>
///     A byte representing the type of a panel. Used by ClientOpCode.SwapSlot
/// </summary>
public enum PanelType : byte
{
    Inventory = 0,
    SpellBook = 1,
    SkillBook = 2,
    Equipment = 3
}

public enum PageType : byte
{
    Page1,
    Page2,
    Page3
}

/// <summary>
///     A byte representing the type of a merchant menu. Used by ServerOpCode.Menu
/// </summary>
public enum MenuType : byte
{
    Menu = 0,
    MenuWithArgs = 1,
    TextEntry = 2,
    TextEntryWithArgs = 3,
    ShowItems = 4,
    ShowPlayerItems = 5,
    ShowSpells = 6,
    ShowSkills = 7,
    ShowPlayerSpells = 8,
    ShowPlayerSkills = 9
}

/// <summary>
///     A byte switch used to specify the type of additional args included in a dialog response. Used by
///     ClientOpCode.DialogResponse
/// </summary>
public enum DialogArgsType : byte
{
    None = 0,
    MenuResponse = 1,
    TextResponse = 2
}

/// <summary>
///     A byte switched used to specify the type of dialog. Used by ServerOpCode.Dialog
/// </summary>
public enum DialogType : byte
{
    Normal = 0,
    DialogMenu = 2,
    TextEntry = 4,
    Speak = 5,
    CreatureMenu = 6,
    Protected = 9,
    CloseDialog = 10
}

/// <summary>
///     A byte representing which button on a dialog window was pressed. Used by ClientOpCode.DialogResponse
/// </summary>
public enum DialogResult : sbyte
{
    Previous = -1,
    Close = 0,
    Next = 1
}

/// <summary>
///     A byte representing the type of board being displayed. Used by ServerOpCode.BulletinBoard
/// </summary>
public enum BoardOrResponseType : byte
{
    BoardList = 1,
    PublicBoard = 2,
    PublicPost = 3,
    MailBoard = 4,
    MailPost = 5,
    SubmitPostResponse = 6,
    DeletePostResponse = 7,
    HighlightPostResponse = 8
}

/// <summary>
///     A byte representing the type of request in association with a board. Used by ClientOpCode.BoardRequest
/// </summary>
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

public enum BoardControls : sbyte
{
    NextPage = -1,
    RequestPost = 0,
    PreviousPage = 1
}

public enum NotepadType
{
    Brown = 0,
    GlitchedBlue1 = 1,
    GlitchedBlue2 = 2,
    Orange = 3,
    White = 4
}
#endregion

#region Skill/Spell
/// <summary>
///     A byte representing the type of a spell. Used by ServerOpCode.AddSpellToPane
/// </summary>
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

/// <summary>
///     A byte representing the emote/body animation being sent. Used by ServerOpCode.BodyAnimation
/// </summary>
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
/// <summary>
///     A byte switch used when receiving information about a click. Used by ClientOpCode.Click
/// </summary>
public enum ClickType : byte
{
    TargetId = 1,
    TargetPoint = 3
}

public enum LevelCircle : byte
{
    /// <summary>
    ///     Levels 1-10
    /// </summary>
    One = 1,

    /// <summary>
    ///     Levels 11-40
    /// </summary>
    Two = 2,

    /// <summary>
    ///     Levels 41-70
    /// </summary>
    Three = 3,

    /// <summary>
    ///     Levels 71-98
    /// </summary>
    Four = 4,

    /// <summary>
    ///     Levels 99+
    /// </summary>
    Five = 5,

    /// <summary>
    ///     Master
    /// </summary>
    Six = 6,

    /// <summary>
    ///     Advanced Class
    /// </summary>
    Seven = 7
}

/// <summary>
///     A byte switch used when receiving information about an action performed on an exchange window. Used by
///     ClientOpCode.Exchange
/// </summary>
public enum ExchangeRequestType : byte
{
    StartExchange = 0,
    AddItem = 1,
    AddStackableItem = 2,
    SetGold = 3,
    Cancel = 4,
    Accept = 5
}

/// <summary>
///     A byte switch used to specify the action performed on an exchange window. Used by ServerOpCode.Exchange
/// </summary>
public enum ExchangeResponseType : byte
{
    StartExchange = 0,
    RequestAmount = 1,
    AddItem = 2,
    SetGold = 3,
    Cancel = 4,
    Accept = 5
}

public enum EntityType : byte
{
    Creature = 1,
    Item = 2,
    Aisling = 4
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

/// <summary>
///     Feel free to rename these to align with whatever you have configured in your light metadata.
/// </summary>
/// <remarks>
///     You can have all 12 values be in sequence of darkest to lightest (or vice versa), but this will constrain you to
///     only having 1 light type per map. Alternatively, you could 2 different light types defined here, 6 values for each,
///     which would allow you to define 2 different light types per map without needing to restart the server. Each set of
///     6 values would both be defined under the same light type in the metadata.
/// </remarks>
public enum LightLevel : byte
{
    Darkest_A = 0,
    Darker_A = 1,
    Dark_A = 2,
    Light_A = 3,
    Lighter_A = 4,
    Lightest_A = 5,
    Darkest_B = 6,
    Darker_B = 7,
    Dark_B = 8,
    Light_B = 9,
    Lighter_B = 10,
    Lightest_B = 11
}

[Flags]
public enum MapFlags : byte
{
    None = 0,
    Snow = 1,
    Rain = 2,
    Darkness = Rain | Snow,
    NoTabMap = 64,
    SnowTileset = 128
}

[Flags]
public enum TileFlags : byte
{
    None = 0,
    Wall = 15,
    Unknown = 128
}
#endregion