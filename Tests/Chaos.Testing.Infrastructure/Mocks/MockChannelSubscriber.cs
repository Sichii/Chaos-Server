using Chaos.Messaging.Abstractions;
using Moq;

namespace Chaos.Testing.Infrastructure.Mocks;

public class MockChannelSubscriber
{
    public static Mock<IChannelSubscriber> Create(Action<Mock<IChannelSubscriber>>? setup = null)
    {
        var mock = new Mock<IChannelSubscriber>();
        setup?.Invoke(mock);

        return mock;
    }

    public static Mock<IChannelSubscriber> Create(string name, Action<Mock<IChannelSubscriber>>? setup = null)
    {
        var subscriber = Create(
            mock =>
            {
                setup?.Invoke(mock);

                mock.Setup(s => s.Name)
                    .Returns(name);
            });

        return subscriber;
    }
}