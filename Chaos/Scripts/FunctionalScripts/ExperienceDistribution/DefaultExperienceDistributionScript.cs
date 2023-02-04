using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.LevelUp;
using Chaos.Services.Servers.Options;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripts.FunctionalScripts.ExperienceDistribution;

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
            Logger.LogError("Tried to give negative amount ({Amount}) experience to {Aisling}", amount, aisling);

        if (amount + aisling.UserStatSheet.TotalExp > uint.MaxValue)
            amount = uint.MaxValue - aisling.UserStatSheet.TotalExp;

        //if you're at max level, you don't gain exp
        //feel free to put a message here if you want
        if ((amount <= 0) || (aisling.UserStatSheet.Level >= WorldOptions.Instance.MaxLevel))
            return;

        aisling.SendActiveMessage($"You have gained {amount} experience!");
        Logger.LogTrace("{Aisling} has gained {Amount} experience", aisling, amount);

        while (amount > 0)
        {
            var expToGive = Math.Min(amount, aisling.UserStatSheet.ToNextLevel);
            aisling.UserStatSheet.AddTotalExp(expToGive);
            aisling.UserStatSheet.TakeTnl(expToGive);

            amount -= expToGive;

            if (aisling.UserStatSheet.ToNextLevel <= 0)
                LevelUpScript.LevelUp(aisling);

            if (aisling.UserStatSheet.Level >= WorldOptions.Instance.MaxLevel)
                break;
        }

        aisling.Client.SendAttributes(StatUpdateType.ExpGold);

        if (aisling.UserStatSheet.TotalExp == uint.MaxValue)
            aisling.SendActiveMessage("You cannot gain any more experience");
    }

    public virtual bool TryTakeExp(Aisling aisling, long amount)
    {
        if (amount < 0)
        {
            Logger.LogError("Tried to take negative amount ({Amount}) experience from {Aisling}", amount, aisling);

            return false;
        }

        if (aisling.UserStatSheet.TotalExp < amount)
            return false;

        var success = aisling.UserStatSheet.Assert(
            statRef =>
            {
                //try to take the exp
                var rst = Interlocked.Add(ref statRef.TotalExp, -amount);

                //if this results in less than 0 exp
                if (rst < 0)
                {
                    //put it back, return false
                    Interlocked.Add(ref statRef.TotalExp, amount);

                    return false;
                }

                return true;
            });

        if (success)
            Logger.LogTrace("{Aisling} has lost {Amount} experience", aisling, amount);

        aisling.Client.SendAttributes(StatUpdateType.ExpGold);

        return success;
    }
}