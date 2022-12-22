using Chaos.Formulae;
using Chaos.Scripts.RuntimeScripts.Abstractions;
using Chaos.Scripts.RuntimeScripts.ApplyDamage;

namespace Chaos.Scripts.RuntimeScripts;

public static class ApplyDamageScripts
{
    public static IApplyDamageScript Default { get; } = new DefaultApplyDamageScript(DamageFormulae.Default);
    public static IApplyDamageScript Pure { get; } = new DefaultApplyDamageScript(DamageFormulae.PureDamage);
}