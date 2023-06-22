using Microsoft.Extensions.Options;
using Moq;

namespace Chaos.Testing.Infrastructure.Mocks;

public class MockOptions
{
    public static Mock<IOptions<T>> Create<T>(T value) where T: class, new()
    {
        var options = new Mock<IOptions<T>>();
        options.Setup(o => o.Value).Returns(value);

        return options;
    }

    public static Mock<IOptions<T>> Create<T>(Action<T>? setup = null) where T: class, new()
    {
        var value = new T();
        setup?.Invoke(value);

        return Create(value);
    }
}