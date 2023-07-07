using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Board;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.Panel.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Packets;
using Chaos.Services.Storage.Abstractions;

namespace Chaos.Networking.Abstractions;

public interface IWorldClient : ISocketClient
{
    Aisling Aisling { get; set; }
    void SendAddItemToPane(Item item);
    void SendAddSkillToPane(Skill skill);
    void SendAddSpellToPane(Spell spell);
    void SendAnimation(Animation animation);
    void SendAttributes(StatUpdateType statUpdateType);
    void SendBoard(BoardBase boardBase, short? startPostId = null);
    void SendBoardList(IEnumerable<BoardBase> boards);
    void SendBoardResponse(BoardOrResponseType responseType, string message, bool success);

    void SendBodyAnimation(
        uint id,
        BodyAnimation bodyAnimation,
        ushort speed,
        byte? sound = null
    );

    void SendCancelCasting();
    void SendConfirmClientWalk(Point oldPoint, Direction direction);
    void SendConfirmExit();
    void SendCooldown(PanelEntityBase panelEntityBase);
    void SendCreatureTurn(uint id, Direction direction);
    void SendCreatureWalk(uint id, Point startPoint, Direction direction);
    void SendDialog(Dialog dialog);
    void SendDisplayAisling(Aisling aisling);
    void SendDoors(IEnumerable<Door> doors);
    void SendEffect(EffectColor effectColor, byte effectIcon);
    void SendEquipment(Item item);
    void SendExchangeAccepted(bool persistExchange);
    void SendExchangeAddItem(bool rightSide, byte index, Item item);
    void SendExchangeCancel(bool rightSide);
    void SendExchangeRequestAmount(byte slot);
    void SendExchangeSetGold(bool rightSide, int amount);
    void SendExchangeStart(Aisling fromAisling);
    void SendForcedClientPacket(ref ClientPacket clientPacket);
    void SendGroupRequest(GroupRequestType groupRequestType, string fromName);
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
        string message
    );

    void SendPost(Post post, bool isMail, bool enablePrevBtn = true);

    void SendProfile(Aisling aisling);
    void SendProfileRequest();
    void SendPublicMessage(uint id, PublicMessageType publicMessageType, string message);
    void SendRefreshResponse();
    void SendRemoveItemFromPane(byte slot);
    void SendRemoveObject(uint id);
    void SendRemoveSkillFromPane(byte slot);
    void SendRemoveSpellFromPane(byte slot);
    void SendSelfProfile();
    void SendServerMessage(ServerMessageType serverMessageType, string message);
    void SendSound(byte sound, bool isMusic);
    void SendUnequip(EquipmentSlot equipmentSlot);
    void SendUserId();
    void SendVisibleEntities(IEnumerable<VisibleEntity> objects);
    void SendWorldList(IEnumerable<Aisling> users);
    void SendWorldMap(WorldMap worldMap);
}