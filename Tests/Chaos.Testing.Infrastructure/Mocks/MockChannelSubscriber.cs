#region
using Chaos.Messaging.Abstractions;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public class MockChannelSubscriber
{
    public static Mock<IChannelSubscriber> Create()
    {
        var mock = new Mock<IChannelSubscriber>();

        return mock;
    }

    public static Mock<IChannelSubscriber> Create(string name)
    {
        var mock = Create();

        mock.Setup(s => s.Name)
            .Returns(name);

        return mock;
    }
}