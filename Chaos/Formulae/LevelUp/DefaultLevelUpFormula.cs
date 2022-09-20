using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Services.Hosted.Options;

namespace Chaos.Formulae.LevelUp;

public class DefaultLevelUpFormula : ILevelUpFormula
{
    /// <inheritdoc />
    public void LevelUp(Aisling aisling)
    {
        aisling.UserStatSheet.IncrementLevel();
        aisling.UserStatSheet.GivePoints(2);

        if (aisling.UserStatSheet.Level < WorldOptions.Instance.MaxLevel)
        {
            var newTnl = (long)Math.Pow(aisling.UserStatSheet.Level, 2) * 200;
            aisling.UserStatSheet.AddTNL(newTnl);
        }

        var levelUpAttribs = new Attributes
        {
            MaximumHp = Convert.ToInt32(aisling.UserStatSheet.Level * 0.3m) + 10,
            MaximumMp = Convert.ToInt32(aisling.UserStatSheet.Level * 0.15m) + 5
        };

        aisling.UserStatSheet.Add(levelUpAttribs);
        aisling.UserStatSheet.RecalculateMaxWeight();

        aisling.Client.SendAttributes(StatUpdateType.Full);
    }
}