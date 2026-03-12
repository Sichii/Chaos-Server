#region
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Networking;
using Chaos.Networking.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class GuildTests
{
    #region ChangeRankName Edge Cases
    [Test]
    public void ChangeRankName_ShouldDoNothing_WhenRankNotFound()
    {
        var guild = MockGuild.Create();

        // Should not throw
        guild.ChangeRankName("NonexistentRank", "NewName");

        // Existing ranks unchanged
        guild.TryGetRank("Leader", out _)
             .Should()
             .BeTrue();
    }
    #endregion

    #region Disband
    [Test]
    public void Disband_ShouldClearAllRanks()
    {
        var guild = MockGuild.Create();
        var aisling = MockAisling.Create(name: "Member1");
        var by = MockAisling.Create(name: "Leader1");

        guild.AddMember(aisling, by);
        guild.Disband();

        guild.GetRanks()
             .Should()
             .BeEmpty();

        guild.HasMember("Member1")
             .Should()
             .BeFalse();
    }
    #endregion

    #region GetMemberNames
    [Test]
    public void GetMemberNames_ShouldReturnAllMembers()
    {
        var guild = MockGuild.Create();
        var by = MockAisling.Create(name: "Recruiter");

        guild.AddMember(MockAisling.Create(name: "Alpha"), by);
        guild.AddMember(MockAisling.Create(name: "Beta"), by);
        guild.AddMember(MockAisling.Create(name: "Gamma"), by);

        var names = guild.GetMemberNames()
                         .ToArray();

        names.Should()
             .HaveCount(3);

        names.Should()
             .Contain("Alpha")
             .And
             .Contain("Beta")
             .And
             .Contain("Gamma");
    }
    #endregion

    #region HasMember
    [Test]
    public void HasMember_ShouldReturnFalse_WhenMemberNotInGuild()
    {
        var guild = MockGuild.Create();

        guild.HasMember("Nobody")
             .Should()
             .BeFalse();
    }
    #endregion

    #region Initialize
    [Test]
    public void Initialize_ShouldReplaceHierarchy()
    {
        var guild = MockGuild.Create();

        var customRanks = new List<GuildRank>
        {
            new("GM", 0, []),
            new("Officer", 1, []),
            new("Recruit", 2, ["TestPlayer"])
        };

        guild.Initialize(customRanks);

        var ranks = guild.GetRanks()
                         .ToArray();

        ranks.Should()
             .HaveCount(3);

        ranks.Select(r => r.Name)
             .Should()
             .ContainInOrder("GM", "Officer", "Recruit");

        guild.HasMember("TestPlayer")
             .Should()
             .BeTrue();
    }
    #endregion

    #region TryKickMember — Online Member
    [Test]
    public void TryKickMember_ShouldClearGuildFromOnlineMember()
    {
        var registry = new ClientRegistry<IChaosWorldClient>();
        var member = MockAisling.Create(name: "KickOnline");
        var leader = MockAisling.Create(name: "KickLeader");

        Mock.Get(member.Client)
            .SetupGet(c => c.Id)
            .Returns(member.Id);

        Mock.Get(leader.Client)
            .SetupGet(c => c.Id)
            .Returns(leader.Id);

        registry.TryAdd(member.Client);
        registry.TryAdd(leader.Client);

        var guild = MockGuild.Create(clientRegistry: registry);
        guild.AddMember(member, leader);

        member.Guild
              .Should()
              .BeSameAs(guild);

        var result = guild.TryKickMember("KickOnline", leader);

        result.Should()
              .BeTrue();

        member.Guild
              .Should()
              .BeNull();

        member.GuildRank
              .Should()
              .BeNull();

        Mock.Get(member.Client)
            .Verify(c => c.SendSelfProfile(), Times.AtLeast(2));
    }
    #endregion

    #region TryKickMember Edge Cases
    [Test]
    public void TryKickMember_ShouldReturnFalse_WhenMemberNotInGuild()
    {
        var guild = MockGuild.Create();
        var by = MockAisling.Create(name: "Leader1");

        guild.TryKickMember("Nobody", by)
             .Should()
             .BeFalse();
    }
    #endregion

    #region Construction
    [Test]
    public void Guild_ShouldHaveCorrectName()
    {
        var guild = MockGuild.Create("Heroes");

        guild.Name
             .Should()
             .Be("Heroes");
    }

    [Test]
    public void Guild_ShouldHaveFourDefaultRanks()
    {
        var guild = MockGuild.Create();

        var ranks = guild.GetRanks();

        ranks.Should()
             .HaveCount(4);
    }

    [Test]
    public void Guild_ShouldHaveDefaultRankNames()
    {
        var guild = MockGuild.Create();
        var ranks = guild.GetRanks();

        var rankNames = ranks.Select(r => r.Name)
                             .ToList();

        rankNames.Should()
                 .Contain("Leader")
                 .And
                 .Contain("Council")
                 .And
                 .Contain("Member")
                 .And
                 .Contain("Applicant");
    }
    #endregion

    #region AddMember
    [Test]
    public void AddMember_ShouldAddToLowestRank()
    {
        var guild = MockGuild.Create();
        var aisling = MockAisling.Create(name: "NewGuy");
        var by = MockAisling.Create(name: "GuildLeader");

        guild.AddMember(aisling, by);

        guild.HasMember("NewGuy")
             .Should()
             .BeTrue();

        var rank = guild.RankOf("NewGuy");

        rank.Name
            .Should()
            .Be("Applicant");
    }

    [Test]
    public void AddMember_ShouldAssociateGuildAndRank()
    {
        var guild = MockGuild.Create();
        var aisling = MockAisling.Create(name: "NewGuy");
        var by = MockAisling.Create(name: "GuildLeader");

        guild.AddMember(aisling, by);

        aisling.Guild
               .Should()
               .BeSameAs(guild);

        aisling.GuildRank
               .Should()
               .Be("Applicant");
    }
    #endregion

    #region ChangeRank — Online Member Updates
    [Test]
    public void ChangeRank_ShouldUpdateAislingGuildRank_WhenMemberIsOnline()
    {
        var registry = new ClientRegistry<IChaosWorldClient>();
        var member = MockAisling.Create(name: "OnlineMember");
        var leader = MockAisling.Create(name: "OnlineLeader");

        // Set up unique IDs on mock clients so they can be added to registry
        Mock.Get(member.Client)
            .SetupGet(c => c.Id)
            .Returns(member.Id);

        Mock.Get(leader.Client)
            .SetupGet(c => c.Id)
            .Returns(leader.Id);

        registry.TryAdd(member.Client);
        registry.TryAdd(leader.Client);

        var guild = MockGuild.Create(clientRegistry: registry);
        guild.AddMember(member, leader);

        Mock.Get(member.Client)
            .Invocations
            .Clear();

        // Promote to Council (tier 1)
        guild.ChangeRank("OnlineMember", 1, leader);

        member.GuildRank
              .Should()
              .Be("Council");

        Mock.Get(member.Client)
            .Verify(c => c.SendSelfProfile(), Times.AtLeastOnce);
    }

    [Test]
    public void ChangeRank_ShouldSendPromotionMessage_WhenOnlineMembersExist()
    {
        var registry = new ClientRegistry<IChaosWorldClient>();
        var member = MockAisling.Create(name: "PromoteMember");
        var leader = MockAisling.Create(name: "PromoteLeader");
        var observer = MockAisling.Create(name: "Observer");

        Mock.Get(member.Client)
            .SetupGet(c => c.Id)
            .Returns(member.Id);

        Mock.Get(leader.Client)
            .SetupGet(c => c.Id)
            .Returns(leader.Id);

        Mock.Get(observer.Client)
            .SetupGet(c => c.Id)
            .Returns(observer.Id);

        registry.TryAdd(member.Client);
        registry.TryAdd(leader.Client);
        registry.TryAdd(observer.Client);

        var guild = MockGuild.Create(clientRegistry: registry);
        guild.AddMember(member, leader);
        guild.AddMember(observer, leader);

        Mock.Get(observer.Client)
            .Invocations
            .Clear();

        // Promote member to Council (tier 1 < current tier 3 → promotion)
        guild.ChangeRank("PromoteMember", 1, leader);

        // Observer should receive promotion notification
        Mock.Get(observer.Client)
            .Verify(c => c.SendServerMessage(It.IsAny<ServerMessageType>(), It.Is<string>(s => s.Contains("promoted"))), Times.AtLeastOnce);
    }
    #endregion

    #region ChangeRank
    [Test]
    public void ChangeRank_ShouldMoveMemberToNewTier()
    {
        var guild = MockGuild.Create();
        var aisling = MockAisling.Create(name: "Member1");
        var by = MockAisling.Create(name: "Leader1");

        guild.AddMember(aisling, by);
        guild.ChangeRank("Member1", 2, by);

        var rank = guild.RankOf("Member1");

        rank.Name
            .Should()
            .Be("Member");

        rank.Tier
            .Should()
            .Be(2);
    }

    [Test]
    public void ChangeRank_ShouldPromoteToCouncil()
    {
        var guild = MockGuild.Create();
        var aisling = MockAisling.Create(name: "Member1");
        var by = MockAisling.Create(name: "Leader1");

        guild.AddMember(aisling, by);
        guild.ChangeRank("Member1", 1, by);

        guild.RankOf("Member1")
             .Name
             .Should()
             .Be("Council");
    }
    #endregion

    #region ChangeRankName
    [Test]
    public void ChangeRankName_ShouldRenameRank()
    {
        var guild = MockGuild.Create();

        guild.ChangeRankName("Applicant", "Initiate");

        guild.TryGetRank("Initiate", out var rank)
             .Should()
             .BeTrue();

        rank!.Tier
             .Should()
             .Be(3);
    }

    [Test]
    public void ChangeRankName_ShouldMakeOldNameNotFound()
    {
        var guild = MockGuild.Create();

        guild.ChangeRankName("Applicant", "Initiate");

        guild.TryGetRank("Applicant", out _)
             .Should()
             .BeFalse();
    }
    #endregion

    #region TryKickMember
    [Test]
    public void TryKickMember_ShouldRemoveMember()
    {
        var guild = MockGuild.Create();
        var aisling = MockAisling.Create(name: "KickMe");
        var by = MockAisling.Create(name: "Leader1");

        guild.AddMember(aisling, by);
        var result = guild.TryKickMember("KickMe", by);

        result.Should()
              .BeTrue();

        guild.HasMember("KickMe")
             .Should()
             .BeFalse();
    }

    [Test]
    public void TryKickMember_ShouldReturnFalse_WhenKickingLeaderTier()
    {
        var guild = MockGuild.Create();
        var leader = MockAisling.Create(name: "TheLeader");
        var by = MockAisling.Create(name: "SomeAdmin");

        guild.AddMember(leader, by);

        // Promote to leader tier (0)
        guild.ChangeRank("TheLeader", 0, by);

        var result = guild.TryKickMember("TheLeader", by);

        result.Should()
              .BeFalse();

        guild.HasMember("TheLeader")
             .Should()
             .BeTrue();
    }
    #endregion

    #region TryLeave
    [Test]
    public void TryLeave_ShouldRemoveMember()
    {
        var guild = MockGuild.Create();
        var aisling = MockAisling.Create(name: "Leaver");
        var by = MockAisling.Create(name: "Leader1");

        guild.AddMember(aisling, by);
        var result = guild.TryLeave(aisling);

        result.Should()
              .BeTrue();

        guild.HasMember("Leaver")
             .Should()
             .BeFalse();
    }

    [Test]
    public void TryLeave_ShouldClearAislingGuildReference()
    {
        var guild = MockGuild.Create();
        var aisling = MockAisling.Create(name: "Leaver");
        var by = MockAisling.Create(name: "Leader1");

        guild.AddMember(aisling, by);
        guild.TryLeave(aisling);

        aisling.Guild
               .Should()
               .BeNull();

        aisling.GuildRank
               .Should()
               .BeNull();
    }

    [Test]
    public void TryLeave_LeaderShouldFailIfOnlyLeader()
    {
        var guild = MockGuild.Create();
        var leader = MockAisling.Create(name: "SoloLeader");
        var by = MockAisling.Create(name: "Recruiter");

        guild.AddMember(leader, by);
        guild.ChangeRank("SoloLeader", 0, by);

        var result = guild.TryLeave(leader);

        // Can't leave as the only leader (rank count of tier 0 is 1)
        result.Should()
              .BeFalse();
    }
    #endregion

    #region Associate
    [Test]
    public void Associate_ShouldAssociateAisling_WhenMemberExists()
    {
        var guild = MockGuild.Create();
        var aisling = MockAisling.Create(name: "Member1");
        var by = MockAisling.Create(name: "Leader1");

        guild.AddMember(aisling, by);

        // Clear the association manually
        aisling.Guild = null;
        aisling.GuildRank = null;

        // Re-associate
        guild.Associate(aisling);

        aisling.Guild
               .Should()
               .BeSameAs(guild);

        aisling.GuildRank
               .Should()
               .Be("Applicant");
    }

    [Test]
    public void Associate_ShouldClearAssociation_WhenMemberNotFound()
    {
        var guild = MockGuild.Create();
        var aisling = MockAisling.Create(name: "Stranger");

        // Set some dummy values to verify they get cleared
        aisling.Guild = guild;
        aisling.GuildRank = "Member";

        guild.Associate(aisling);

        aisling.Guild
               .Should()
               .BeNull();

        aisling.GuildRank
               .Should()
               .BeNull();
    }
    #endregion

    #region AddMember Edge Cases
    [Test]
    public void AddMember_ShouldReturnEarly_WhenAlreadyInSameGuild()
    {
        var guild = MockGuild.Create();
        var aisling = MockAisling.Create(name: "Member1");
        var by = MockAisling.Create(name: "Leader1");

        guild.AddMember(aisling, by);

        // Adding again to same guild should just return without error
        guild.AddMember(aisling, by);

        // Should still be in guild, just once
        guild.GetMemberNames()
             .Count(n => n == "Member1")
             .Should()
             .Be(1);
    }

    [Test]
    public void AddMember_ShouldThrow_WhenInDifferentGuild()
    {
        var guild1 = MockGuild.Create("Guild1");
        var guild2 = MockGuild.Create("Guild2");
        var aisling = MockAisling.Create(name: "Member1");
        var by = MockAisling.Create(name: "Leader1");

        guild1.AddMember(aisling, by);

        var act = () => guild2.AddMember(aisling, by);

        act.Should()
           .Throw<InvalidOperationException>();
    }
    #endregion

    #region ChangeRank Edge Cases
    [Test]
    public void ChangeRank_ShouldThrow_WhenRankDoesNotExist()
    {
        var guild = MockGuild.Create();
        var aisling = MockAisling.Create(name: "Member1");
        var by = MockAisling.Create(name: "Leader1");

        guild.AddMember(aisling, by);

        var act = () => guild.ChangeRank("Member1", 99, by);

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
    public void ChangeRank_ShouldThrow_WhenMemberNotInGuild()
    {
        var guild = MockGuild.Create();
        var by = MockAisling.Create(name: "Leader1");

        var act = () => guild.ChangeRank("Nobody", 2, by);

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
    public void ChangeRank_ShouldNotChange_WhenMemberIsLeaderTier()
    {
        var guild = MockGuild.Create();
        var aisling = MockAisling.Create(name: "TheLeader");
        var by = MockAisling.Create(name: "Admin");

        guild.AddMember(aisling, by);
        guild.ChangeRank("TheLeader", 0, by);

        // Trying to change a leader should be ignored
        guild.ChangeRank("TheLeader", 2, by);

        guild.RankOf("TheLeader")
             .Tier
             .Should()
             .Be(0);
    }
    #endregion

    #region RankOf
    [Test]
    public void RankOf_ShouldReturnRank_WhenMemberExists()
    {
        var guild = MockGuild.Create();
        var aisling = MockAisling.Create(name: "Member1");
        var by = MockAisling.Create(name: "Leader1");

        guild.AddMember(aisling, by);

        var rank = guild.RankOf("Member1");

        rank.Name
            .Should()
            .Be("Applicant");

        rank.Tier
            .Should()
            .Be(3);
    }

    [Test]
    public void RankOf_ShouldThrow_WhenMemberNotInGuild()
    {
        var guild = MockGuild.Create();

        var act = () => guild.RankOf("Nobody");

        act.Should()
           .Throw<InvalidOperationException>();
    }
    #endregion

    #region TryGetRank
    [Test]
    public void TryGetRank_ByName_ShouldReturnTrue_WhenFound()
    {
        var guild = MockGuild.Create();

        guild.TryGetRank("Leader", out var rank)
             .Should()
             .BeTrue();

        rank!.Name
             .Should()
             .Be("Leader");

        rank.Tier
            .Should()
            .Be(0);
    }

    [Test]
    public void TryGetRank_ByName_ShouldReturnFalse_WhenNotFound()
    {
        var guild = MockGuild.Create();

        guild.TryGetRank("NonexistentRank", out var rank)
             .Should()
             .BeFalse();

        rank.Should()
            .BeNull();
    }

    [Test]
    public void TryGetRank_ByTier_ShouldReturnTrue_WhenFound()
    {
        var guild = MockGuild.Create();

        guild.TryGetRank(0, out var rank)
             .Should()
             .BeTrue();

        rank!.Name
             .Should()
             .Be("Leader");
    }

    [Test]
    public void TryGetRank_ByTier_ShouldReturnFalse_WhenNotFound()
    {
        var guild = MockGuild.Create();

        guild.TryGetRank(99, out var rank)
             .Should()
             .BeFalse();

        rank.Should()
            .BeNull();
    }
    #endregion

    #region TryLeave Edge Cases
    [Test]
    public void TryLeave_ShouldReturnFalse_WhenNotInGuild()
    {
        var guild = MockGuild.Create();
        var aisling = MockAisling.Create(name: "Stranger");

        guild.TryLeave(aisling)
             .Should()
             .BeFalse();
    }

    [Test]
    public void TryLeave_LeaderShouldSucceed_WhenMultipleLeaders()
    {
        var guild = MockGuild.Create();
        var leader1 = MockAisling.Create(name: "Leader1");
        var leader2 = MockAisling.Create(name: "Leader2");
        var by = MockAisling.Create(name: "Recruiter");

        guild.AddMember(leader1, by);
        guild.AddMember(leader2, by);
        guild.ChangeRank("Leader1", 0, by);
        guild.ChangeRank("Leader2", 0, by);

        // With 2 leaders, one can leave
        guild.TryLeave(leader1)
             .Should()
             .BeTrue();

        guild.HasMember("Leader1")
             .Should()
             .BeFalse();
    }
    #endregion

    #region ChangeRank — Demotion
    [Test]
    public void ChangeRank_ShouldSendDemotionMessage_WhenNewTierIsHigherThanCurrent()
    {
        var registry = new ClientRegistry<IChaosWorldClient>();
        var member = MockAisling.Create(name: "DemoteMember");
        var leader = MockAisling.Create(name: "DemoteLeader");
        var observer = MockAisling.Create(name: "DemoteObserver");

        Mock.Get(member.Client)
            .SetupGet(c => c.Id)
            .Returns(member.Id);

        Mock.Get(leader.Client)
            .SetupGet(c => c.Id)
            .Returns(leader.Id);

        Mock.Get(observer.Client)
            .SetupGet(c => c.Id)
            .Returns(observer.Id);

        registry.TryAdd(member.Client);
        registry.TryAdd(leader.Client);
        registry.TryAdd(observer.Client);

        var guild = MockGuild.Create(clientRegistry: registry);
        guild.AddMember(member, leader);
        guild.AddMember(observer, leader);

        // First promote member to Council (tier 1)
        guild.ChangeRank("DemoteMember", 1, leader);

        Mock.Get(observer.Client)
            .Invocations
            .Clear();

        // Now demote to Member (tier 2, higher than 1 → demotion)
        guild.ChangeRank("DemoteMember", 2, leader);

        member.GuildRank
              .Should()
              .Be("Member");

        // Observer should receive demotion notification
        Mock.Get(observer.Client)
            .Verify(c => c.SendServerMessage(It.IsAny<ServerMessageType>(), It.Is<string>(s => s.Contains("demoted"))), Times.AtLeastOnce);
    }

    [Test]
    public void ChangeRank_ShouldDemoteOfflineMember_WithoutUpdatingClient()
    {
        // Use an empty registry so the member is "offline"
        var registry = new ClientRegistry<IChaosWorldClient>();
        var guild = MockGuild.Create(clientRegistry: registry);

        // Manually initialize hierarchy with a member at tier 1
        guild.Initialize(
            [
                new GuildRank("Leader", 0, []),
                new GuildRank("Council", 1, ["OfflineMember"]),
                new GuildRank("Member", 2, []),
                new GuildRank("Applicant", 3, [])
            ]);

        var by = MockAisling.Create(name: "Admin");

        // Demote OfflineMember from Council (tier 1) to Member (tier 2)
        guild.ChangeRank("OfflineMember", 2, by);

        // Member should now be in the Member rank
        var rank = guild.RankOf("OfflineMember");

        rank.Name
            .Should()
            .Be("Member");

        rank.Tier
            .Should()
            .Be(2);
    }

    [Test]
    public void ChangeRank_ShouldPromoteOfflineMember_WithoutUpdatingClient()
    {
        // Use an empty registry so the member is "offline"
        var registry = new ClientRegistry<IChaosWorldClient>();
        var guild = MockGuild.Create(clientRegistry: registry);

        // Manually initialize hierarchy with a member at tier 3
        guild.Initialize(
            [
                new GuildRank("Leader", 0, []),
                new GuildRank("Council", 1, []),
                new GuildRank("Member", 2, []),
                new GuildRank("Applicant", 3, ["OfflineApplicant"])
            ]);

        var by = MockAisling.Create(name: "Admin");

        // Promote from Applicant (tier 3) to Council (tier 1)
        guild.ChangeRank("OfflineApplicant", 1, by);

        var rank = guild.RankOf("OfflineApplicant");

        rank.Name
            .Should()
            .Be("Council");

        rank.Tier
            .Should()
            .Be(1);
    }
    #endregion

    #region Equality
    [Test]
    public void Equals_SameGuild_ShouldBeTrue()
    {
        var guild = MockGuild.Create();

        guild.Equals(guild)
             .Should()
             .BeTrue();
    }

    [Test]
    public void Equals_DifferentGuild_ShouldBeFalse()
    {
        var guild1 = MockGuild.Create("Guild1");
        var guild2 = MockGuild.Create("Guild2");

        guild1.Equals(guild2)
              .Should()
              .BeFalse();
    }

    [Test]
    public void Equals_Null_ShouldBeFalse()
    {
        var guild = MockGuild.Create();

        guild.Equals(null)
             .Should()
             .BeFalse();
    }

    [Test]
    public void Equals_Object_NotGuild_ShouldBeFalse()
    {
        var guild = MockGuild.Create();

        // ReSharper disable once SuspiciousTypeConversion.Global
        guild.Equals("not a guild")
             .Should()
             .BeFalse();
    }

    [Test]
    public void OperatorEquals_ShouldWork()
    {
        var guild1 = MockGuild.Create("Same");
        var guild2 = MockGuild.Create("Different");

        // ReSharper disable once EqualExpressionComparison
        (guild1 == guild1).Should()
                          .BeTrue();

        (guild1 != guild2).Should()
                          .BeTrue();

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        (guild1 == null).Should()
                        .BeFalse();

        (null == guild1).Should()
                        .BeFalse();
    }

    [Test]
    public void GetHashCode_ShouldBeBasedOnGuid()
    {
        var guild = MockGuild.Create();

        guild.GetHashCode()
             .Should()
             .Be(guild.Guid.GetHashCode());
    }

    [Test]
    public void Equals_Object_SameGuild_ShouldBeTrue()
    {
        var guild = MockGuild.Create();
        object boxed = guild;

        guild.Equals(boxed)
             .Should()
             .BeTrue();
    }

    [Test]
    public void Equals_Object_DifferentGuild_ShouldBeFalse()
    {
        var guild1 = MockGuild.Create("GuildA");
        var guild2 = MockGuild.Create("GuildB");
        object boxed = guild2;

        guild1.Equals(boxed)
              .Should()
              .BeFalse();
    }

    [Test]
    public void OperatorNotEquals_BothNull_ShouldBeFalse()
    {
        Guild? left = null;
        Guild? right = null;

        (left != right).Should()
                       .BeFalse();
    }

    [Test]
    public void OperatorEquals_BothNull_ShouldBeTrue()
    {
        Guild? left = null;
        Guild? right = null;

        (left == right).Should()
                       .BeTrue();
    }

    [Test]
    public void OperatorNotEquals_LeftNullRightNotNull_ShouldBeTrue()
    {
        Guild? left = null;
        var right = MockGuild.Create();

        (left != right).Should()
                       .BeTrue();
    }

    [Test]
    public void GetHashCode_SameGuid_ShouldProduceSameHash()
    {
        var guild = MockGuild.Create();

        var hash1 = guild.GetHashCode();
        var hash2 = guild.GetHashCode();

        hash1.Should()
             .Be(hash2);
    }
    #endregion
}