#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockAisling
{
    private static int Counter;

    public static Aisling Create(
        MapInstance? mapInstance = null,
        string? name = null,
        IPoint? position = null,
        Action<Aisling>? setup = null)
    {
        mapInstance ??= MockMapInstance.Create();
        name ??= $"TestAisling{Interlocked.Increment(ref Counter)}";
        position ??= new Point(5, 5);

        var exchangeLoggerMock = new Mock<ILogger<Exchange>>();
        var exchangeFactoryMock = new Mock<IFactory<Exchange>>();

        exchangeFactoryMock.Setup(f => f.Create(It.IsAny<object[]>()))
                           .Returns((object[] args) => new Exchange((Aisling)args[0], (Aisling)args[1], exchangeLoggerMock.Object));

        var loggerMock = new Mock<ILogger<Aisling>>();

        var aisling = new Aisling(
            name,
            mapInstance,
            position,
            exchangeFactoryMock.Object,
            MockScriptProvider.Instance.Object,
            loggerMock.Object,
            MockScriptProvider.ItemCloner.Object);

        var clientMock = new Mock<IChaosWorldClient>();

        clientMock.SetupGet(c => c.Aisling)
                  .Returns(aisling);

        aisling.Client = clientMock.Object;

        setup?.Invoke(aisling);

        return aisling;
    }

    /// <summary>
    ///     Configures the aisling's script mock to allow all actions (CanUseItem, CanMove, CanPickupItem, etc.)
    /// </summary>
    public static void SetupScriptAllows(Aisling aisling)
    {
        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanUseItem(It.IsAny<Item>()))
                  .Returns(true);

        scriptMock.Setup(s => s.CanUseSkill(It.IsAny<Skill>()))
                  .Returns(true);

        scriptMock.Setup(s => s.CanUseSpell(It.IsAny<Spell>()))
                  .Returns(true);

        scriptMock.Setup(s => s.CanDropItem(It.IsAny<Item>()))
                  .Returns(true);

        scriptMock.Setup(s => s.CanDropMoney(It.IsAny<int>()))
                  .Returns(true);

        scriptMock.Setup(s => s.CanDropMoneyOn(It.IsAny<Aisling>(), It.IsAny<int>()))
                  .Returns(true);

        scriptMock.Setup(s => s.CanDropItemOn(It.IsAny<Aisling>(), It.IsAny<Item>()))
                  .Returns(true);

        scriptMock.Setup(s => s.CanPickupItem(It.IsAny<GroundItem>()))
                  .Returns(true);

        scriptMock.Setup(s => s.CanPickupMoney(It.IsAny<Money>()))
                  .Returns(true);

        scriptMock.Setup(s => s.CanMove())
                  .Returns(true);

        scriptMock.Setup(s => s.CanTurn())
                  .Returns(true);

        scriptMock.Setup(s => s.CanTalk())
                  .Returns(true);

        scriptMock.Setup(s => s.CanSee(It.IsAny<VisibleEntity>()))
                  .Returns(true);
    }
}