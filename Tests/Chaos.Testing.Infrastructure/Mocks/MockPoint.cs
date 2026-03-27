using Chaos.Geometry.Abstractions;
using Moq;

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockPoint
{
    public static Mock<IPoint> Create(int x, int y, Action<Mock<IPoint>>? setup = null)
    {
        var mock = new Mock<IPoint>();

        mock.SetupGet(p => p.X)
            .Returns(x);

        mock.SetupGet(p => p.Y)
            .Returns(y);
        setup?.Invoke(mock);

        return mock;
    }
}