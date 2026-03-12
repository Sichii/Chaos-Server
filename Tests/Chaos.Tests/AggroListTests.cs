#region
using Chaos.Collections;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class AggroListTests
{
    #region Enumeration
    [Test]
    public void Enumeration_ShouldYieldAllEntries()
    {
        var aggroList = new AggroList();
        var monster1 = MockMonster.Create();
        var monster2 = MockMonster.Create();
        aggroList.AddAggro(monster1, 10);
        aggroList.AddAggro(monster2, 20);

        var entries = aggroList.ToList();

        entries.Should()
               .HaveCount(2);

        entries.Should()
               .Contain(kvp => (kvp.Key == monster1.Id) && (kvp.Value == 10));

        entries.Should()
               .Contain(kvp => (kvp.Key == monster2.Id) && (kvp.Value == 20));
    }
    #endregion

    #region SetAggro
    [Test]
    public void SetAggro_ShouldOverwriteExistingValue()
    {
        var aggroList = new AggroList();
        var monster = MockMonster.Create();
        aggroList.AddAggro(monster, 10);

        aggroList.SetAggro(monster, 99);

        aggroList.GetAggro(monster)
                 .Should()
                 .Be(99);
    }
    #endregion

    #region AddAggro
    [Test]
    public void AddAggro_ShouldSetInitialValue_WhenCreatureNotPresent()
    {
        var aggroList = new AggroList();
        var monster = MockMonster.Create();

        var result = aggroList.AddAggro(monster, 10);

        result.Should()
              .Be(10);
    }

    [Test]
    public void AddAggro_ShouldAccumulate_WhenCreatureAlreadyPresent()
    {
        var aggroList = new AggroList();
        var monster = MockMonster.Create();

        aggroList.AddAggro(monster, 10);
        var result = aggroList.AddAggro(monster, 5);

        result.Should()
              .Be(15);
    }
    #endregion

    #region GetAggro
    [Test]
    public void GetAggro_ShouldReturnZero_WhenCreatureNotPresent()
    {
        var aggroList = new AggroList();
        var monster = MockMonster.Create();

        aggroList.GetAggro(monster)
                 .Should()
                 .Be(0);
    }

    [Test]
    public void GetAggro_ShouldReturnValue_WhenCreaturePresent()
    {
        var aggroList = new AggroList();
        var monster = MockMonster.Create();
        aggroList.AddAggro(monster, 42);

        aggroList.GetAggro(monster)
                 .Should()
                 .Be(42);
    }
    #endregion

    #region Clear
    [Test]
    public void Clear_WithCreature_ShouldRemoveOnlyThatCreature()
    {
        var aggroList = new AggroList();
        var monster1 = MockMonster.Create();
        var monster2 = MockMonster.Create();
        aggroList.AddAggro(monster1, 10);
        aggroList.AddAggro(monster2, 20);

        aggroList.Clear(monster1);

        aggroList.GetAggro(monster1)
                 .Should()
                 .Be(0);

        aggroList.GetAggro(monster2)
                 .Should()
                 .Be(20);
    }

    [Test]
    public void Clear_WithoutCreature_ShouldRemoveAll()
    {
        var aggroList = new AggroList();
        var monster1 = MockMonster.Create();
        var monster2 = MockMonster.Create();
        aggroList.AddAggro(monster1, 10);
        aggroList.AddAggro(monster2, 20);

        aggroList.Clear();

        aggroList.GetAggro(monster1)
                 .Should()
                 .Be(0);

        aggroList.GetAggro(monster2)
                 .Should()
                 .Be(0);
    }
    #endregion
}