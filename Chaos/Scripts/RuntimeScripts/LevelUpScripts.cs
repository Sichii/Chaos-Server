using Chaos.Formulae;
using Chaos.Scripts.RuntimeScripts.Abstractions;
using Chaos.Scripts.RuntimeScripts.LevelUp;

namespace Chaos.Scripts.RuntimeScripts;

public static class LevelUpScripts
{
    public static ILevelUpScript Default { get; } = new DefaultLevelUpScript(LevelUpFormulae.Default);
}