#region
using Chaos.Collections;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class ContributionListTests
{
    #region Enumeration
    [Test]
    public void Enumeration_ShouldYieldAllEntries()
    {
        var list = new ContributionList();
        var monster1 = MockMonster.Create();
        var monster2 = MockMonster.Create();
        list.AddContribution(monster1, 10);
        list.AddContribution(monster2, 20);

        var entries = list.ToList();

        entries.Should()
               .HaveCount(2);

        entries.Should()
               .Contain(kvp => (kvp.Key == monster1.Id) && (kvp.Value == 10));

        entries.Should()
               .Contain(kvp => (kvp.Key == monster2.Id) && (kvp.Value == 20));
    }
    #endregion

    #region SetContribution
    [Test]
    public void SetContribution_ShouldOverwriteExistingValue()
    {
        var list = new ContributionList();
        var monster = MockMonster.Create();
        list.AddContribution(monster, 10);

        list.SetContribution(monster, 99);

        list.GetContribution(monster)
            .Should()
            .Be(99);
    }
    #endregion

    #region AddContribution
    [Test]
    public void AddContribution_ShouldSetInitialValue_WhenCreatureNotPresent()
    {
        var list = new ContributionList();
        var monster = MockMonster.Create();

        var result = list.AddContribution(monster, 10);

        result.Should()
              .Be(10);
    }

    [Test]
    public void AddContribution_ShouldAccumulate_WhenCreatureAlreadyPresent()
    {
        var list = new ContributionList();
        var monster = MockMonster.Create();

        list.AddContribution(monster, 10);
        var result = list.AddContribution(monster, 5);

        result.Should()
              .Be(15);
    }
    #endregion

    #region GetContribution
    [Test]
    public void GetContribution_ShouldReturnZero_WhenCreatureNotPresent()
    {
        var list = new ContributionList();
        var monster = MockMonster.Create();

        list.GetContribution(monster)
            .Should()
            .Be(0);
    }

    [Test]
    public void GetContribution_ShouldReturnValue_WhenCreaturePresent()
    {
        var list = new ContributionList();
        var monster = MockMonster.Create();
        list.AddContribution(monster, 42);

        list.GetContribution(monster)
            .Should()
            .Be(42);
    }
    #endregion

    #region Clear
    [Test]
    public void Clear_WithCreature_ShouldRemoveOnlyThatCreature()
    {
        var list = new ContributionList();
        var monster1 = MockMonster.Create();
        var monster2 = MockMonster.Create();
        list.AddContribution(monster1, 10);
        list.AddContribution(monster2, 20);

        list.Clear(monster1);

        list.GetContribution(monster1)
            .Should()
            .Be(0);

        list.GetContribution(monster2)
            .Should()
            .Be(20);
    }

    [Test]
    public void Clear_WithoutCreature_ShouldRemoveAll()
    {
        var list = new ContributionList();
        var monster1 = MockMonster.Create();
        var monster2 = MockMonster.Create();
        list.AddContribution(monster1, 10);
        list.AddContribution(monster2, 20);

        list.Clear();

        list.GetContribution(monster1)
            .Should()
            .Be(0);

        list.GetContribution(monster2)
            .Should()
            .Be(0);
    }
    #endregion
}