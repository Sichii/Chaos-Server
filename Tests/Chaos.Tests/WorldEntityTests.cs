#region
using Chaos.Models.World.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class WorldEntityTests
{
    #region Equals — same ID different Creation
    [Test]
    public void Equals_ShouldReturnFalse_WhenSameIdButDifferentCreation()
    {
        var monster1 = MockMonster.Create();
        var monster2 = MockMonster.Create();

        // Force same ID via reflection
        typeof(WorldEntity).GetProperty(nameof(WorldEntity.Id))!.SetValue(monster2, monster1.Id);

        // IDs now match, but Creation timestamps differ
        monster1.Equals(monster2)
                .Should()
                .BeFalse();
    }
    #endregion

    #region Equals(WorldEntity?)
    [Test]
    public void Equals_Null_ReturnsFalse()
    {
        var monster = MockMonster.Create();

        monster.Equals(null)
               .Should()
               .BeFalse();
    }

    [Test]
    public void Equals_SameReference_ReturnsTrue()
    {
        var monster = MockMonster.Create();

        monster.Equals(monster)
               .Should()
               .BeTrue();
    }

    [Test]
    public void Equals_DifferentId_ReturnsFalse()
    {
        var monster1 = MockMonster.Create();
        var monster2 = MockMonster.Create();

        monster1.Equals(monster2)
                .Should()
                .BeFalse();
    }
    #endregion

    #region Equals(object?)
    [Test]
    public void EqualsObject_Null_ReturnsFalse()
    {
        var monster = MockMonster.Create();

        monster.Equals((object?)null)
               .Should()
               .BeFalse();
    }

    [Test]
    public void EqualsObject_SameReference_ReturnsTrue()
    {
        var monster = MockMonster.Create();

        monster.Equals((object)monster)
               .Should()
               .BeTrue();
    }

    [Test]
    public void EqualsObject_DifferentWorldEntity_ReturnsFalse()
    {
        var monster1 = MockMonster.Create();
        var monster2 = MockMonster.Create();

        monster1.Equals((object)monster2)
                .Should()
                .BeFalse();
    }

    [Test]
    public void EqualsObject_NonWorldEntity_ReturnsFalse()
    {
        var monster = MockMonster.Create();

        // ReSharper disable once SuspiciousTypeConversion.Global
        monster.Equals("not a world entity")
               .Should()
               .BeFalse();
    }
    #endregion

    #region GetHashCode
    [Test]
    public void GetHashCode_ShouldReturnIdAsInt()
    {
        var monster = MockMonster.Create();

        monster.GetHashCode()
               .Should()
               .Be((int)monster.Id);
    }

    [Test]
    public void GetHashCode_ShouldBeConsistentAcrossCalls()
    {
        var monster = MockMonster.Create();

        var hash1 = monster.GetHashCode();
        var hash2 = monster.GetHashCode();

        hash1.Should()
             .Be(hash2);
    }
    #endregion

    #region IdEqualityComparer
    [Test]
    public void IdComparer_SameReference_ReturnsTrue()
    {
        var monster = MockMonster.Create();

        WorldEntity.IdComparer
                   .Equals(monster, monster)
                   .Should()
                   .BeTrue();
    }

    [Test]
    public void IdComparer_XNull_ReturnsFalse()
    {
        var monster = MockMonster.Create();

        WorldEntity.IdComparer
                   .Equals(null, monster)
                   .Should()
                   .BeFalse();
    }

    [Test]
    public void IdComparer_YNull_ReturnsFalse()
    {
        var monster = MockMonster.Create();

        WorldEntity.IdComparer
                   .Equals(monster, null)
                   .Should()
                   .BeFalse();
    }

    [Test]
    public void IdComparer_BothNull_ReturnsTrue()
        => WorldEntity.IdComparer
                      .Equals(null, null)
                      .Should()
                      .BeTrue();

    [Test]
    public void IdComparer_DifferentIds_ReturnsFalse()
    {
        var monster1 = MockMonster.Create();
        var monster2 = MockMonster.Create();

        WorldEntity.IdComparer
                   .Equals(monster1, monster2)
                   .Should()
                   .BeFalse();
    }

    [Test]
    public void IdComparer_GetHashCode_ShouldReturnIdAsInt()
    {
        var monster = MockMonster.Create();

        WorldEntity.IdComparer
                   .GetHashCode(monster)
                   .Should()
                   .Be((int)monster.Id);
    }
    #endregion
}