using System.Collections.Generic;
using Chaos.Containers;
using Chaos.Core.Data;
using Chaos.Core.Definitions;
using Chaos.Core.Geometry;
using Chaos.DataObjects;
using Chaos.Effects.Abstractions;
using Chaos.Managers.Interfaces;
using Chaos.Networking.Interfaces;
using Chaos.Packets;
using Chaos.PanelObjects;
using Chaos.PanelObjects.Abstractions;
using Chaos.WorldObjects;
using Chaos.WorldObjects.Abstractions;

namespace Chaos.Clients.Interfaces;

public interface IWorldClient : ISocketClient
{
    User User { get; set; }
    void SendAddItemToPane(Item item);
    void SendAddSkillToPane(Skill skill);
    void SendAddSpellToPane(Spell spell);
    void SendAnimation(Animation animation);
    void SendAttributes(StatUpdateType statUpdateType);
    void SendBodyAnimation(uint id, BodyAnimation bodyAnimation, ushort speed, byte? sound = null);
    void SendCancelCasting();
    void SendConfirmClientWalk(Point oldPoint, Direction direction);
    void SendConfirmExit();
    void SendCooldown(PanelObjectBase panelObjectBase);
    void SendCreatureTurn(uint id, Direction direction);
    void SendCreatureWalk(uint id, Point point, Direction direction);

    void SendDisplayUser(User user);

    //void SendMenu
    //void SendDialog
    //void SendBoard
    void SendDoors(params Door[] doors);
    void SendEffect(EffectBase effect);
    void SendEquipment(Item item);
    void SendExchange(ExchangeResponseType exchangeResponseType);
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
    void SendProfile(User user);
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
    void SendVisibleObjects(params VisibleObject[] objects);
    void SendWorldList(ICollection<User> users);
    void SendWorldMap(WorldMap worldMap);
    void SendMetafile(MetafileRequestType metafileRequestType, ICacheManager<string, Metafile> metafileManager, string? name = null);
}