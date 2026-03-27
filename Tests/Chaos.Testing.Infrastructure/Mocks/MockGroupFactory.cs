#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockGroupFactory
{
    public static Mock<IFactory<Group>> Create(IChannelService? channelService = null, Action<Mock<IFactory<Group>>>? setup = null)
    {
        channelService ??= MockChannelService.Create();
        var mock = new Mock<IFactory<Group>>();

        mock.Setup(f => f.Create(It.IsAny<object[]>()))
            .Returns<object[]>(args =>
            {
                var sender = (Aisling)args[0];
                var receiver = (Aisling)args[1];
                var logger = MockLogger.Create<Group>();

                return new Group(
                    sender,
                    receiver,
                    channelService,
                    logger.Object);
            });

        setup?.Invoke(mock);

        return mock;
    }
}