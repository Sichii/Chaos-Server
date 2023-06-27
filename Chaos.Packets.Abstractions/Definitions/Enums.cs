namespace Chaos.Packets.Abstractions.Definitions;

/// <summary>
///     OpCodes used when receiving packets from a client
/// </summary>
/// <remarks>
///     In networking, an opcode is used to identify the type of packet that is being sent. The opcode is generally part of
///     a header near the
///     beginning of the data stream
/// </remarks>
public enum ClientOpCode : byte
{
    /// <summary>
    ///     OpCode used when a client requests the encryption details, and a checksum of the details of available login servers
    ///     <br />
    ///     Hex value: 0x00
    /// </summary>
    ConnectionInfoRequest = 0,
    /// <summary>
    ///     Opcode used when a client requests to create a new character. This is the first step in the process and
    ///     will only contain a name and password<br />
    ///     Hex value: 0x02
    /// </summary>
    CreateCharRequest = 2,
    /// <summary>
    ///     OpCode used when a client provides credentials to log into the world<br />
    ///     Hex value: 0x03
    /// </summary>
    Login = 3,
    /// <summary>
    ///     OpCode used when a client requests to create a new character. This is the second step in the process and
    ///     will contain appearance details<br />
    ///     Hex value: 0x04
    /// </summary>
    CreateCharFinalize = 4,
    /// <summary>
    ///     OpCode used when a client requests tile data for the current map<br />
    ///     Hex value: 0x05
    /// </summary>
    RequestMapData = 5,
    /// <summary>
    ///     OpCode used when a client walks in a direction<br />
    ///     Hex value: 0x06
    /// </summary>
    ClientWalk = 6,
    /// <summary>
    ///     OpCode used when a client picks up an item from the ground<br />
    ///     Hex value: 0x07
    /// </summary>
    Pickup = 7,
    /// <summary>
    ///     OpCode used when a client drops an item on the ground<br />
    ///     Hex value: 0x08
    /// </summary>
    ItemDrop = 8,
    /// <summary>
    ///     OpCode used when a client tries to log out<br />
    ///     Hex value: 0x0B
    /// </summary>
    ExitRequest = 11,
    /// <summary>
    ///     OpCode used when a client requests the server to display an object<br />
    ///     Hex value: 0x0C
    /// </summary>
    DisplayObjectRequest = 12,
    /// <summary>
    ///     OpCode used when a client ignores or un-ignores another player, or requests a list of ignored players<br />
    ///     Hex value: 0x0D
    /// </summary>
    Ignore = 13,
    /// <summary>
    ///     OpCode used when a client sends a publicly visible message<br />
    ///     Hex value: 0x0E
    /// </summary>
    PublicMessage = 14,
    /// <summary>
    ///     OpCode used when a client uses a spell<br />
    ///     Hex value: 0x0F
    /// </summary>
    UseSpell = 15,
    /// <summary>
    ///     OpCode used when a client is redirected to this server<br />
    ///     Hex value: 0x10
    /// </summary>
    ClientRedirected = 16,
    /// <summary>
    ///     OpCode used when a client changes their character's direction<br />
    ///     Hex value: 0x11
    /// </summary>
    Turn = 17,
    /// <summary>
    ///     OpCode used when a client presses their spacebar<br />
    ///     Hex value: 0x13
    /// </summary>
    SpaceBar = 19,
    /// <summary>
    ///     OpCode used when a client requests a list of all online players<br />
    ///     Hex value: 0x18
    /// </summary>
    WorldListRequest = 24,
    /// <summary>
    ///     OpCode used when a client sends a private message to the server<br />
    ///     Hex value: 0x19
    /// </summary>
    Whisper = 25,
    /// <summary>
    ///     OpCode used when a client toggles a user option<br />
    ///     Hex value: 0x1B
    /// </summary>
    UserOptionToggle = 27,
    /// <summary>
    ///     OpCode used when a client uses an item<br />
    ///     Hex value: 0x1A
    /// </summary>
    UseItem = 28,
    /// <summary>
    ///     OpCode used when a client uses an emote<br />
    ///     Hex value: 0x1B
    /// </summary>
    Emote = 29,
    /// <summary>
    ///     OpCode used when a client sets persistent text on an object<br />
    ///     Hex value: 0x23
    /// </summary>
    SetNotepad = 35,
    /// <summary>
    ///     OpCode used when a client drops gold on the ground<br />
    ///     Hex value: 0x24
    /// </summary>
    GoldDrop = 36,
    /// <summary>
    ///     OpCode used when a client requests to change a character's password<br />
    ///     Hex value: 0x26
    /// </summary>
    PasswordChange = 38,
    /// <summary>
    ///     OpCode used when a client drops an item on a creature<br />
    ///     Hex value: 0x29
    /// </summary>
    ItemDroppedOnCreature = 41,
    /// <summary>
    ///     OpCode used when a client drops gold on a creature<br />
    ///     Hex value: 0x2A
    /// </summary>
    GoldDroppedOnCreature = 42,
    /// <summary>
    ///     OpCode used when a client requests the profile of another player<br />
    ///     Hex value: 0x2D
    /// </summary>
    RequestProfile = 45,
    /// <summary>
    ///     OpCode used when a client invites another player to a group, responds to a group invite, or creates or destroys a
    ///     group box<br />
    ///     Hex value: 0x2E
    /// </summary>
    GroupRequest = 46,
    /// <summary>
    ///     OpCode used when a client toggles their group availability<br />
    ///     Hex value: 0x2F
    /// </summary>
    ToggleGroup = 47,
    /// <summary>
    ///     OpCode used when a client swaps two panel objects<br />
    ///     Hex value: 0x30
    /// </summary>
    SwapSlot = 48,
    /// <summary>
    ///     OpCode used when a client refreshes their viewport<br />
    ///     Hex value: 0x38
    /// </summary>
    RequestRefresh = 56,
    /// <summary>
    ///     OpCode used when a client responds to a merchant menu<br />
    ///     Hex value: 0x39
    /// </summary>
    PursuitRequest = 57,
    /// <summary>
    ///     OpCode used when a client responds to a dialog<br />
    ///     Hex value: 0x3A
    /// </summary>
    DialogResponse = 58,
    /// <summary>
    ///     OpCode used when a client accesses a board or mail<br />
    ///     Hex value: 0x3B
    /// </summary>
    BoardRequest = 59,
    /// <summary>
    ///     OpCode used when a client uses a skill<br />
    ///     Hex value: 0x3E
    /// </summary>
    UseSkill = 62,
    /// <summary>
    ///     OpCode used when a client clicks on a world map node<br />
    ///     Hex value: 0x3F
    /// </summary>
    WorldMapClick = 63,
    /// <summary>
    ///     OpCode used when a client clicks on an object<br />
    ///     Hex value: 0x43
    /// </summary>
    Click = 67,
    /// <summary>
    ///     OpCode used when a client unequips an item<br />
    ///     Hex value: 0x44
    /// </summary>
    Unequip = 68,
    /// <summary>
    ///     OpCode used when a client sends a heartbeat(keep-alive) ping<br />
    ///     Hex value: 0x45
    /// </summary>
    HeartBeat = 69,
    /// <summary>
    ///     OpCode used when a client requests to raise a stat<br />
    ///     Hex value: 0x47
    /// </summary>
    RaiseStat = 71,
    /// <summary>
    ///     OpCode used when a client interacts with an exchange window<br />
    ///     Hex value: 0x4A
    /// </summary>
    Exchange = 74,
    /// <summary>
    ///     OpCode used when a client requests EULA details<br />
    ///     Hex value: 0x4B
    /// </summary>
    NoticeRequest = 75,
    /// <summary>
    ///     OpCode used when a client begins casting a spell with cast lines<br />
    ///     Hex value: 0x4D
    /// </summary>
    BeginChant = 77,
    /// <summary>
    ///     OpCode used when a client uses an ability that has chant lines<br />
    ///     Hex value: 0x4E
    /// </summary>
    Chant = 78,
    /// <summary>
    ///     OpCode used when a client responds to a request for profile data (portrait, text)<br />
    ///     Hex value: 0x4F
    /// </summary>
    Profile = 79,
    /// <summary>
    ///     OpCode used when a client requests the details of available login servers<br />
    ///     Hex value: 0x57
    /// </summary>
    ServerTableRequest = 87,
    /// <summary>
    ///     OpCode used when a client requests to change the packet sequence number<br />
    ///     Hex value: 0x62
    /// </summary>
    SequenceChange = 98,
    /// <summary>
    ///     OpCode used when a client requests the url of the homepage<br />
    ///     Hex value: 0x68
    /// </summary>
    HomepageRequest = 104,
    /// <summary>
    ///     OpCode used when a client sends it's Environment.Ticks value<br />
    ///     Hex value: 0x75
    /// </summary>
    SynchronizeTicks = 117,
    /// <summary>
    ///     OpCode used when a client changes their social status<br />
    ///     Hex value: 0x79
    /// </summary>
    SocialStatus = 121,
    /// <summary>
    ///     OpCode used when a client requests metadata details or data<br />
    ///     Hex value: 0x7B
    /// </summary>
    MetaDataRequest = 123
}

/// <summary>
///     OpCodes used when sending packets to a client
/// </summary>
/// <remarks>
///     In networking, an opcode is used to identify the type of packet that is being sent. The opcode is generally part of
///     a header near the
///     beginning of the data stream
/// </remarks>
public enum ServerOpCode : byte
{
    /// <summary>
    ///     OpCode used to send the encryption details and checksum of the details of available login servers<br />
    ///     Hex value: 0x00
    /// </summary>
    ConnectionInfo = 0,
    /// <summary>
    ///     OpCode used to send a message to a client on the login server<br />
    ///     Hex value: 0x02
    /// </summary>
    LoginMessage = 2,
    /// <summary>
    ///     OpCode used to redirect a client to another server<br />
    ///     Hex value: 0x03
    /// </summary>
    Redirect = 3,
    /// <summary>
    ///     OpCode used to send a client it's location<br />
    ///     Hex value: 0x04
    /// </summary>
    Location = 4,
    /// <summary>
    ///     OpCode used to send a client it's id<br />
    ///     Hex value: 0x05
    /// </summary>
    UserId = 5,
    /// <summary>
    ///     OpCode used to send a client all non-aisling objects in it's viewport<br />
    ///     Hex value: 0x07
    /// </summary>
    DisplayVisibleEntities = 7,
    /// <summary>
    ///     OpCode used to send a client it's attributes<br />
    ///     Hex value: 0x08
    /// </summary>
    Attributes = 8,
    /// <summary>
    ///     OpCode used to send a client a non-public message<br />
    ///     Hex value: 0x0A
    /// </summary>
    ServerMessage = 10,
    /// <summary>
    ///     OpCode used to respond to a client's request to walk<br />
    ///     Hex value: 0x0B
    /// </summary>
    ConfirmClientWalk = 11,
    /// <summary>
    ///     OpCode used to send a client another creature's walk<br />
    ///     Hex value: 0x0C
    /// </summary>
    CreatureWalk = 12,
    /// <summary>
    ///     OpCode used to send a client a public message<br />
    ///     Hex value: 0x0D
    /// </summary>
    PublicMessage = 13,
    /// <summary>
    ///     OpCode used to remove an object from the client's viewport<br />
    ///     Hex value: 0x0E
    /// </summary>
    RemoveObject = 14,
    /// <summary>
    ///     OpCode used to add an item to the client's inventory<br />
    ///     Hex value: 0x0F
    /// </summary>
    AddItemToPane = 15,
    /// <summary>
    ///     OpCode used to remove an item from the client's inventory<br />
    ///     Hex value: 0x10
    /// </summary>
    RemoveItemFromPane = 16,
    /// <summary>
    ///     OpCode used to send a client another creature's turn<br />
    ///     Hex value: 0x11
    /// </summary>
    CreatureTurn = 17,
    /// <summary>
    ///     OpCode used to display a creature's health bar<br />
    ///     Hex value: 0x13
    /// </summary>
    HealthBar = 19,
    /// <summary>
    ///     OpCode used to send map details to a client<br />
    ///     Hex value: 0x15
    /// </summary>
    MapInfo = 21,
    /// <summary>
    ///     OpCode used to add a spell to the client's spellbook<br />
    ///     Hex value: 0x17
    /// </summary>
    AddSpellToPane = 23,
    /// <summary>
    ///     OpCode used to remove a spell from the client's spellbook<br />
    ///     Hex value: 0x18
    /// </summary>
    RemoveSpellFromPane = 24,
    /// <summary>
    ///     OpCode used to tell the client to play a sound<br />
    ///     Hex value: 0x19
    /// </summary>
    Sound = 25,
    /// <summary>
    ///     OpCode used to animate a creature's body<br />
    ///     Hex value: 0x1A
    /// </summary>
    BodyAnimation = 26,
    /// <summary>
    ///     OpCode used to associate text with an object<br />
    ///     Hex value: 0x1B
    /// </summary>
    Notepad = 27,
    /// <summary>
    ///     OpCode used to signal to the client that a map change operation has completed<br />
    ///     Hex value: 0x1F
    /// </summary>
    MapChangeComplete = 31,
    /// <summary>
    ///     OpCode used to change the light level of the map<br />
    ///     Hex value: 0x20
    /// </summary>
    LightLevel = 32,
    /// <summary>
    ///     OpCode used to respond to a client's request to refresh the viewport<br />
    ///     Hex value: 0x22
    /// </summary>
    RefreshResponse = 34,
    /// <summary>
    ///     OpCode used to play an animation on a point or object<br />
    ///     Hex value: 0x29
    /// </summary>
    Animation = 41,
    /// <summary>
    ///     OpCode used to add a skill to a client's skillbook<br />
    ///     Hex value: 0x2C
    /// </summary>
    AddSkillToPane = 44,
    /// <summary>
    ///     OpCode used to remove a skill from a client's skillbook<br />
    ///     Hex value: 0x2D
    /// </summary>
    RemoveSkillFromPane = 45,
    /// <summary>
    ///     OpCode used to transition a client to the world map<br />
    ///     Hex value: 0x2E
    /// </summary>
    WorldMap = 46,
    /// <summary>
    ///     OpCode used to display a merchant menu to a client<br />
    ///     Hex value: 0x2F
    /// </summary>
    Menu = 47,
    /// <summary>
    ///     OpCode used to display a dialog to a client<br />
    ///     Hex value: 0x30
    /// </summary>
    Dialog = 48,
    /// <summary>
    ///     OpCode used to display a board to a client<br />
    ///     Hex value: 0x31
    /// </summary>
    Board = 49,
    /// <summary>
    ///     OpCode used to give details of nearby doors to a client<br />
    ///     Hex value: 0x32
    /// </summary>
    Door = 50,
    /// <summary>
    ///     OpCode used to display an aisling to a client<br />
    ///     Hex value: 0x33
    /// </summary>
    DisplayAisling = 51,
    /// <summary>
    ///     OpCode used to display an aisling's profile to a client<br />
    ///     Hex value: 0x34
    /// </summary>
    Profile = 52,
    /// <summary>
    ///     OpCode used to display the world list to a client<br />
    ///     Hex value: 0x36
    /// </summary>
    WorldList = 54,
    /// <summary>
    ///     OpCode used to send a client a change in an equipment slot<br />
    ///     Hex value: 0x37
    /// </summary>
    Equipment = 55,
    /// <summary>
    ///     OpCode used to send a client a removal from an equipment slot<br />
    ///     Hex value: 0x38
    /// </summary>
    Unequip = 56,
    /// <summary>
    ///     OpCode used to send a client it's own profile<br />
    ///     Hex value: 0x39
    /// </summary>
    SelfProfile = 57,
    /// <summary>
    ///     OpCode used to display an effect on the bar on the right hand side of the viewport<br />
    ///     Hex value: 0x3A
    /// </summary>
    Effect = 58,
    /// <summary>
    ///     OpCode used to respond to a client's heartbeat<br />
    ///     Hex value: 0x3B
    /// </summary>
    HeartBeatResponse = 59,
    /// <summary>
    ///     OpCode used to send a client tile data for a map<br />
    ///     Hex value: 0x3C
    /// </summary>
    MapData = 60,
    /// <summary>
    ///     OpCode used to send a skill or spell cooldown to a client<br />
    ///     Hex value: 0x3F
    /// </summary>
    Cooldown = 63,
    /// <summary>
    ///     OpCode used to send data displayed int an exchange window to a client<br />
    ///     Hex value: 0x42
    /// </summary>
    Exchange = 66,
    /// <summary>
    ///     OpCode used to tell a client to cancel a spellcast<br />
    ///     Hex value: 0x48
    /// </summary>
    CancelCasting = 72,
    /// <summary>
    ///     OpCode used to request profile details from a client<br />
    ///     Hex value: 0x49
    /// </summary>
    ProfileRequest = 73,
    /// <summary>
    ///     OpCode used to force a client to send back a specified packet<br />
    ///     Hex value: 0x4B
    /// </summary>
    ForceClientPacket = 75,
    /// <summary>
    ///     OpCode used to send a client confirmation of a request to exit to the login server<br />
    ///     Hex value: 0x4C
    /// </summary>
    ConfirmExit = 76,
    /// <summary>
    ///     OpCode used to send a client a list of available servers<br />
    ///     Hex value: 0x56
    /// </summary>
    ServerTable = 86,
    /// <summary>
    ///     OpCode used to signal a client that it has finished sending map data<br />
    ///     Hex value: 0x58
    /// </summary>
    MapLoadComplete = 88,
    /// <summary>
    ///     OpCode used to send a client the EULA / login notice<br />
    ///     Hex value: 0x60
    /// </summary>
    LoginNotice = 96,
    /// <summary>
    ///     OpCode used to send a client a group request<br />
    ///     Hex value: 0x63
    /// </summary>
    GroupRequest = 99,
    /// <summary>
    ///     OpCode used to send a client data for the lobby<br />
    ///     Hex value: 0x66
    /// </summary>
    LoginControls = 102,
    /// <summary>
    ///     OpCode used to signal a client that it is changing maps<br />
    ///     Hex value: 0x67
    /// </summary>
    MapChangePending = 103,
    /// <summary>
    ///     OpCode used to synchronize the server's ticks with a client's<br />
    ///     Hex value: 0x68
    /// </summary>
    SynchronizeTicks = 104,
    /// <summary>
    ///     OpCode used to send metadata data to a client<br />
    ///     Hex value: 0x6F
    /// </summary>
    MetaData = 111,
    /// <summary>
    ///     OpCode sent to a client to confirm their initial connection<br />
    ///     Hex value: 0x7E
    /// </summary>
    AcceptConnection = 126
}