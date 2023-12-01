namespace Chaos.Scripting.Abstractions.Tests.Mocks;

public class MockScript : IScript
{
    /// <inheritdoc />
    public string ScriptKey { get; } = Guid.NewGuid()
                                           .ToString();
}