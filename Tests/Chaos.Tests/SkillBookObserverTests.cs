#region
using Chaos.Observers;
using Chaos.Testing.Infrastructure.Mocks;
using IObserver_Skill = Chaos.Observers.Abstractions.IObserver<Chaos.Models.Panel.Skill>;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class SkillBookObserverTests
{
    #region OnAdded
    [Test]
    public void OnAdded_ShouldSendSkillToPane()
    {
        var aisling = MockAisling.Create();
        var clientMock = Mock.Get(aisling.Client);
        var skill = MockSkill.Create();

        var observer = new SkillBookObserver(aisling);
        observer.OnAdded(skill);

        clientMock.Verify(c => c.SendAddSkillToPane(skill), Times.Once);
    }
    #endregion

    #region OnRemoved
    [Test]
    public void OnRemoved_ShouldSendRemoveSkillFromPane()
    {
        var aisling = MockAisling.Create();
        var clientMock = Mock.Get(aisling.Client);
        var skill = MockSkill.Create();
        byte slot = 3;

        var observer = new SkillBookObserver(aisling);
        observer.OnRemoved(slot, skill);

        clientMock.Verify(c => c.SendRemoveSkillFromPane(slot), Times.Once);
    }
    #endregion

    #region OnUpdated
    [Test]
    public void OnUpdated_ShouldSendSkillToPane()
    {
        var aisling = MockAisling.Create();
        var clientMock = Mock.Get(aisling.Client);
        var skill = MockSkill.Create();

        var observer = new SkillBookObserver(aisling);
        observer.OnUpdated(2, skill);

        clientMock.Verify(c => c.SendAddSkillToPane(skill), Times.Once);
    }
    #endregion

    #region Equals(IObserver<Skill>?)
    [Test]
    public void Equals_Null_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var observer = new SkillBookObserver(aisling);

        var result = observer.Equals(null);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_SameReference_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer = new SkillBookObserver(aisling);

        var result = observer.Equals(observer);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void Equals_SameAisling_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer1 = new SkillBookObserver(aisling);
        var observer2 = new SkillBookObserver(aisling);

        var result = observer1.Equals(observer2);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void Equals_DifferentAisling_ReturnsFalse()
    {
        var aisling1 = MockAisling.Create();
        var aisling2 = MockAisling.Create();
        var observer1 = new SkillBookObserver(aisling1);
        var observer2 = new SkillBookObserver(aisling2);

        var result = observer1.Equals(observer2);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_DifferentObserverType_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var skillObserver = new SkillBookObserver(aisling);
        var spellObserver = new SpellBookObserver(aisling);

        // Both implement IObserver but for different types, so this won't compile directly
        // Instead, test that the type check fails via object Equals
        // ReSharper disable once SuspiciousTypeConversion.Global
        var result = skillObserver.Equals(spellObserver);

        result.Should()
              .BeFalse();
    }
    #endregion

    #region Equals(object?)
    [Test]
    public void EqualsObject_SameReference_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer = new SkillBookObserver(aisling);

        var result = observer.Equals((object)observer);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void EqualsObject_DifferentType_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var observer = new SkillBookObserver(aisling);

        // ReSharper disable once SuspiciousTypeConversion.Global
        var result = observer.Equals("not an observer");

        result.Should()
              .BeFalse();
    }

    [Test]
    public void EqualsObject_Null_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var observer = new SkillBookObserver(aisling);

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
        var observer = new SkillBookObserver(aisling);

        var hash1 = observer.GetHashCode();
        var hash2 = observer.GetHashCode();

        hash1.Should()
             .Be(hash2);
    }

    [Test]
    public void GetHashCode_SameAisling_ShouldBeEqual()
    {
        var aisling = MockAisling.Create();
        var observer1 = new SkillBookObserver(aisling);
        var observer2 = new SkillBookObserver(aisling);

        observer1.GetHashCode()
                 .Should()
                 .Be(observer2.GetHashCode());
    }

    [Test]
    public void OperatorEquals_SameAisling_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer1 = new SkillBookObserver(aisling);
        var observer2 = new SkillBookObserver(aisling);

        var result = observer1 == observer2;

        result.Should()
              .BeTrue();
    }

    [Test]
    public void OperatorNotEquals_DifferentAisling_ReturnsTrue()
    {
        var aisling1 = MockAisling.Create();
        var aisling2 = MockAisling.Create();
        var observer1 = new SkillBookObserver(aisling1);
        var observer2 = new SkillBookObserver(aisling2);

        var result = observer1 != observer2;

        result.Should()
              .BeTrue();
    }

    [Test]
    public void OperatorEquals_BothNull_ReturnsTrue()
    {
        SkillBookObserver? left = null;
        SkillBookObserver? right = null;

        var result = left == right;

        result.Should()
              .BeTrue();
    }

    [Test]
    public void OperatorEquals_LeftNull_ReturnsFalse()
    {
        SkillBookObserver? left = null;
        var right = new SkillBookObserver(MockAisling.Create());

        var result = left == right;

        result.Should()
              .BeFalse();
    }
    #endregion
}