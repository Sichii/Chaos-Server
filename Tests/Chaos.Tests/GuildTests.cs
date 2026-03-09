#region
using Chaos.Collections;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class GuildTests
{
    #region GetMemberNames
    [Test]
    public void GetMemberNames_ShouldReturnAllMembers()
    {
        var guild = MockGuild.Create();
        var by = MockAisling.Create(name: "Recruiter");

        guild.AddMember(MockAisling.Create(name: "Alpha"), by);
        guild.AddMember(MockAisling.Create(name: "Beta"), by);
        guild.AddMember(MockAisling.Create(name: "Gamma"), by);

        var names = guild.GetMemberNames();

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

        var ranks = guild.GetRanks();

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
    #endregion
}