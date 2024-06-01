using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Represents a client that is connected to the world server.
/// </summary>
public interface IWorldClient : IConnectedClient
{
    /// <summary>
    ///     Sends a packet to display a item in a pane
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.AddItemToPane" />
    /// </remarks>
    void SendAddItemToPane(AddItemToPaneArgs args);

    /// <summary>
    ///     Sends a packet to display a skill in a pane
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.AddSkillToPane" />
    /// </remarks>
    void SendAddSkillToPane(AddSkillToPaneArgs args);

    /// <summary>
    ///     Sends a packet to display a spell in a pane
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.AddSpellToPane" />
    /// </remarks>
    void SendAddSpellToPane(AddSpellToPaneArgs args);

    /// <summary>
    ///     Sends a packet to display an animation.
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.Animation" />
    /// </remarks>
    void SendAnimation(AnimationArgs args);

    /// <summary>
    ///     Sends a packet to update the client's attributes
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.Attributes" />
    /// </remarks>
    void SendAttributes(AttributesArgs args);

    /// <summary>
    ///     Sends a packet to animate an aisling's body
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.BodyAnimation" />
    /// </remarks>
    void SendBodyAnimation(BodyAnimationArgs args);

    /// <summary>
    ///     Sends a packet signaling the client to cancel casting (chanting) of the current spell
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.CancelCasting" />
    /// </remarks>
    void SendCancelCasting();

    /// <summary>
    ///     Sends a packet to respond to a client's walk request
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.ClientWalkResponse" />
    /// </remarks>
    void SendClientWalkResponse(ClientWalkResponseArgs args);

    /// <summary>
    ///     Sends a packet to start the cooldown of a skill or spell
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.Cooldown" />
    /// </remarks>
    void SendCooldown(CooldownArgs args);

    /// <summary>
    ///     Sends a packet to turn a creature (that isnt the current user) in a specific direction
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.CreatureTurn" />
    /// </remarks>
    void SendCreatureTurn(CreatureTurnArgs args);

    /// <summary>
    ///     Sends a packet to move a creature (that isnt the current user) in a specific direction
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.CreatureWalk" />
    /// </remarks>
    void SendCreatureWalk(CreatureWalkArgs args);

    /// <summary>
    ///     Sends a packet to display an aisling
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.DisplayAisling" />
    /// </remarks>
    void SendDisplayAisling(DisplayAislingArgs args);

    /// <summary>
    ///     Sends a packet to display a board, list of boards, or a board response.
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.DisplayBoard" />
    /// </remarks>
    void SendDisplayBoard(DisplayBoardArgs args);

    /// <summary>
    ///     Sends a packet to display a dialog
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.DisplayDialog" />
    /// </remarks>
    void SendDisplayDialog(DisplayDialogArgs args);

    /// <summary>
    ///     Sends a packet to display, close, or update an exchange
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.DisplayExchange" />
    /// </remarks>
    void SendDisplayExchange(DisplayExchangeArgs args);

    /// <summary>
    ///     Sends a packet to display a group invite
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.DisplayGroupInvite" />
    /// </remarks>
    void SendDisplayGroupInvite(DisplayGroupInviteArgs args);

    /// <summary>
    ///     Sends a packet to display a public message. (Normal, Shout, Chant)
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.DisplayPublicMessage" />
    /// </remarks>
    void SendDisplayPublicMessage(DisplayPublicMessageArgs args);

    /// <summary>
    ///     Sends a packet to unequip an item (remove from equipment pane)
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.DisplayUnequip" />
    /// </remarks>
    void SendDisplayUnequip(DisplayUnequipArgs args);

    /// <summary>
    ///     Sends a packet to display visible entities to the client (Monsters, Merchants, GroundItems only)
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.DisplayVisibleEntities" />
    /// </remarks>
    void SendDisplayVisibleEntities(DisplayVisibleEntitiesArgs args);

    /// <summary>
    ///     Sends a packet to display doors
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.Door" />
    /// </remarks>
    void SendDoors(DoorArgs args);

    /// <summary>
    ///     Sends a packet to request the editable profile from the client. (Profile text and image)
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.EditableProfileRequest" />
    /// </remarks>
    void SendEditableProfileRequest();

    /// <summary>
    ///     Sends a packet to display an effect in the effect bar (top right of screen)
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.Effect" />
    /// </remarks>
    void SendEffect(EffectArgs args);

    /// <summary>
    ///     Sends a packet to display an item in the equipment pane
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.Equipment" />
    /// </remarks>
    void SendEquipment(EquipmentArgs args);

    /// <summary>
    ///     Sends a packet to respond to a client's exit request
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.ExitResponse" />
    /// </remarks>
    void SendExitResponse(ExitResponseArgs args);

    /// <summary>
    ///     Sends a packet to force a client to send a packet to the server
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.ForceClientPacket" />
    /// </remarks>
    void SendForceClientPacket(ForceClientPacketArgs args);

    /// <summary>
    ///     Sends a packet to display a health bar
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.HealthBar" />
    /// </remarks>
    void SendHealthBar(HealthBarArgs args);

    /// <summary>
    ///     Sends a packet to update the light level of the client (day/night)
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.LightLevel" />
    /// </remarks>
    void SendLightLevel(LightLevelArgs args);

    /// <summary>
    ///     Sends a packet to update the client's location
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.Location" />
    /// </remarks>
    void SendLocation(LocationArgs args);

    /// <summary>
    ///     Sends a packet to signal the client that a map change is complete
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.MapChangeComplete" />
    /// </remarks>
    void SendMapChangeComplete();

    /// <summary>
    ///     Sends a packet to signal the client that a map change is pending
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.MapChangePending" />
    /// </remarks>
    void SendMapChangePending();

    /// <summary>
    ///     Sends a packet to send map data to the client
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.MapData" />
    /// </remarks>
    void SendMapData(MapDataArgs args);

    /// <summary>
    ///     Sends a packet to send map information to the client
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.MapInfo" />
    /// </remarks>
    void SendMapInfo(MapInfoArgs args);

    /// <summary>
    ///     Sends a packet to signal the client that the map has finished loading
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.MapLoadComplete" />
    /// </remarks>
    void SendMapLoadComplete();

    /// <summary>
    ///     Sends meta data to the client
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.MetaData" />
    /// </remarks>
    void SendMetaData(MetaDataArgs args);

    /// <summary>
    ///     Sends a packet to display an editable notepad
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.Notepad" />
    /// </remarks>
    void SendNotepad(NotepadArgs args);

    /// <summary>
    ///     Sends a packet to display another player's profile
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.OtherProfile" />
    /// </remarks>
    void SendOtherProfile(OtherProfileArgs args);

    /// <summary>
    ///     Sends a packet to respond to a request to refresh the client. This is more for signalling that the refresh
    ///     operation has completed.
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.RefreshResponse" />
    /// </remarks>
    void SendRefreshResponse();

    /// <summary>
    ///     Sends a packet to remove an entity from the client
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.RemoveEntity" />
    /// </remarks>
    void SendRemoveEntity(RemoveEntityArgs args);

    /// <summary>
    ///     Sends a packet to remove an item from a pane
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.RemoveItemFromPane" />
    /// </remarks>
    void SendRemoveItemFromPane(RemoveItemFromPaneArgs args);

    /// <summary>
    ///     Sends a packet to remove a skill from a pane
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.RemoveSkillFromPane" />
    /// </remarks>
    void SendRemoveSkillFromPane(RemoveSkillFromPaneArgs args);

    /// <summary>
    ///     Sends a packet to remove a spell from a pane
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.RemoveSpellFromPane" />
    /// </remarks>
    void SendRemoveSpellFromPane(RemoveSpellFromPaneArgs args);

    /// <summary>
    ///     Sends a packet containing the client's profile
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.SelfProfile" />
    /// </remarks>
    void SendSelfProfile(SelfProfileArgs args);

    /// <summary>
    ///     Sends a packet to display a server message (Whisper, Guild, Group, OrangeBar, WhiteText, etc)
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.ServerMessage" />
    /// </remarks>
    void SendServerMessage(ServerMessageArgs args);

    /// <summary>
    ///     Sends a packet to play a sound (or music)
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.Sound" />
    /// </remarks>
    void SendSound(SoundArgs args);

    /// <summary>
    ///     Sends a packet to give the client it's user id. This is what the client uses to attach it's viewport to an aisling.
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.UserId" />
    /// </remarks>
    void SendUserId(UserIdArgs args);

    /// <summary>
    ///     Sends a packet to display a world list to the client
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.WorldList" />
    /// </remarks>
    void SendWorldList(WorldListArgs args);

    /// <summary>
    ///     Sends a packet to display a world map to the client
    /// </summary>
    /// <remarks>
    ///     Opcode: <see cref="ServerOpCode.WorldMap" />
    /// </remarks>
    void SendWorldMap(WorldMapArgs args);
}