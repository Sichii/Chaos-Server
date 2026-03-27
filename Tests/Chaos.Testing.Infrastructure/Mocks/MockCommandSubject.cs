#region
using Chaos.Messaging.Abstractions;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public class MockCommandSubject
{
    public static Mock<ICommandSubject> Create() => new();

    public static Mock<ICommandSubject> Create(string name, bool isAdmin)
    {
        var mock = Create();

        mock.Setup(s => s.Name)
            .Returns(name);

        mock.Setup(s => s.IsAdmin)
            .Returns(isAdmin);

        return mock;
    }
}