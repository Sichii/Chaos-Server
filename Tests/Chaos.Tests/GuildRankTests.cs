#region
using Chaos.Collections;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class GuildRankTests
{
    #region GetMemberNames
    [Test]
    public void GetMemberNames_ShouldReturnAllMembers()
    {
        var rank = new GuildRank(
            "Members",
            2,
            [
                "Alice",
                "Bob"
            ]);

        rank.GetMemberNames()
            .Should()
            .BeEquivalentTo("Alice", "Bob");
    }
    #endregion

    #region Constructor
    [Test]
    public void Constructor_ShouldSetNameAndTier()
    {
        var rank = new GuildRank("Leader", 0);

        rank.Name
            .Should()
            .Be("Leader");

        rank.Tier
            .Should()
            .Be(0);

        rank.Count
            .Should()
            .Be(0);
    }

    [Test]
    public void Constructor_WithMembers_ShouldPopulate()
    {
        var rank = new GuildRank(
            "Members",
            2,
            [
                "Alice",
                "Bob"
            ]);

        rank.Count
            .Should()
            .Be(2);

        rank.HasMember("Alice")
            .Should()
            .BeTrue();

        rank.HasMember("Bob")
            .Should()
            .BeTrue();
    }
    #endregion

    #region AddMember / RemoveMember / HasMember
    [Test]
    public void AddMember_ShouldIncrementCount()
    {
        var rank = new GuildRank("Members", 2);

        rank.AddMember("Alice");

        rank.Count
            .Should()
            .Be(1);

        rank.HasMember("Alice")
            .Should()
            .BeTrue();
    }

    [Test]
    public void RemoveMember_ShouldDecrementCount()
    {
        var rank = new GuildRank("Members", 2, ["Alice"]);

        var result = rank.RemoveMember("Alice");

        result.Should()
              .BeTrue();

        rank.Count
            .Should()
            .Be(0);
    }

    [Test]
    public void RemoveMember_ShouldReturnFalse_WhenNotFound()
    {
        var rank = new GuildRank("Members", 2);

        var result = rank.RemoveMember("Nobody");

        result.Should()
              .BeFalse();
    }

    [Test]
    public void HasMember_ShouldBeCaseInsensitive()
    {
        var rank = new GuildRank("Members", 2, ["Alice"]);

        rank.HasMember("alice")
            .Should()
            .BeTrue();

        rank.HasMember("ALICE")
            .Should()
            .BeTrue();

        rank.HasMember("aLiCe")
            .Should()
            .BeTrue();
    }
    #endregion

    #region Tier Properties
    [Test]
    public void IsLeaderRank_ShouldBeTrue_WhenTierIsZero()
    {
        var rank = new GuildRank("Leader", 0);

        rank.IsLeaderRank
            .Should()
            .BeTrue();

        rank.IsOfficerRank
            .Should()
            .BeTrue();

        rank.CanBePromoted
            .Should()
            .BeFalse();

        rank.CanBeDemoted
            .Should()
            .BeTrue();
    }

    [Test]
    public void IsOfficerRank_ShouldBeTrue_WhenTierIsOne()
    {
        var rank = new GuildRank("Officer", 1);

        rank.IsLeaderRank
            .Should()
            .BeFalse();

        rank.IsOfficerRank
            .Should()
            .BeTrue();

        rank.CanBePromoted
            .Should()
            .BeTrue();

        rank.CanBeDemoted
            .Should()
            .BeTrue();
    }

    [Test]
    public void Tier3_ShouldNotBeDemotable()
    {
        var rank = new GuildRank("Initiate", 3);

        rank.CanBeDemoted
            .Should()
            .BeFalse();

        rank.CanBePromoted
            .Should()
            .BeTrue();
    }
    #endregion

    #region ChangeRankName
    [Test]
    public void ChangeRankName_ShouldUpdateName()
    {
        var rank = new GuildRank("Members", 2, ["Alice"]);

        rank.ChangeRankName("Veterans", []);

        rank.Name
            .Should()
            .Be("Veterans");
    }

    [Test]
    public void ChangeRankName_ShouldSendSelfProfile_WhenOnlineMembersExist()
    {
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map, "Alice");
        var clientMock = Mock.Get(aisling.Client);

        var rank = new GuildRank("Members", 2, ["Alice"]);

        rank.ChangeRankName("Veterans", [aisling.Client]);

        clientMock.Verify(c => c.SendSelfProfile(), Times.Once);
    }
    #endregion

    #region IsInferiorTo / IsSuperiorTo
    [Test]
    public void IsInferiorTo_ShouldReturnTrue_WhenHigherTier()
    {
        var leader = new GuildRank("Leader", 0);
        var member = new GuildRank("Member", 2);

        member.IsInferiorTo(leader)
              .Should()
              .BeTrue();

        leader.IsInferiorTo(member)
              .Should()
              .BeFalse();
    }

    [Test]
    public void IsSuperiorTo_ShouldReturnTrue_WhenLowerTier()
    {
        var leader = new GuildRank("Leader", 0);
        var member = new GuildRank("Member", 2);

        leader.IsSuperiorTo(member)
              .Should()
              .BeTrue();

        member.IsSuperiorTo(leader)
              .Should()
              .BeFalse();
    }

    [Test]
    public void IsInferiorTo_WithMinDiff_ShouldRequireMinimumDifference()
    {
        var officer = new GuildRank("Officer", 1);
        var member = new GuildRank("Member", 2);

        // Diff is 1, minDiff is 2
        member.IsInferiorTo(officer, 2)
              .Should()
              .BeFalse();

        // Diff is 1, minDiff is 1
        member.IsInferiorTo(officer)
              .Should()
              .BeTrue();
    }

    [Test]
    public void IsSuperiorTo_WithMinDiff_ShouldRequireMinimumDifference()
    {
        var leader = new GuildRank("Leader", 0);
        var member = new GuildRank("Member", 2);

        // Diff is 2, minDiff is 3
        leader.IsSuperiorTo(member, 3)
              .Should()
              .BeFalse();

        // Diff is 2, minDiff is 2
        leader.IsSuperiorTo(member, 2)
              .Should()
              .BeTrue();
    }

    [Test]
    public void IsInferiorTo_ShouldReturnFalse_WhenSameTier()
    {
        var rank1 = new GuildRank("Members1", 2);
        var rank2 = new GuildRank("Members2", 2);

        rank1.IsInferiorTo(rank2)
             .Should()
             .BeFalse();
    }

    [Test]
    public void IsSuperiorTo_ShouldReturnFalse_WhenSameTier()
    {
        var rank1 = new GuildRank("Members1", 2);
        var rank2 = new GuildRank("Members2", 2);

        rank1.IsSuperiorTo(rank2)
             .Should()
             .BeFalse();
    }
    #endregion
}