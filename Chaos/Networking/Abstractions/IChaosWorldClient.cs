#region
using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Board;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.Panel.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets;
using Chaos.Services.Storage.Abstractions;
#endregion

namespace Chaos.Networking.Abstractions;

public interface IChaosWorldClient : IConnectedClient
{
    Aisling Aisling { get; set; }

    /// <summary>
    ///     The first byte of the heartbeat
    /// </summary>
    byte? Heartbeat1 { get; set; }

    /// <summary>
    ///     The second byte of the heartbeat
    /// </summary>
    byte? Heartbeat2 { get; set; }

    /// <summary>
    ///     Client side id of the client
    /// </summary>
    uint LoginId1 { get; set; }

    /// <summary>
    ///     Client side id of the client (2)
    /// </summary>
    ushort LoginId2 { get; set; }

    void SendAddItemToPane(Item item);
    void SendAddSkillToPane(Skill skill);
    void SendAddSpellToPane(Spell spell);
    void SendAnimation(Animation animation);
    void SendAttributes(StatUpdateType statUpdateType);
    void SendBoardList(IEnumerable<BoardBase> boards);
    void SendBoardResponse(BoardOrResponseType responseType, string message, bool success);

    void SendBodyAnimation(
        uint id,
        BodyAnimation bodyAnimation,
        ushort speed,
        byte? sound = null);

    void SendCancelCasting();
    void SendClientWalkResponse(Point oldPoint, Direction direction);
    void SendCooldown(PanelEntityBase panelEntityBase);
    void SendCreatureTurn(uint id, Direction direction);
    void SendCreatureWalk(uint id, Point startPoint, Direction direction);
    void SendDisplayAisling(Aisling aisling);
    void SendDisplayBoard(BoardBase boardBase, short? startPostId = null);
    void SendDisplayDialog(Dialog dialog);
    void SendDisplayGroupInvite(ServerGroupSwitch serverGroupSwitch, string fromName, DisplayGroupBoxInfo? groupBoxInfo = null);

    /// <summary>
    ///     Displays a notepad to the client
    /// </summary>
    /// <param name="type">
    ///     The type of notepad
    /// </param>
    /// <param name="item">
    ///     The item the notepad is attached to
    /// </param>
    /// <param name="width">
    ///     In game, the notepad will display with a character width close to (2.5 * thisValue) with midpoint rounding
    /// </param>
    /// <param name="height">
    ///     In game, the notepad will display with a line height close to (1.4 * thisValue) with midpoint rouding
    /// </param>
    void SendDisplayNotepad(
        NotepadType type,
        Item item,
        byte width,
        byte height);

    void SendDisplayPublicMessage(uint id, PublicMessageType publicMessageType, string message);
    void SendDisplayUnequip(EquipmentSlot equipmentSlot);
    void SendDoors(params IEnumerable<Door> doors);
    void SendEditableProfileRequest();
    void SendEffect(EffectColor effectColor, byte effectIcon);
    void SendEquipment(Item item);
    void SendExchangeAccepted(bool persistExchange);
    void SendExchangeAddItem(bool rightSide, byte index, Item item);
    void SendExchangeCancel(bool rightSide);
    void SendExchangeRequestAmount(byte slot);
    void SendExchangeSetGold(bool rightSide, int amount);
    void SendExchangeStart(Aisling fromAisling);
    void SendExitResponse();
    void SendForceClientPacket(ref Packet packet);
    void SendHealthBar(Creature creature, byte? sound = null);
    void SendLightLevel(LightLevel lightLevel);
    void SendLocation();
    void SendMapChangeComplete();
    void SendMapChangePending();
    void SendMapData();
    void SendMapInfo();
    void SendMapLoadComplete();
    void SendMetaData(MetaDataRequestType metaDataRequestType, IMetaDataStore metaData, string? name = null);

    void SendNotepad(
        byte identifier,
        NotepadType type,
        byte height,
        byte width,
        string message);

    void SendOtherProfile(Aisling aisling);

    void SendPost(Post post, bool isMail, bool enablePrevBtn = true);
    void SendRefreshResponse();
    void SendRemoveEntity(uint id);
    void SendRemoveItemFromPane(byte slot);
    void SendRemoveSkillFromPane(byte slot);
    void SendRemoveSpellFromPane(byte slot);
    void SendSelfProfile();
    void SendServerMessage(ServerMessageType serverMessageType, string message);
    void SendSound(byte sound, bool isMusic);
    void SendUserId();
    void SendVisibleEntities(params IEnumerable<VisibleEntity> objects);
    void SendWorldList(IEnumerable<Aisling> users);
    void SendWorldMap(WorldMap worldMap);
}