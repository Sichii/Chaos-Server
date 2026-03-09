#region
using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Services.Other.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class GroupServiceTests
{
    private readonly IGroupService GroupService;
    private readonly MapInstance SharedMap;

    public GroupServiceTests()
    {
        SharedMap = MockMapInstance.Create();
        GroupService = MockGroupService.Create();
    }

    private Aisling CreateAisling(string name) => MockAisling.Create(SharedMap, name, setup: a => a.Options.AllowGroup = true);

    #region Kick
    [Test]
    public void Kick_ShouldRemoveMember_WhenSenderIsLeader()
    {
        var leader = CreateAisling("Leader");
        var member = CreateAisling("Member");
        var third = CreateAisling("Third");

        GroupService.Invite(leader, member);
        GroupService.AcceptInvite(leader, member);
        leader.Group!.Add(third);
        third.Group = leader.Group;

        GroupService.Kick(leader, member);

        member.Group
              .Should()
              .BeNull();

        leader.Group
              .Should()
              .NotBeNull();

        leader.Group!.Contains(member)
              .Should()
              .BeFalse();
    }
    #endregion

    #region Promote
    [Test]
    public void Promote_ShouldChangeGroupLeader()
    {
        var leader = CreateAisling("Leader");
        var member = CreateAisling("Member");

        GroupService.Invite(leader, member);
        GroupService.AcceptInvite(leader, member);

        GroupService.Promote(leader, member);

        leader.Group!.Leader
              .Name
              .Should()
              .Be("Member");
    }
    #endregion

    #region Invite
    [Test]
    public void Invite_ShouldSendInvite_WhenBothPartiesAreUngrouped()
    {
        var sender = CreateAisling("Sender");
        var receiver = CreateAisling("Receiver");

        GroupService.Invite(sender, receiver);

        // The invite was created — verify by accepting it
        GroupService.AcceptInvite(sender, receiver);

        sender.Group
              .Should()
              .NotBeNull();

        receiver.Group
                .Should()
                .BeSameAs(sender.Group);
    }

    [Test]
    public void Invite_SelfInvite_ShouldLeaveGroup()
    {
        var sender = CreateAisling("Sender");
        var receiver = CreateAisling("Receiver");

        // Form a group first
        GroupService.Invite(sender, receiver);
        GroupService.AcceptInvite(sender, receiver);

        sender.Group
              .Should()
              .NotBeNull();

        // Add a third member so leaving doesn't disband
        var third = CreateAisling("Third");
        sender.Group!.Add(third);
        third.Group = sender.Group;

        // Self-invite = leave
        GroupService.Invite(sender, sender);

        sender.Group
              .Should()
              .BeNull();
    }
    #endregion

    #region AcceptInvite
    [Test]
    public void AcceptInvite_ShouldCreateNewGroup_WhenNeitherIsGrouped()
    {
        var sender = CreateAisling("Sender");
        var receiver = CreateAisling("Receiver");

        GroupService.Invite(sender, receiver);
        GroupService.AcceptInvite(sender, receiver);

        sender.Group
              .Should()
              .NotBeNull();

        receiver.Group
                .Should()
                .NotBeNull();

        sender.Group
              .Should()
              .BeSameAs(receiver.Group);

        sender.Group!.Count
              .Should()
              .Be(2);
    }

    [Test]
    public void AcceptInvite_ShouldAddToExistingGroup_WhenSenderHasGroup()
    {
        var leader = CreateAisling("Leader");
        var member1 = CreateAisling("Member1");
        var member2 = CreateAisling("Member2");

        // Leader invites member1, creating a group
        GroupService.Invite(leader, member1);
        GroupService.AcceptInvite(leader, member1);

        // Leader invites member2 into existing group
        GroupService.Invite(leader, member2);
        GroupService.AcceptInvite(leader, member2);

        leader.Group!.Count
              .Should()
              .Be(3);

        leader.Group
              .Should()
              .Contain(member2);
    }
    #endregion

    #region DetermineRequestType
    [Test]
    public void DetermineRequestType_ShouldReturnInvite_WhenPendingInviteExists()
    {
        var sender = CreateAisling("GroupedSender");
        var receiver = CreateAisling("UngroupedReceiver");

        // Invite creates a pending invite entry
        GroupService.Invite(sender, receiver);

        var requestType = GroupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .Be(IGroupService.RequestType.Invite);
    }

    [Test]
    public void DetermineRequestType_ShouldReturnRequestToJoin_WhenPendingRequestExists()
    {
        var sender = CreateAisling("UngroupedSender");
        var receiver = CreateAisling("GroupedReceiver");

        // Give receiver a group so RequestToJoin path is used
        var partner = CreateAisling("Partner");
        GroupService.Invite(receiver, partner);
        GroupService.AcceptInvite(receiver, partner);

        // RequestToJoin creates a pending request entry
        GroupService.RequestToJoin(sender, receiver);

        var requestType = GroupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .Be(IGroupService.RequestType.RequestToJoin);
    }

    [Test]
    public void DetermineRequestType_ShouldReturnNull_WhenNoPendingInvitesOrRequests()
    {
        var sender = CreateAisling("Sender");
        var receiver = CreateAisling("Receiver");

        var requestType = GroupService.DetermineRequestType(sender, receiver);

        requestType.Should()
                   .BeNull();
    }
    #endregion
}