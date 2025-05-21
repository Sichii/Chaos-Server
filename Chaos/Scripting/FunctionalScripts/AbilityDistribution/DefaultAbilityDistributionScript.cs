#region
using System.Diagnostics;
using Chaos.DarkAges.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.AbilityUp;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Services.Servers.Options;
#endregion

namespace Chaos.Scripting.FunctionalScripts.AbilityDistribution;

public class DefaultAbilityDistributionScript : ScriptBase, IAbilityDistributionScript
{
    /// <inheritdoc />
    public IAbilityFormula AbilityFormula { get; set; } = AbilityFormulae.Default;

    /// <inheritdoc />
    public IAbilityUpScript AbilityUpScript { get; set; } = DefaultAbilityUpScript.Create();

    public ILogger<DefaultAbilityDistributionScript> Logger { get; set; }

    /// <inheritdoc />
    public static string Key { get; } = GetScriptKey(typeof(DefaultAbilityDistributionScript));

    public DefaultAbilityDistributionScript(ILogger<DefaultAbilityDistributionScript> logger) => Logger = logger;

    /// <inheritdoc />
    public static IAbilityDistributionScript Create() => FunctionalScriptRegistry.Instance.Get<IAbilityDistributionScript>(Key);

    /// <inheritdoc />
    public void DistributeAbility(Creature killedCreature, params ICollection<Aisling> aislings)
    {
        var ability = AbilityFormula.Calculate(killedCreature, aislings);

        foreach (var aisling in aislings)
            GiveAbility(aisling, ability);
    }

    /// <inheritdoc />
    public void GiveAbility(Aisling aisling, long amount)
    {
        if (amount < 0)
        {
            var stackTrace = new StackTrace(true).ToString();

            Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.AbilityExp, Topics.Actions.Add)
                  .WithProperty(aisling)
                  .WithProperty(stackTrace)
                  .LogError("Tried to give {Amount:N0} ability to {@AislingName}", amount, aisling.Name);

            return;
        }

        if ((amount + aisling.UserStatSheet.TotalAbility) > uint.MaxValue)
            amount = uint.MaxValue - aisling.UserStatSheet.TotalAbility;

        if (amount <= 0)
            return;

        aisling.SendActiveMessage($"You have gained {amount:N0} ability!");

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.AbilityExp, Topics.Actions.Add)
              .WithProperty(aisling)
              .LogInformation("Aisling {@AislingName} has gained {Amount:N0} ability", aisling.Name, amount);

        while (amount > 0)
            if (aisling.UserStatSheet.AbilityLevel >= WorldOptions.Instance.MaxAbilityLevel)
                amount = 0;
            else
            {
                var abilityToGive = Math.Min(amount, aisling.UserStatSheet.ToNextAbility);
                aisling.UserStatSheet.AddTotalAbility(abilityToGive);
                aisling.UserStatSheet.SubtractTna(abilityToGive);

                amount -= abilityToGive;

                if (aisling.UserStatSheet.ToNextAbility <= 0)
                    AbilityUpScript.AbilityUp(aisling);
            }

        aisling.Client.SendAttributes(StatUpdateType.ExpGold);

        if (aisling.UserStatSheet.TotalAbility == uint.MaxValue)
            aisling.SendActiveMessage("You cannot gain any more ability");
    }
}