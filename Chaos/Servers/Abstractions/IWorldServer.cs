using Chaos.Clients.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Packets;

namespace Chaos.Servers.Abstractions;

public interface IWorldServer : IServer<IWorldClient>
{
    ValueTask OnBeginChant(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnBoardRequest(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnChant(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnClick(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnClientRedirected(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnClientWalk(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnDialogResponse(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnEmote(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnExchange(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnExitRequest(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnGoldDropped(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnGoldDroppedOnCreature(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnGroupRequest(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnIgnore(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnItemDropped(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnItemDroppedOnCreature(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnMapDataRequest(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnMetafileRequest(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnPickup(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnProfile(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnProfileRequest(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnPublicMessage(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnPursuitRequest(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnRaiseStat(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnRefreshRequest(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnSocialStatus(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnSpacebar(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnSwapSlot(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnToggleGroup(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnTurn(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnUnequip(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnUseItem(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnUserOptionToggle(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnUseSkill(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnUseSpell(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnWhisper(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnWorldListRequest(IWorldClient client, in ClientPacket clientPacket);
    ValueTask OnWorldMapClick(IWorldClient client, in ClientPacket clientPacket);
}