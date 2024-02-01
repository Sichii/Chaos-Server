using Chaos.Packets;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Defines a pattern for a client that connects to an <see cref="IServer{T}" />
/// </summary>
public interface IClient : ISocketClient
{
    /// <summary>
    ///     The server is sending the client a confirmation that a connection has been established
    /// </summary>
    ValueTask OnAcceptConnection(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about an item to add to the inventory
    /// </summary>
    ValueTask OnAddItemToPane(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about a skill to add to the skillbook
    /// </summary>
    ValueTask OnAddSkillToPane(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about a spell to add to the spellbook
    /// </summary>
    ValueTask OnAddSpellToPane(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about an animation to play
    /// </summary>
    ValueTask OnAnimation(in Packet packet);

    /// <summary>
    ///     The world server is sending the client the aisling's attributes
    /// </summary>
    ValueTask OnAttributes(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details used to animate a creature's body
    /// </summary>
    ValueTask OnBodyAnimation(in Packet packet);

    /// <summary>
    ///     The world server is sending the client an instruction to cancel the currently casting spell
    /// </summary>
    ValueTask OnCancelCasting(in Packet packet);

    /// <summary>
    ///     The world server is sending the client a confirmation that the aisling's attempt to walk was successful
    /// </summary>
    ValueTask OnClientWalkResponse(in Packet packet);

    /// <summary>
    ///     The login server is sending the client encryption details and the server table hash
    /// </summary>
    ValueTask OnConnectionInfo(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about a skill or spell cooldown
    /// </summary>
    ValueTask OnCooldown(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about a creature turning
    /// </summary>
    ValueTask OnCreatureTurn(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about another creature walking
    /// </summary>
    ValueTask OnCreatureWalk(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about all visible aislings
    /// </summary>
    ValueTask OnDisplayAislings(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about a board interaction
    /// </summary>
    ValueTask OnDisplayBoard(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about a dialog interaction
    /// </summary>
    ValueTask OnDisplayDialog(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about an exchange interaction
    /// </summary>
    ValueTask OnDisplayExchage(in Packet packet);

    /// <summary>
    ///     The world server is sending the client a group invite
    /// </summary>
    ValueTask OnDisplayGroupInvite(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about a menu interaction
    /// </summary>
    ValueTask OnDisplayMenu(in Packet packet);

    /// <summary>
    ///     The world server is sending the client a public message
    /// </summary>
    ValueTask OnDisplayPublicMessage(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about an item being unequipped
    /// </summary>
    ValueTask OnDisplayUnequip(in Packet packet);

    /// <summary>
    ///     The world server is sending the client a collection of entities that are visible to the aisling
    /// </summary>
    ValueTask OnDisplayVisibleEntities(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about doors visible to the aisling
    /// </summary>
    ValueTask OnDoor(in Packet packet);

    /// <summary>
    ///     The world server is sending the client a request for profile data
    /// </summary>
    ValueTask OnEditableProfileRequest(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about an effect to display in the effect bar
    /// </summary>
    ValueTask OnEffect(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about an item being equipped
    /// </summary>
    ValueTask OnEquipment(in Packet packet);

    /// <summary>
    ///     The world server is sending the client a confirmation for it's request to exit the world
    /// </summary>
    ValueTask OnExitResponse(in Packet packet);

    /// <summary>
    ///     The world server is sending the client data for a packet it wants the client to send back to it
    /// </summary>
    ValueTask OnForceClientPacket(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about a creature's health bar
    /// </summary>
    ValueTask OnHealthBar(in Packet packet);

    /// <summary>
    ///     The world server is sending the client a keep-alive heartbeat
    /// </summary>
    ValueTask OnHeartBeatResponse(in Packet packet);

    /// <summary>
    ///     The world server is sending the client the time of day, which controls the lighting on the map
    /// </summary>
    ValueTask OnLightLevel(in Packet packet);

    /// <summary>
    ///     The world server is sending the client the aisling's map location
    /// </summary>
    ValueTask OnLocation(in Packet packet);

    /// <summary>
    ///     The login server is sending the client some background data
    /// </summary>
    ValueTask OnLoginControl(in Packet packet);

    /// <summary>
    ///     The login server is sending the client a message
    /// </summary>
    ValueTask OnLoginMessage(in Packet packet);

    /// <summary>
    ///     The login server is sending the client the eula displayed at login
    /// </summary>
    ValueTask OnLoginNotice(in Packet packet);

    /// <summary>
    ///     The world server is sending the client a notification that it's done sending data needed for a map change
    /// </summary>
    ValueTask OnMapChangeComplete(in Packet packet);

    /// <summary>
    ///     The world server is sending the client an indication that a map change is about to start
    /// </summary>
    ValueTask OnMapChangePending(in Packet packet);

    /// <summary>
    ///     The world server is sending the client the data of a map it doesnt have
    /// </summary>
    ValueTask OnMapData(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about the current map
    /// </summary>
    ValueTask OnMapInfo(in Packet packet);

    /// <summary>
    ///     The world server is sending the client an indication that it has finished sending map data
    /// </summary>
    ValueTask OnMapLoadComplete(in Packet packet);

    /// <summary>
    ///     The login or world server is sending the client a request for metadata or checking it's current metadata
    /// </summary>
    ValueTask OnMetaData(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about an editable panel attached to an entity
    /// </summary>
    ValueTask OnNotepad(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about another aisling's profile
    /// </summary>
    ValueTask OnOtherProfile(in Packet packet);

    /// <summary>
    ///     The server is sending the client details about another server that the client needs to connect to
    /// </summary>
    ValueTask OnRedirect(in Packet packet);

    /// <summary>
    ///     The world server is sending the client a notification that the refresh is complete
    /// </summary>
    ValueTask OnRefreshResponse(in Packet packet);

    /// <summary>
    ///     The world server is sending the client the id of an entity to remove from the screen
    /// </summary>
    ValueTask OnRemoveEntity(in Packet packet);

    /// <summary>
    ///     The world server is sending the client the slot of an item to remove from the inventory
    /// </summary>
    ValueTask OnRemoveItemFromPane(in Packet packet);

    /// <summary>
    ///     The world server is sending the client the slot of a skill to remove from the skillbook
    /// </summary>
    ValueTask OnRemoveSkillFromPane(in Packet packet);

    /// <summary>
    ///     The world server is sending the client the slot of a spell to remove from the spellbook
    /// </summary>
    ValueTask OnRemoveSpellFromPane(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about the aisling's profile
    /// </summary>
    ValueTask OnSelfProfile(in Packet packet);

    /// <summary>
    ///     The world server is sending the client a private message
    /// </summary>
    ValueTask OnServerMessage(in Packet packet);

    /// <summary>
    ///     The lobby server is sending the client the server table data
    /// </summary>
    ValueTask OnServerTable(in Packet packet);

    /// <summary>
    ///     The world server is sending the client the id of a sound or music to play
    /// </summary>
    ValueTask OnSound(in Packet packet);

    /// <summary>
    ///     The server is sending the client a value used to gauge latency
    /// </summary>
    ValueTask OnSynchronizeTicks(in Packet packet);

    /// <summary>
    ///     The world server is sending the client it's id and some details about the aisling
    /// </summary>
    ValueTask OnUserId(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details about all logged on aislings
    /// </summary>
    ValueTask OnWorldList(in Packet packet);

    /// <summary>
    ///     The world server is sending the client details required to display a world map
    /// </summary>
    ValueTask OnWorldMap(in Packet packet);
}