using Chaos.Formulae;
using Chaos.Scripts.RuntimeScripts.Abstractions;
using Chaos.Scripts.RuntimeScripts.NaturalRegeneration;

namespace Chaos.Scripts.RuntimeScripts;

public static class NaturalRegenerationScripts
{
    public static INaturalRegenerationScript Default { get; } = new DefaultNaturalRegenerationScript(RegenFormulae.Default);
}