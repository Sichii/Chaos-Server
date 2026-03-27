#region
using Chaos.Scripting.Abstractions;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockScript
{
    public static Mock<IScript> Create() => new();

    public static Mock<IScript> Create(string key)
    {
        var mock = Create();

        mock.SetupGet(s => s.ScriptKey)
            .Returns(key);

        return mock;
    }
}