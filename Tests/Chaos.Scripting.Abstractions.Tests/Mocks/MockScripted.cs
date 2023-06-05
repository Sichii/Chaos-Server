namespace Chaos.Scripting.Abstractions.Tests.Mocks;

public sealed class MockScripted : IScripted
{
    public ISet<string> ScriptKeys { get; } = new HashSet<string>();
}