#region
using Chaos.Observers;
using Chaos.Testing.Infrastructure.Mocks;
using IObserver_Item = Chaos.Observers.Abstractions.IObserver<Chaos.Models.Panel.Item>;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class ExchangeObserverTests
{
    #region OnAdded
    [Test]
    public void OnAdded_ShouldNotifyBothClients()
    {
        var owner = MockAisling.Create();
        var other = MockAisling.Create();
        var ownerClientMock = Mock.Get(owner.Client);
        var otherClientMock = Mock.Get(other.Client);
        var item = MockItem.Create(setup: i => i.Slot = 2);

        var observer = new ExchangeObserver(owner, other);
        observer.OnAdded(item);

        ownerClientMock.Verify(c => c.SendExchangeAddItem(false, item.Slot, item), Times.Once);
        otherClientMock.Verify(c => c.SendExchangeAddItem(true, item.Slot, item), Times.Once);
    }
    #endregion

    #region OnRemoved
    [Test]
    public void OnRemoved_ShouldNotThrow()
    {
        var owner = MockAisling.Create();
        var other = MockAisling.Create();
        var item = MockItem.Create();

        var observer = new ExchangeObserver(owner, other);

        var act = () => observer.OnRemoved(1, item);

        act.Should()
           .NotThrow();
    }
    #endregion

    #region OnUpdated
    [Test]
    public void OnUpdated_ShouldNotifyBothClients()
    {
        var owner = MockAisling.Create();
        var other = MockAisling.Create();
        var ownerClientMock = Mock.Get(owner.Client);
        var otherClientMock = Mock.Get(other.Client);
        var item = MockItem.Create(setup: i => i.Slot = 4);

        var observer = new ExchangeObserver(owner, other);
        observer.OnUpdated(4, item);

        ownerClientMock.Verify(c => c.SendExchangeAddItem(false, item.Slot, item), Times.Once);
        otherClientMock.Verify(c => c.SendExchangeAddItem(true, item.Slot, item), Times.Once);
    }
    #endregion

    #region Equals(IObserver<Item>?)
    [Test]
    public void Equals_Null_ReturnsFalse()
    {
        var owner = MockAisling.Create();
        var other = MockAisling.Create();
        var observer = new ExchangeObserver(owner, other);

        var result = observer.Equals(null);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_SameReference_ReturnsTrue()
    {
        var owner = MockAisling.Create();
        var other = MockAisling.Create();
        var observer = new ExchangeObserver(owner, other);

        var result = observer.Equals(observer);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void Equals_SameOwnerAndOther_ReturnsTrue()
    {
        var owner = MockAisling.Create();
        var other = MockAisling.Create();
        var observer1 = new ExchangeObserver(owner, other);
        var observer2 = new ExchangeObserver(owner, other);

        var result = observer1.Equals(observer2);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void Equals_DifferentOwner_ReturnsFalse()
    {
        var owner1 = MockAisling.Create();
        var owner2 = MockAisling.Create();
        var other = MockAisling.Create();
        var observer1 = new ExchangeObserver(owner1, other);
        var observer2 = new ExchangeObserver(owner2, other);

        var result = observer1.Equals(observer2);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_DifferentOther_ReturnsFalse()
    {
        var owner = MockAisling.Create();
        var other1 = MockAisling.Create();
        var other2 = MockAisling.Create();
        var observer1 = new ExchangeObserver(owner, other1);
        var observer2 = new ExchangeObserver(owner, other2);

        var result = observer1.Equals(observer2);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_DifferentObserverType_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var exchangeObserver = new ExchangeObserver(aisling, MockAisling.Create());
        var equipObserver = new EquipmentObserver(aisling);

        var result = exchangeObserver.Equals(equipObserver);

        result.Should()
              .BeFalse();
    }
    #endregion

    #region Equals(object?)
    [Test]
    public void EqualsObject_SameReference_ReturnsTrue()
    {
        var owner = MockAisling.Create();
        var other = MockAisling.Create();
        var observer = new ExchangeObserver(owner, other);

        var result = observer.Equals((object)observer);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void EqualsObject_DifferentType_ReturnsFalse()
    {
        var owner = MockAisling.Create();
        var other = MockAisling.Create();
        var observer = new ExchangeObserver(owner, other);

        // ReSharper disable once SuspiciousTypeConversion.Global
        var result = observer.Equals("not an observer");

        result.Should()
              .BeFalse();
    }

    [Test]
    public void EqualsObject_Null_ReturnsFalse()
    {
        var owner = MockAisling.Create();
        var other = MockAisling.Create();
        var observer = new ExchangeObserver(owner, other);

        var result = observer.Equals((object?)null);

        result.Should()
              .BeFalse();
    }
    #endregion

    #region GetHashCode / Operators
    [Test]
    public void GetHashCode_ShouldBeConsistent()
    {
        var owner = MockAisling.Create();
        var other = MockAisling.Create();
        var observer = new ExchangeObserver(owner, other);

        var hash1 = observer.GetHashCode();
        var hash2 = observer.GetHashCode();

        hash1.Should()
             .Be(hash2);
    }

    [Test]
    public void GetHashCode_SameOwnerAndOther_ShouldBeEqual()
    {
        var owner = MockAisling.Create();
        var other = MockAisling.Create();
        var observer1 = new ExchangeObserver(owner, other);
        var observer2 = new ExchangeObserver(owner, other);

        observer1.GetHashCode()
                 .Should()
                 .Be(observer2.GetHashCode());
    }

    [Test]
    public void OperatorEquals_SameOwnerAndOther_ReturnsTrue()
    {
        var owner = MockAisling.Create();
        var other = MockAisling.Create();
        var observer1 = new ExchangeObserver(owner, other);
        var observer2 = new ExchangeObserver(owner, other);

        var result = observer1 == observer2;

        result.Should()
              .BeTrue();
    }

    [Test]
    public void OperatorNotEquals_DifferentOwner_ReturnsTrue()
    {
        var owner1 = MockAisling.Create();
        var owner2 = MockAisling.Create();
        var other = MockAisling.Create();
        var observer1 = new ExchangeObserver(owner1, other);
        var observer2 = new ExchangeObserver(owner2, other);

        var result = observer1 != observer2;

        result.Should()
              .BeTrue();
    }

    [Test]
    public void OperatorEquals_BothNull_ReturnsTrue()
    {
        ExchangeObserver? left = null;
        ExchangeObserver? right = null;

        var result = left == right;

        result.Should()
              .BeTrue();
    }

    [Test]
    public void OperatorEquals_LeftNull_ReturnsFalse()
    {
        ExchangeObserver? left = null;
        var right = new ExchangeObserver(MockAisling.Create(), MockAisling.Create());

        var result = left == right;

        result.Should()
              .BeFalse();
    }
    #endregion
}