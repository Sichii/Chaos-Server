using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.LevelUp;
using Chaos.Services.Servers.Options;

namespace Chaos.Scripts.FunctionalScripts.ExperienceDistribution;

public class DefaultExperienceDistributionScript : ScriptBase, IExperienceDistributionScript
{
    public IExperienceFormula ExperienceFormula { get; set; }
    public ILevelUpScript LevelUpScript { get; set; }

    /// <inheritdoc />
    public static string Key { get; } = GetScriptKey(typeof(DefaultExperienceDistributionScript));

    public DefaultExperienceDistributionScript()
    {
        ExperienceFormula = ExperienceFormulae.Default;
        LevelUpScript = DefaultLevelUpScript.Create();
    }

    /// <inheritdoc />
    public static IExperienceDistributionScript Create() => FunctionalScriptRegistry.Instance.Get<IExperienceDistributionScript>(Key);

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
                LevelUpScript.LevelUp(aisling);

            if (aisling.UserStatSheet.Level >= WorldOptions.Instance.MaxLevel)
                break;
        }

        aisling.Client.SendAttributes(StatUpdateType.Full);
    }
}