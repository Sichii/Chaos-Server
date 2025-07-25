#region
using Chaos.DarkAges.Definitions;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
#endregion

namespace Chaos.Scripting.FunctionalScripts.Abstractions;

public interface IApplyDamageScript : IFunctionalScript
{
    IDamageFormula DamageFormula { get; set; }

    int ApplyDamage(
        Creature source,
        Creature target,
        IScript script,
        int damage,
        Element? elementOverride = null);

    static virtual IApplyDamageScript Create() => throw new NotImplementedException("Override this method in your implementation");
}