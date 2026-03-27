using Chaos.Scripting.Abstractions;

namespace Chaos.Testing.Infrastructure.Mocks;

public sealed class MockScripted : IScripted
{
    public ISet<string> ScriptKeys { get; } = new HashSet<string>();
}