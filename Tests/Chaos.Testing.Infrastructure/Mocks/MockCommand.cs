using Chaos.Collections.Common;
using Chaos.Messaging.Abstractions;
using Moq;

namespace Chaos.Testing.Infrastructure.Mocks;

public class MockCommand
{
    public static Mock<ICommand<ICommandSubject>> Create(Func<ICommandSubject, ArgumentCollection, ValueTask>? execute = null)
    {
        var mock = new Mock<ICommand<ICommandSubject>>();

        mock.Setup(c => c.ExecuteAsync(It.IsAny<ICommandSubject>(), It.IsAny<ArgumentCollection>()))
            .Returns(execute ?? ((_, _) => ValueTask.CompletedTask));

        return mock;
    }
}