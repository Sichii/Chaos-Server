#region
using Chaos.Scripting.Abstractions;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockCompositeScript
{
    public static Mock<CompositeScriptBase<IScript>> Create(params ICollection<IScript> scripts)
    {
        var mock = new Mock<CompositeScriptBase<IScript>>();

        foreach (var script in scripts)
            mock.Object.Add(script);

        return mock;
    }
}