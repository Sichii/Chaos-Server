using Chaos.Common.Definitions;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.RuntimeScripts.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Scripts.RuntimeScripts.ExperienceDistribution;

public class DefaultExperienceDistributionScript : ScriptBase, IExperienceDistributionScript
{
    public IExperienceFormula ExperienceFormula { get; }

    public DefaultExperienceDistributionScript(IExperienceFormula experienceFormula) => ExperienceFormula = experienceFormula;

    /// <inheritdoc />
    public virtual void DistributeExperience(Creature killedCreature, params Aisling[] aislings)
    {
        var exp = ExperienceFormula.Calculate(killedCreature, aislings);

        foreach (var aisling in aislings)
            GiveExp(aisling, exp);
    }

    public virtual void GiveExp(Aisling aisling, long amount)
    {
        if (amount + aisling.UserStatSheet.TotalExp > uint.MaxValue)
            amount = uint.MaxValue - aisling.UserStatSheet.TotalExp;

        //if you're at max level, you don't gain exp
        //feel free to put a message here if you want
        if ((amount <= 0) || (aisling.UserStatSheet.Level >= WorldOptions.Instance.MaxLevel))
            return;

        aisling.SendActiveMessage($"You have gained {amount} experience!");

        while (amount > 0)
        {
            var expToGive = Math.Min(amount, aisling.UserStatSheet.ToNextLevel);
            aisling.UserStatSheet.AddTotalExp(expToGive);
            aisling.UserStatSheet.AddTNL(-expToGive);

            amount -= expToGive;

            if (aisling.UserStatSheet.ToNextLevel <= 0)
                LevelUpScripts.Default.LevelUp(aisling);

            if (aisling.UserStatSheet.Level >= WorldOptions.Instance.MaxLevel)
                break;
        }

        aisling.Client.SendAttributes(StatUpdateType.Full);
    }
}