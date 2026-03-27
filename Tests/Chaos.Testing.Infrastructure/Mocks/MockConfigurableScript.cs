using Chaos.Common.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Testing.Infrastructure.Mocks;

public sealed class MockConfigurableScript : ConfigurableScriptBase<MockScripted>
{
    public int IntProp { get; set; }
    public string StringProp { get; set; } = null!;

    public MockConfigurableScript(MockScripted subject, IScriptVars scriptVars)
        : base(subject, scriptVars) { }

    public MockConfigurableScript(MockScripted subject, Func<string, IScriptVars> scriptVarsFactory)
        : base(subject, scriptVarsFactory) { }
}