#region
using Chaos.Collections;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class GroupTests
{
    private Group CreateGroup()
    {
        var map = MockMapInstance.Create();
        var sender = MockAisling.Create(map, "Leader");
        var receiver = MockAisling.Create(map, "Member1");
        var channelService = MockChannelService.Create();
        var logger = MockLogger.Create<Group>();

        var group = new Group(
            sender,
            receiver,
            channelService,
            logger.Object);

        sender.Group = group;
        receiver.Group = group;

        return group;
    }

    #region Enumeration
    [Test]
    public void GetEnumerator_ShouldReturnAllMembers()
    {
        var group = CreateGroup();
        var map = group.Leader.MapInstance;
        var member2 = MockAisling.Create(map, "Member2");
        group.Add(member2);

        var members = group.ToList();

        members.Should()
               .HaveCount(3);

        members.Select(m => m.Name)
               .Should()
               .Contain("Leader")
               .And
               .Contain("Member1")
               .And
               .Contain("Member2");
    }
    #endregion

    [Test]
    public void Promote_Self_ShouldNotChangeLeader()
    {
        var group = CreateGroup();
        var leader = group.Leader;

        group.Promote(leader);

        // Promoting self should be a no-op (the "dumbly point to yourself" branch)
        group.Leader
             .Name
             .Should()
             .Be("Leader");
    }

    #region ToString
    [Test]
    public void ToString_ShouldMarkLeaderWithAsterisk()
    {
        var group = CreateGroup();

        var str = group.ToString();

        str.Should()
           .Contain("* Leader");
    }
    #endregion

    #region Construction
    [Test]
    public void Group_ShouldContainBothMembersAfterCreation()
    {
        var group = CreateGroup();

        group.Count
             .Should()
             .Be(2);
    }

    [Test]
    public void Group_ShouldSetSenderAsLeader()
    {
        var group = CreateGroup();

        group.Leader
             .Name
             .Should()
             .Be("Leader");
    }
    #endregion

    #region Add
    [Test]
    public void Add_ShouldIncreaseCount()
    {
        var group = CreateGroup();
        var map = group.Leader.MapInstance;
        var newMember = MockAisling.Create(map, "Member2");

        group.Add(newMember);
        newMember.Group = group;

        group.Count
             .Should()
             .Be(3);
    }

    [Test]
    public void Add_ShouldMakeNewMemberEnumerable()
    {
        var group = CreateGroup();
        var map = group.Leader.MapInstance;
        var newMember = MockAisling.Create(map, "Member2");

        group.Add(newMember);

        group.Should()
             .Contain(newMember);
    }
    #endregion

    #region Contains
    [Test]
    public void Contains_ShouldReturnTrue_ForExistingMember()
    {
        var group = CreateGroup();

        group.Contains(group.Leader)
             .Should()
             .BeTrue();
    }

    [Test]
    public void Contains_ShouldReturnFalse_ForNonMember()
    {
        var group = CreateGroup();
        var outsider = MockAisling.Create(name: "Outsider");

        group.Contains(outsider)
             .Should()
             .BeFalse();
    }
    #endregion

    #region Promote
    [Test]
    public void Promote_ShouldChangeLeader()
    {
        var group = CreateGroup();
        var members = group.ToList();
        var member1 = members.First(a => a.Name == "Member1");

        group.Promote(member1);

        group.Leader
             .Name
             .Should()
             .Be("Member1");
    }

    [Test]
    public void Promote_ShouldKeepSameCount()
    {
        var group = CreateGroup();
        var members = group.ToList();
        var member1 = members.First(a => a.Name == "Member1");

        group.Promote(member1);

        group.Count
             .Should()
             .Be(2);
    }
    #endregion

    #region Leave
    [Test]
    public void Leave_ByNonLeader_ShouldDisbandWhenOnlyTwoMembers()
    {
        var group = CreateGroup();
        var members = group.ToList();
        var member1 = members.First(a => a.Name == "Member1");

        group.Leave(member1);

        // With only 2 members, leaving disbands the group
        group.Count
             .Should()
             .Be(0);
    }

    [Test]
    public void Leave_ByLeader_ShouldTransferLeadership_WhenMoreThanTwoMembers()
    {
        var group = CreateGroup();
        var map = group.Leader.MapInstance;
        var member2 = MockAisling.Create(map, "Member2");
        group.Add(member2);
        member2.Group = group;

        var leader = group.Leader;
        group.Leave(leader);

        group.Count
             .Should()
             .Be(2);

        group.Leader
             .Name
             .Should()
             .NotBe("Leader");
    }
    #endregion

    #region Kick
    [Test]
    public void Kick_LeaderKicksSelf_ShouldLeaveGroup_WhenMoreThanTwoMembers()
    {
        var group = CreateGroup();
        var map = group.Leader.MapInstance;
        var member2 = MockAisling.Create(map, "Member2");
        group.Add(member2);
        member2.Group = group;

        var leader = group.Leader;

        // When leader kicks themselves, it delegates to Leave
        group.Kick(leader);

        group.Contains(leader)
             .Should()
             .BeFalse();

        leader.Group
              .Should()
              .BeNull();

        group.Count
             .Should()
             .Be(2);
    }

    [Test]
    public void Kick_ShouldRemoveMember_WhenMoreThanTwoMembers()
    {
        var group = CreateGroup();
        var map = group.Leader.MapInstance;
        var member2 = MockAisling.Create(map, "Member2");
        group.Add(member2);
        member2.Group = group;

        var members = group.ToList();
        var member1 = members.First(a => a.Name == "Member1");

        group.Kick(member1);

        group.Contains(member1)
             .Should()
             .BeFalse();

        group.Count
             .Should()
             .Be(2);
    }

    [Test]
    public void Kick_ShouldDisbandGroup_WhenOnlyTwoMembers()
    {
        var group = CreateGroup();
        var members = group.ToList();
        var member1 = members.First(a => a.Name == "Member1");

        group.Kick(member1);

        group.Count
             .Should()
             .Be(0);
    }
    #endregion
}