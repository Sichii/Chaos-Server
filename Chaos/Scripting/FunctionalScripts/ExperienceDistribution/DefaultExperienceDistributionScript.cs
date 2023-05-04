using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.LevelUp;
using Chaos.Services.Servers.Options;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

public class DefaultExperienceDistributionScript : ScriptBase, IExperienceDistributionScript
{
    public IExperienceFormula ExperienceFormula { get; set; }
    public ILevelUpScript LevelUpScript { get; set; }
    public ILogger<DefaultExperienceDistributionScript> Logger { get; set; }

    /// <inheritdoc />
    public static string Key { get; } = GetScriptKey(typeof(DefaultExperienceDistributionScript));

    public DefaultExperienceDistributionScript(ILogger<DefaultExperienceDistributionScript> logger)
    {
        ExperienceFormula = ExperienceFormulae.Default;
        LevelUpScript = DefaultLevelUpScript.Create();
        Logger = logger;
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
        if (amount < 0)
            Logger.LogError("Tried to give negative amount ({Amount}) experience to {@Client}", amount, aisling.Client);

        if (amount + aisling.UserStatSheet.TotalExp > uint.MaxValue)
            amount = uint.MaxValue - aisling.UserStatSheet.TotalExp;

        if (amount <= 0)
            return;

        aisling.SendActiveMessage($"You have gained {amount} experience!");
        Logger.LogTrace("{@Player} has gained {ExpAmount} experience", aisling, amount);

        while (amount > 0)
            if (aisling.UserStatSheet.Level >= WorldOptions.Instance.MaxLevel)
            {
                aisling.UserStatSheet.AddTotalExp(amount);
                amount = 0;
            } else
            {
                var expToGive = Math.Min(amount, aisling.UserStatSheet.ToNextLevel);
                aisling.UserStatSheet.AddTotalExp(expToGive);
                aisling.UserStatSheet.SubtractTnl(expToGive);

                amount -= expToGive;

                if (aisling.UserStatSheet.ToNextLevel <= 0)
                    LevelUpScript.LevelUp(aisling);
            }

        aisling.Client.SendAttributes(StatUpdateType.ExpGold);

        if (aisling.UserStatSheet.TotalExp == uint.MaxValue)
            aisling.SendActiveMessage("You cannot gain any more experience");
    }

    public virtual bool TryTakeExp(Aisling aisling, long amount)
    {
        if (amount < 0)
        {
            Logger.LogError("Tried to take negative amount ({Amount}) experience from {@Client}", amount, aisling.Client);

            return false;
        }

        if (aisling.UserStatSheet.TotalExp < amount)
            return false;

        if (!aisling.UserStatSheet.TrySubtractTotalExp(amount))
            return false;

        Logger.LogTrace("{@Player} has lost {ExpAmount} experience", aisling, amount);

        aisling.Client.SendAttributes(StatUpdateType.ExpGold);

        return true;
    }
}