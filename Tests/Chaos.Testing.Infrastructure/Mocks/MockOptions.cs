#region
using Microsoft.Extensions.Options;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public class MockOptions
{
    public static Mock<IOptions<T>> Create<T>() where T: class => new();

    public static Mock<IOptions<T>> Create<T>(T value) where T: class
    {
        var options = Create<T>();

        options.Setup(o => o.Value)
               .Returns(value);

        return options;
    }
}