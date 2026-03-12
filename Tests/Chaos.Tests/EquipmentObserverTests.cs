#region
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Observers;
using Chaos.Testing.Infrastructure.Mocks;
using IObserver_Item = Chaos.Observers.Abstractions.IObserver<Chaos.Models.Panel.Item>;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class EquipmentObserverTests
{
    #region OnUpdated
    [Test]
    public void OnUpdated_ShouldNotThrow()
    {
        var aisling = MockAisling.Create();
        var item = MockItem.Create();

        var observer = new EquipmentObserver(aisling);

        var act = () => observer.OnUpdated(1, item);

        act.Should()
           .NotThrow();
    }
    #endregion

    #region Equals(IObserver<Item>?)
    [Test]
    public void Equals_Null_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var observer = new EquipmentObserver(aisling);

        var result = observer.Equals(null);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_SameReference_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer = new EquipmentObserver(aisling);

        var result = observer.Equals(observer);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void Equals_SameAisling_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer1 = new EquipmentObserver(aisling);
        var observer2 = new EquipmentObserver(aisling);

        var result = observer1.Equals(observer2);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void Equals_DifferentAisling_ReturnsFalse()
    {
        var aisling1 = MockAisling.Create();
        var aisling2 = MockAisling.Create();
        var observer1 = new EquipmentObserver(aisling1);
        var observer2 = new EquipmentObserver(aisling2);

        var result = observer1.Equals(observer2);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void Equals_DifferentObserverType_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var equipObserver = new EquipmentObserver(aisling);
        var invObserver = new InventoryObserver(aisling);

        var result = equipObserver.Equals(invObserver);

        result.Should()
              .BeFalse();
    }
    #endregion

    #region Equals(object?)
    [Test]
    public void EqualsObject_SameReference_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer = new EquipmentObserver(aisling);

        var result = observer.Equals((object)observer);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void EqualsObject_DifferentType_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var observer = new EquipmentObserver(aisling);

        // ReSharper disable once SuspiciousTypeConversion.Global
        var result = observer.Equals("not an observer");

        result.Should()
              .BeFalse();
    }

    [Test]
    public void EqualsObject_Null_ReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var observer = new EquipmentObserver(aisling);

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
        var observer = new EquipmentObserver(aisling);

        var hash1 = observer.GetHashCode();
        var hash2 = observer.GetHashCode();

        hash1.Should()
             .Be(hash2);
    }

    [Test]
    public void GetHashCode_SameAisling_ShouldBeEqual()
    {
        var aisling = MockAisling.Create();
        var observer1 = new EquipmentObserver(aisling);
        var observer2 = new EquipmentObserver(aisling);

        observer1.GetHashCode()
                 .Should()
                 .Be(observer2.GetHashCode());
    }

    [Test]
    public void OperatorEquals_SameAisling_ReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var observer1 = new EquipmentObserver(aisling);
        var observer2 = new EquipmentObserver(aisling);

        var result = observer1 == observer2;

        result.Should()
              .BeTrue();
    }

    [Test]
    public void OperatorNotEquals_DifferentAisling_ReturnsTrue()
    {
        var aisling1 = MockAisling.Create();
        var aisling2 = MockAisling.Create();
        var observer1 = new EquipmentObserver(aisling1);
        var observer2 = new EquipmentObserver(aisling2);

        var result = observer1 != observer2;

        result.Should()
              .BeTrue();
    }

    [Test]
    public void OperatorEquals_BothNull_ReturnsTrue()
    {
        EquipmentObserver? left = null;
        EquipmentObserver? right = null;

        var result = left == right;

        result.Should()
              .BeTrue();
    }

    [Test]
    public void OperatorEquals_LeftNull_ReturnsFalse()
    {
        EquipmentObserver? left = null;
        var right = new EquipmentObserver(MockAisling.Create());

        var result = left == right;

        result.Should()
              .BeFalse();
    }
    #endregion

    #region OnAdded
    [Test]
    public void OnAdded_WithModifiers_ShouldCallAddBonusAndNotifyClient()
    {
        var aisling = MockAisling.Create();
        var clientMock = Mock.Get(aisling.Client);

        var modifiers = new Attributes
        {
            Str = 5,
            Con = 3
        };

        var item = MockItem.Create(
            setup: i =>
            {
                i.Modifiers = modifiers;
                i.Slot = 1;
            });

        var observer = new EquipmentObserver(aisling);

        observer.OnAdded(item);

        clientMock.Verify(c => c.SendEquipment(item), Times.Once);
        clientMock.Verify(c => c.SendAttributes(StatUpdateType.Full), Times.Once);

        Mock.Get(item.Script)
            .Verify(s => s.OnEquipped(aisling), Times.Once);
    }

    [Test]
    public void OnAdded_WithDefaultModifiers_ShouldStillCallAddBonusAndNotifyClient()
    {
        var aisling = MockAisling.Create();
        var clientMock = Mock.Get(aisling.Client);

        // Default MockItem has Modifiers = new Attributes() (all zeros)
        // new Attributes().Equals(null) returns false, so !false = true, AddBonus is called
        var item = MockItem.Create(setup: i => i.Slot = 1);

        var observer = new EquipmentObserver(aisling);

        observer.OnAdded(item);

        clientMock.Verify(c => c.SendEquipment(item), Times.Once);
        clientMock.Verify(c => c.SendAttributes(StatUpdateType.Full), Times.Once);

        Mock.Get(item.Script)
            .Verify(s => s.OnEquipped(aisling), Times.Once);
    }

    [Test]
    public void OnAdded_ShouldAddWeight()
    {
        var aisling = MockAisling.Create();
        var item = MockItem.Create(setup: i => i.Weight = 5);

        var initialWeight = aisling.UserStatSheet.CurrentWeight;

        var observer = new EquipmentObserver(aisling);
        observer.OnAdded(item);

        aisling.UserStatSheet
               .CurrentWeight
               .Should()
               .Be(initialWeight + 5);
    }
    #endregion

    #region OnRemoved
    [Test]
    public void OnRemoved_WithModifiers_ShouldSubtractBonusAndNotifyClient()
    {
        var aisling = MockAisling.Create();
        var clientMock = Mock.Get(aisling.Client);

        var modifiers = new Attributes
        {
            Str = 5,
            Con = 3
        };

        var item = MockItem.Create(
            setup: i =>
            {
                i.Modifiers = modifiers;
                i.Slot = 1;
            });

        var observer = new EquipmentObserver(aisling);
        byte slot = 1;

        observer.OnRemoved(slot, item);

        clientMock.Verify(c => c.SendDisplayUnequip((EquipmentSlot)slot), Times.Once);
        clientMock.Verify(c => c.SendSelfProfile(), Times.Once);
        clientMock.Verify(c => c.SendAttributes(StatUpdateType.Full), Times.Once);

        Mock.Get(item.Script)
            .Verify(s => s.OnUnEquipped(aisling), Times.Once);
    }

    [Test]
    public void OnRemoved_WithDefaultModifiers_ShouldStillCallSubtractBonusAndNotifyClient()
    {
        var aisling = MockAisling.Create();
        var clientMock = Mock.Get(aisling.Client);

        // Default MockItem has Modifiers = new Attributes() (all zeros)
        var item = MockItem.Create(setup: i => i.Slot = 1);

        var observer = new EquipmentObserver(aisling);
        byte slot = 1;

        observer.OnRemoved(slot, item);

        clientMock.Verify(c => c.SendDisplayUnequip((EquipmentSlot)slot), Times.Once);
        clientMock.Verify(c => c.SendSelfProfile(), Times.Once);
        clientMock.Verify(c => c.SendAttributes(StatUpdateType.Full), Times.Once);

        Mock.Get(item.Script)
            .Verify(s => s.OnUnEquipped(aisling), Times.Once);
    }

    [Test]
    public void OnRemoved_ShouldSubtractWeight()
    {
        var aisling = MockAisling.Create();
        var item = MockItem.Create(setup: i => i.Weight = 5);

        // First add weight so we can observe the subtraction
        aisling.UserStatSheet.AddWeight(5);
        var weightBeforeRemove = aisling.UserStatSheet.CurrentWeight;

        var observer = new EquipmentObserver(aisling);
        observer.OnRemoved(1, item);

        aisling.UserStatSheet
               .CurrentWeight
               .Should()
               .Be(weightBeforeRemove - 5);
    }
    #endregion
}