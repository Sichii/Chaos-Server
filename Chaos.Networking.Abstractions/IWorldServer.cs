using Chaos.Packets;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Defines a pattern for a server that facilites all aspects of actual gameplay
/// </summary>
/// <typeparam name="TClient">
/// </typeparam>
public interface IWorldServer<in TClient> : IServer<TClient> where TClient: IServerClient
{
    /// <summary>
    ///     Occurs when a client begins casting a spell with cast lines
    /// </summary>
    ValueTask OnBeginChant(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client accesses a board or mail
    /// </summary>
    ValueTask OnBoardRequest(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client uses an ability that has chant lines
    /// </summary>
    ValueTask OnChant(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client clicks on an object
    /// </summary>
    ValueTask OnClick(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client is redirected to this server
    /// </summary>
    ValueTask OnClientRedirected(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client walks in a direction
    /// </summary>
    ValueTask OnClientWalk(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client responds to a dialog
    /// </summary>
    ValueTask OnDialogResponse(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client requests an object be re-sent
    /// </summary>
    ValueTask OnDisplayEntityRequest(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client uses an emote
    /// </summary>
    ValueTask OnEmote(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client interacts with an exchange window
    /// </summary>
    ValueTask OnExchange(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client tries to log out
    /// </summary>
    ValueTask OnExitRequest(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client drops gold on the ground
    /// </summary>
    ValueTask OnGoldDropped(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client drops gold on a creature
    /// </summary>
    ValueTask OnGoldDroppedOnCreature(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client invites another player to a group, responds to a group invite, or creates or destroys a group
    ///     box
    /// </summary>
    ValueTask OnGroupRequest(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client ignores or un-ignores another player, or requests a list of ignored players
    /// </summary>
    ValueTask OnIgnore(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client drops an item on the ground
    /// </summary>
    ValueTask OnItemDropped(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client drops an item on a creature
    /// </summary>
    ValueTask OnItemDroppedOnCreature(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client requests tile data for the current map
    /// </summary>
    ValueTask OnMapDataRequest(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client requests metadata details or data
    /// </summary>
    ValueTask OnMetaDataRequest(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client picks up an item from the ground
    /// </summary>
    ValueTask OnPickup(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client responds to a request for profile data (portrait, text)
    /// </summary>
    ValueTask OnProfile(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client requests the profile of another player
    /// </summary>
    ValueTask OnProfileRequest(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client sends a publicly visible message
    /// </summary>
    ValueTask OnPublicMessage(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client responds to a merchant menu
    /// </summary>
    ValueTask OnPursuitRequest(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client requests to raise a stat
    /// </summary>
    ValueTask OnRaiseStat(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client refreshes their viewport
    /// </summary>
    ValueTask OnRefreshRequest(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client changes their social status
    /// </summary>
    ValueTask OnSocialStatus(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client presses their spacebar
    /// </summary>
    ValueTask OnSpacebar(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client swaps two panel objects
    /// </summary>
    ValueTask OnSwapSlot(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client toggles their group availability
    /// </summary>
    ValueTask OnToggleGroup(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client changes their character's direction
    /// </summary>
    ValueTask OnTurn(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client unequips an item
    /// </summary>
    ValueTask OnUnequip(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client uses an item
    /// </summary>
    ValueTask OnUseItem(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client toggles a user option
    /// </summary>
    ValueTask OnUserOptionToggle(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client uses a skill
    /// </summary>
    ValueTask OnUseSkill(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client uses a spell
    /// </summary>
    ValueTask OnUseSpell(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client sends a private message to the server
    /// </summary>
    ValueTask OnWhisper(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client requests a list of all online players
    /// </summary>
    ValueTask OnWorldListRequest(TClient client, in Packet packet);

    /// <summary>
    ///     Occurs when a client clicks on a world map node
    /// </summary>
    ValueTask OnWorldMapClick(TClient client, in Packet packet);
}