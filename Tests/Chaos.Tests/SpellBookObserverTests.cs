#region
using Chaos.Observers;
using Chaos.Testing.Infrastructure.Mocks;
using IObserver_Spell = Chaos.Observers.Abstractions.IObserver<Chaos.Models.Panel.Spell>;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class SpellBookObserverTests
{
    #region OnAdded
    [Test]
    public void OnAdded_ShouldSendSpellToPane()
    {
        var aisling = MockAisling.Create();
        var clientMock = Mock.Get(aisling.Client);
        var spell = MockSpell.Create();

        var observer = new SpellBookObserver(aisling);
        observer.OnAdded(spell);

        clientMock.Verify(c => c.SendAddSpellToPane(spell), Times.Once);
    }
    #endregion

    #region OnRemoved
    [Test]
    public void OnRemoved_ShouldSendRemoveSpellFromPane()
    {
        var aisling = MockAisling.Create();
        var clientMock = Mock.Get(aisling.Client);
        var spell = MockSpell.Create();
        byte slot = 7;

        var observer = new SpellBookObserver(aisling);
        observer.OnRemoved(slot, spell);

        clientMock.Verify(c => c.SendRemoveSpellFromPane(slot), Times.Once);
    }
    #endregion

    #region OnUpdated
    [Test]
    public void OnUpdated_ShouldSendSpellToPane()
    {
        var aisling = MockAisling.Create();
        var clientMock = Mock.Get(aisling.Client);
        var spell = MockSpell.Create();

        var observer = new SpellBookObserver(aisling);
        observer.OnUpdated(4, spell);

        clientMock.Verify(c => c.SendAddSpellToPane(spell), Times.Once);
    }
    #endregion

    #region Equals(IObserver<Spell>?)
    [Test]
    public void Equals_Null_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var observer = new SpellBookObserver(aisling);

        var result = observer.Equals(null);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_SameReference_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer = new SpellBookObserver(aisling);

        var result = observer.Equals(observer);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void Equals_SameAisling_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer1 = new SpellBookObserver(aisling);
        var observer2 = new SpellBookObserver(aisling);

        var result = observer1.Equals(observer2);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void Equals_DifferentAisling_ReturnsFalse()
    {
        var aisling1 = MockAisling.Create();
        var aisling2 = MockAisling.Create();
        var observer1 = new SpellBookObserver(aisling1);
        var observer2 = new SpellBookObserver(aisling2);

        var result = observer1.Equals(observer2);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_DifferentObserverType_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var spellObserver = new SpellBookObserver(aisling);
        var skillObserver = new SkillBookObserver(aisling);

        // Different generic type parameters, so test via object Equals
        // ReSharper disable once SuspiciousTypeConversion.Global
        var result = spellObserver.Equals(skillObserver);

        result.Should()
              .BeFalse();
    }
    #endregion

    #region Equals(object?)
    [Test]
    public void EqualsObject_SameReference_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer = new SpellBookObserver(aisling);

        var result = observer.Equals((object)observer);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void EqualsObject_DifferentType_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var observer = new SpellBookObserver(aisling);

        // ReSharper disable once SuspiciousTypeConversion.Global
        var result = observer.Equals("not an observer");

        result.Should()
              .BeFalse();
    }

    [Test]
    public void EqualsObject_Null_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var observer = new SpellBookObserver(aisling);

        var result = observer.Equals((object?)null);

        result.Should()
              .BeFalse();
    }
    #endregion

    #region GetHashCode / Operators
    [Test]
    public void GetHashCode_ShouldBeConsistent()
    {
        var aisling = MockAisling.Create();
        var observer = new SpellBookObserver(aisling);

        var hash1 = observer.GetHashCode();
        var hash2 = observer.GetHashCode();

        hash1.Should()
             .Be(hash2);
    }

    [Test]
    public void GetHashCode_SameAisling_ShouldBeEqual()
    {
        var aisling = MockAisling.Create();
        var observer1 = new SpellBookObserver(aisling);
        var observer2 = new SpellBookObserver(aisling);

        observer1.GetHashCode()
                 .Should()
                 .Be(observer2.GetHashCode());
    }

    [Test]
    public void OperatorEquals_SameAisling_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer1 = new SpellBookObserver(aisling);
        var observer2 = new SpellBookObserver(aisling);

        var result = observer1 == observer2;

        result.Should()
              .BeTrue();
    }

    [Test]
    public void OperatorNotEquals_DifferentAisling_ReturnsTrue()
    {
        var aisling1 = MockAisling.Create();
        var aisling2 = MockAisling.Create();
        var observer1 = new SpellBookObserver(aisling1);
        var observer2 = new SpellBookObserver(aisling2);

        var result = observer1 != observer2;

        result.Should()
              .BeTrue();
    }

    [Test]
    public void OperatorEquals_BothNull_ReturnsTrue()
    {
        SpellBookObserver? left = null;
        SpellBookObserver? right = null;

        var result = left == right;

        result.Should()
              .BeTrue();
    }

    [Test]
    public void OperatorEquals_LeftNull_ReturnsFalse()
    {
        SpellBookObserver? left = null;
        var right = new SpellBookObserver(MockAisling.Create());

        var result = left == right;

        result.Should()
              .BeFalse();
    }
    #endregion
}