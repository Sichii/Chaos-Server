using Chaos.Messaging.Abstractions;
using Moq;

namespace Chaos.Testing.Infrastructure.Mocks;

public class MockCommandSubject
{
    public static Mock<ICommandSubject> Create(string name, bool isAdmin)
    {
        var mock = new Mock<ICommandSubject>();

        mock.Setup(s => s.Name)
            .Returns(name);

        mock.Setup(s => s.IsAdmin)
            .Returns(isAdmin);

        return mock;
    }
}