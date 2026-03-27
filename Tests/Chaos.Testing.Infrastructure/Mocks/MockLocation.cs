using Chaos.Geometry.Abstractions;
using Moq;

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockLocation
{
    public static Mock<ILocation> Create(
        string map,
        int x,
        int y,
        Action<Mock<ILocation>>? setup = null)
    {
        var mock = new Mock<ILocation>();

        mock.SetupGet(l => l.Map)
            .Returns(map);

        mock.SetupGet(l => l.X)
            .Returns(x);

        mock.SetupGet(l => l.Y)
            .Returns(y);
        setup?.Invoke(mock);

        return mock;
    }
}