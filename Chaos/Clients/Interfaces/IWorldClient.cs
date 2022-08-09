using Chaos.Containers;
using Chaos.Data;
using Chaos.Geometry.Definitions;
using Chaos.Networking.Definitions;
using Chaos.Networking.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Packets;
using Chaos.Services.Caches.Interfaces;

namespace Chaos.Clients.Interfaces;

public interface IWorldClient : ISocketClient
{
    Aisling Aisling { get; set; }
    void SendAddItemToPane(Item item);
    void SendAddSkillToPane(Skill skill);
    void SendAddSpellToPane(Spell spell);
    void SendAnimation(Animation animation);
    void SendAttributes(StatUpdateType statUpdateType);

    void SendBodyAnimation(
        uint id,
        BodyAnimation bodyAnimation,
        ushort speed,
        byte? sound = null
    );

    void SendCancelCasting();
    void SendConfirmClientWalk(Point oldPoint, Direction direction);
    void SendConfirmExit();
    void SendCooldown(PanelObjectBase panelObjectBase);
    void SendCreatureTurn(uint id, Direction direction);
    void SendCreatureWalk(uint id, Point startPoint, Direction direction);

    void SendDisplayAisling(Aisling aisling);

    //void SendMenu
    //void SendDialog
    //void SendBoard
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
    void SendMetafile(MetafileRequestType metafileRequestType, ISimpleCache<Metafile> metafile, string? name = null);
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
    void SendVisibleObjects(IEnumerable<VisibleEntity> objects);
    void SendWorldList(IEnumerable<Aisling> users);
    void SendWorldMap(WorldMap worldMap);
}