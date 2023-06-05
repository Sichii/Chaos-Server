using Chaos.Common.Abstractions;

#pragma warning disable CS8618

namespace Chaos.Scripting.Abstractions.Tests.Mocks;

public sealed class MockConfigurableScript : ConfigurableScriptBase<MockScripted>
{
    public int IntProp { get; set; }

    // Define some test properties here
    public string StringProp { get; set; }

    public MockConfigurableScript(MockScripted subject, IScriptVars scriptVars)
        : base(subject, scriptVars) { }

    public MockConfigurableScript(MockScripted subject, Func<string, IScriptVars> scriptVarsFactory)
        : base(subject, scriptVarsFactory) { }
}