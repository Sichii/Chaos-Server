#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
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

        var exchangeFactoryMock = new Mock<IFactory<Exchange>>();
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
}