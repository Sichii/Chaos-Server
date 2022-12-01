using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Services.Servers.Options;

namespace Chaos.Formulae.LevelUp;

public class DefaultLevelUpFormula : ILevelUpFormula
{
    public virtual Attributes GetLevelUpAttributes(Aisling aisling)
    {
        var levelUpAttribs = new Attributes
        {
            //each level, add (Level * 0.3) + 10 hp
            MaximumHp = Convert.ToInt32(aisling.UserStatSheet.Level * 0.3m) + 10,
            //each level, add (Level * 0.15) + 5 mp
            MaximumMp = Convert.ToInt32(aisling.UserStatSheet.Level * 0.15m) + 5,
            //every 3 levels, subtract 1 ac
            Ac = aisling.StatSheet.Level % 3 == 0 ? -1 : 0
        };

        return levelUpAttribs;
    }

    protected virtual int GetNewTnl(Aisling aisling) => Convert.ToInt32(Math.Pow(aisling.UserStatSheet.Level, 2) * 200);

    /// <inheritdoc />
    public virtual void LevelUp(Aisling aisling)
    {
        aisling.UserStatSheet.IncrementLevel();
        aisling.UserStatSheet.GivePoints(2);

        if (aisling.UserStatSheet.Level < WorldOptions.Instance.MaxLevel)
        {
            var newTnl = GetNewTnl(aisling);
            aisling.UserStatSheet.AddTNL(newTnl);
        }

        var levelUpAttribs = GetLevelUpAttributes(aisling);

        aisling.UserStatSheet.Add(levelUpAttribs);
        aisling.UserStatSheet.RecalculateMaxWeight();

        aisling.Client.SendAttributes(StatUpdateType.Full);
    }
}