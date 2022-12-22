using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.RuntimeScripts.Abstractions;

public interface IApplyDamageScript : IScript
{
    void ApplyDamage(
        Creature attacker,
        Creature defender,
        IScript source,
        int damage
    );
}