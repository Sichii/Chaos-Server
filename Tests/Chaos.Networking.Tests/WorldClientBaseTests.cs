#region
using Chaos.DarkAges.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Networking.Tests;

public sealed class WorldClientBaseTests
{
    [Test]
    public void SendAddItemToPane_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new AddItemToPaneArgs();
        client.SendAddItemToPane(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendAddSkillToPane_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new AddSkillToPaneArgs();
        client.SendAddSkillToPane(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendAddSpellToPane_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new AddSpellToPaneArgs();
        client.SendAddSpellToPane(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendAnimation_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new AnimationArgs();
        client.SendAnimation(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendAttributes_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new AttributesArgs();
        client.SendAttributes(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendBodyAnimation_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new BodyAnimationArgs();
        client.SendBodyAnimation(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendCancelCasting_ShouldSendCancelCastingArgs()
    {
        var client = MockWorldClient.Create();

        client.SendCancelCasting();

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeOfType<CancelCastingArgs>();
    }

    [Test]
    public void SendClientWalkResponse_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new ClientWalkResponseArgs();
        client.SendClientWalkResponse(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendCooldown_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new CooldownArgs();
        client.SendCooldown(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendCreatureTurn_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new CreatureTurnArgs();
        client.SendCreatureTurn(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendCreatureWalk_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new CreatureWalkArgs();
        client.SendCreatureWalk(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendDisplayAisling_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new DisplayAislingArgs();
        client.SendDisplayAisling(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendDisplayBoard_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new DisplayBoardArgs();
        client.SendDisplayBoard(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendDisplayDialog_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new DisplayDialogArgs
        {
            DialogType = DialogType.Normal
        };
        client.SendDisplayDialog(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendDisplayEditableNotepad_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new DisplayEditableNotepadArgs();
        client.SendDisplayEditableNotepad(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendDisplayExchange_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new DisplayExchangeArgs();
        client.SendDisplayExchange(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendDisplayGroupInvite_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new DisplayGroupInviteArgs();
        client.SendDisplayGroupInvite(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendDisplayPublicMessage_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new DisplayPublicMessageArgs();
        client.SendDisplayPublicMessage(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendDisplayReadonlyNotepad_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new DisplayReadonlyNotepadArgs();
        client.SendDisplayReadonlyNotepad(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendDisplayUnequip_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new DisplayUnequipArgs();
        client.SendDisplayUnequip(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendDisplayVisibleEntities_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new DisplayVisibleEntitiesArgs();
        client.SendDisplayVisibleEntities(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendDoors_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new DoorArgs();
        client.SendDoors(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendEditableProfileRequest_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        client.SendEditableProfileRequest();

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeOfType<EditableProfileRequestArgs>();
    }

    [Test]
    public void SendEffect_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new EffectArgs();
        client.SendEffect(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendEquipment_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new EquipmentArgs();
        client.SendEquipment(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendExitResponse_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new ExitResponseArgs();
        client.SendExitResponse(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendForceClientPacket_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new ForceClientPacketArgs();
        client.SendForceClientPacket(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendHealthBar_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new HealthBarArgs();
        client.SendHealthBar(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendLightLevel_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new LightLevelArgs();
        client.SendLightLevel(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendLocation_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new LocationArgs();
        client.SendLocation(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendMapChangeComplete_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        client.SendMapChangeComplete();

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeOfType<MapChangeCompleteArgs>();
    }

    [Test]
    public void SendMapChangePending_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        client.SendMapChangePending();

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeOfType<MapChangePendingArgs>();
    }

    [Test]
    public void SendMapData_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new MapDataArgs();
        client.SendMapData(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendMapInfo_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new MapInfoArgs();
        client.SendMapInfo(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendMapLoadComplete_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        client.SendMapLoadComplete();

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeOfType<MapLoadCompleteArgs>();
    }

    [Test]
    public void SendMetaData_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new MetaDataArgs();
        client.SendMetaData(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendNotepad_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new DisplayEditableNotepadArgs();
        client.SendNotepad(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendOtherProfile_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new OtherProfileArgs();
        client.SendOtherProfile(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendRefreshResponse_ShouldSendRefreshResponseArgs()
    {
        var client = MockWorldClient.Create();

        client.SendRefreshResponse();

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeOfType<RefreshResponseArgs>();
    }

    [Test]
    public void SendRemoveEntity_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new RemoveEntityArgs();
        client.SendRemoveEntity(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendRemoveItemFromPane_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new RemoveItemFromPaneArgs();
        client.SendRemoveItemFromPane(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendRemoveSkillFromPane_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new RemoveSkillFromPaneArgs();
        client.SendRemoveSkillFromPane(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendRemoveSpellFromPane_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new RemoveSpellFromPaneArgs();
        client.SendRemoveSpellFromPane(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendSelfProfile_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new SelfProfileArgs();
        client.SendSelfProfile(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendServerMessage_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new ServerMessageArgs();
        client.SendServerMessage(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendSound_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new SoundArgs();
        client.SendSound(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendUserId_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new UserIdArgs();
        client.SendUserId(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendWorldList_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new WorldListArgs();
        client.SendWorldList(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendWorldMap_ShouldSendArgs()
    {
        var client = MockWorldClient.Create();

        var args = new WorldMapArgs();
        client.SendWorldMap(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }
}