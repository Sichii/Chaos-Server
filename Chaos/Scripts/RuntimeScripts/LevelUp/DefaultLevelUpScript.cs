using Chaos.Common.Definitions;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.RuntimeScripts.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Scripts.RuntimeScripts.LevelUp;

public class DefaultLevelUpScript : ScriptBase, ILevelUpScript
{
    public ILevelUpFormula LevelUpFormula { get; }

    public DefaultLevelUpScript(ILevelUpFormula levelUpFormula) => LevelUpFormula = levelUpFormula;

    /// <inheritdoc />
    public virtual void LevelUp(Aisling aisling)
    {
        aisling.UserStatSheet.IncrementLevel();
        aisling.UserStatSheet.GivePoints(2);

        if (aisling.UserStatSheet.Level < WorldOptions.Instance.MaxLevel)
        {
            var newTnl = LevelUpFormula.CalculateTnl(aisling);
            aisling.UserStatSheet.AddTNL(newTnl);
        }

        var levelUpAttribs = LevelUpFormula.CalculateAttributesIncrease(aisling);

        aisling.UserStatSheet.Add(levelUpAttribs);
        aisling.UserStatSheet.SetMaxWeight(LevelUpFormula.CalculateMaxWeight(aisling));

        aisling.Client.SendAttributes(StatUpdateType.Full);
    }
}