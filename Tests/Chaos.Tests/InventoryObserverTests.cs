#region
using Chaos.DarkAges.Definitions;
using Chaos.Observers;
using Chaos.Testing.Infrastructure.Mocks;
using IObserver_Item = Chaos.Observers.Abstractions.IObserver<Chaos.Models.Panel.Item>;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class InventoryObserverTests
{
    #region OnAdded
    [Test]
    public void OnAdded_ShouldSendItemAndUpdateWeight()
    {
        var aisling = MockAisling.Create();
        var clientMock = Mock.Get(aisling.Client);
        var item = MockItem.Create(setup: i => i.Weight = 3);

        var initialWeight = aisling.UserStatSheet.CurrentWeight;

        var observer = new InventoryObserver(aisling);
        observer.OnAdded(item);

        clientMock.Verify(c => c.SendAddItemToPane(item), Times.Once);
        clientMock.Verify(c => c.SendAttributes(StatUpdateType.Primary), Times.Once);

        aisling.UserStatSheet
               .CurrentWeight
               .Should()
               .Be(initialWeight + 3);
    }
    #endregion

    #region OnRemoved
    [Test]
    public void OnRemoved_ShouldRemoveItemAndUpdateWeight()
    {
        var aisling = MockAisling.Create();
        var clientMock = Mock.Get(aisling.Client);
        var item = MockItem.Create(setup: i => i.Weight = 3);

        aisling.UserStatSheet.AddWeight(3);
        var weightBeforeRemove = aisling.UserStatSheet.CurrentWeight;

        var observer = new InventoryObserver(aisling);
        byte slot = 5;

        observer.OnRemoved(slot, item);

        clientMock.Verify(c => c.SendRemoveItemFromPane(slot), Times.Once);
        clientMock.Verify(c => c.SendAttributes(StatUpdateType.Primary), Times.Once);

        aisling.UserStatSheet
               .CurrentWeight
               .Should()
               .Be(weightBeforeRemove - 3);
    }
    #endregion

    #region OnUpdated
    [Test]
    public void OnUpdated_ShouldSendItem()
    {
        var aisling = MockAisling.Create();
        var clientMock = Mock.Get(aisling.Client);
        var item = MockItem.Create();

        var observer = new InventoryObserver(aisling);

        observer.OnUpdated(1, item);

        clientMock.Verify(c => c.SendAddItemToPane(item), Times.Once);
    }
    #endregion

    #region Equals(IObserver<Item>?)
    [Test]
    public void Equals_Null_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var observer = new InventoryObserver(aisling);

        var result = observer.Equals(null);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_SameReference_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer = new InventoryObserver(aisling);

        var result = observer.Equals(observer);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void Equals_SameAisling_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer1 = new InventoryObserver(aisling);
        var observer2 = new InventoryObserver(aisling);

        var result = observer1.Equals(observer2);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void Equals_DifferentAisling_ReturnsFalse()
    {
        var aisling1 = MockAisling.Create();
        var aisling2 = MockAisling.Create();
        var observer1 = new InventoryObserver(aisling1);
        var observer2 = new InventoryObserver(aisling2);

        var result = observer1.Equals(observer2);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_DifferentObserverType_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var invObserver = new InventoryObserver(aisling);
        var equipObserver = new EquipmentObserver(aisling);

        var result = invObserver.Equals(equipObserver);

        result.Should()
              .BeFalse();
    }
    #endregion

    #region Equals(object?)
    [Test]
    public void EqualsObject_SameReference_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer = new InventoryObserver(aisling);

        var result = observer.Equals((object)observer);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void EqualsObject_DifferentType_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var observer = new InventoryObserver(aisling);

        // ReSharper disable once SuspiciousTypeConversion.Global
        var result = observer.Equals("not an observer");

        result.Should()
              .BeFalse();
    }

    [Test]
    public void EqualsObject_Null_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var observer = new InventoryObserver(aisling);

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
        var observer = new InventoryObserver(aisling);

        var hash1 = observer.GetHashCode();
        var hash2 = observer.GetHashCode();

        hash1.Should()
             .Be(hash2);
    }

    [Test]
    public void GetHashCode_SameAisling_ShouldBeEqual()
    {
        var aisling = MockAisling.Create();
        var observer1 = new InventoryObserver(aisling);
        var observer2 = new InventoryObserver(aisling);

        observer1.GetHashCode()
                 .Should()
                 .Be(observer2.GetHashCode());
    }

    [Test]
    public void OperatorEquals_SameAisling_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer1 = new InventoryObserver(aisling);
        var observer2 = new InventoryObserver(aisling);

        var result = observer1 == observer2;

        result.Should()
              .BeTrue();
    }

    [Test]
    public void OperatorNotEquals_DifferentAisling_ReturnsTrue()
    {
        var aisling1 = MockAisling.Create();
        var aisling2 = MockAisling.Create();
        var observer1 = new InventoryObserver(aisling1);
        var observer2 = new InventoryObserver(aisling2);

        var result = observer1 != observer2;

        result.Should()
              .BeTrue();
    }

    [Test]
    public void OperatorEquals_BothNull_ReturnsTrue()
    {
        InventoryObserver? left = null;
        InventoryObserver? right = null;

        var result = left == right;

        result.Should()
              .BeTrue();
    }

    [Test]
    public void OperatorEquals_LeftNull_ReturnsFalse()
    {
        InventoryObserver? left = null;
        var right = new InventoryObserver(MockAisling.Create());

        var result = left == right;

        result.Should()
              .BeFalse();
    }
    #endregion
}