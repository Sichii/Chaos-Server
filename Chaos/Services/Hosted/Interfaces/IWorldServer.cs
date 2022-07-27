using System.Threading.Tasks;
using Chaos.Clients.Interfaces;
using Chaos.Networking.Interfaces;
using Chaos.Packets;

namespace Chaos.Services.Hosted.Interfaces;

public interface IWorldServer : IServer
{
    ValueTask OnBeginChant(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnBoardRequest(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnChant(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnClick(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnClientRedirected(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnClientWalk(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnDialogResponse(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnEmote(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnExchange(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnExitRequest(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnGoldDropped(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnGoldDroppedOnCreature(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnGroupRequest(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnIgnore(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnItemDropped(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnItemDroppedOnCreature(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnMapDataRequest(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnMetafileRequest(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnPickup(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnProfile(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnProfileRequest(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnPublicMessage(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnPursuitRequest(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnRaiseStat(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnRefreshRequest(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnSocialStatus(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnSpacebar(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnSwapSlot(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnToggleGroup(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnTurn(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnUnequip(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnUseItem(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnUserOptionToggle(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnUseSkill(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnUseSpell(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnWhisper(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnWorldListRequest(IWorldClient client, ref ClientPacket clientPacket);
    ValueTask OnWorldMapClick(IWorldClient client, ref ClientPacket clientPacket);
}