#region
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
#endregion

namespace Chaos.Scripting.ItemScripts.Abstractions;

public abstract class SetBonusItemScriptBase : ItemScriptBase
{
    protected abstract Dictionary<int, Attributes> SetBonus { get; }
    protected abstract HashSet<string> SetItemTemplateKeys { get; }

    protected SetBonusItemScriptBase(Item subject)
        : base(subject) { }

    private Attributes GetCumulativeBonus(int currentSetBonusCount)
    {
        var cumulativeBonus = new Attributes();

        for (var i = 0; i <= currentSetBonusCount; i++)
            if (SetBonus.TryGetValue(i, out var bonus))
                cumulativeBonus.Add(bonus);

        return cumulativeBonus;
    }

    public override void OnEquipped(Aisling aisling)
    {
        if (aisling.Equipment.Count(item => item.Template.TemplateKey.EqualsI(Subject.Template.TemplateKey)) > 1)
            return;

        var countOfUniqueSetItems = aisling.Equipment
                                           .DistinctBy(item => item.Template.TemplateKey)
                                           .Count(item => SetItemTemplateKeys.Contains(item.Template.TemplateKey));

        //we have equipped an item, so the previous bonus is from 1 less than the current number of set items
        var previousBonus = GetCumulativeBonus(countOfUniqueSetItems - 1);
        var newBonus = GetCumulativeBonus(countOfUniqueSetItems);

        aisling.UserStatSheet.SubtractBonus(previousBonus);
        aisling.UserStatSheet.AddBonus(newBonus);
    }

    public override void OnUnEquipped(Aisling aisling)
    {
        if (aisling.Equipment.Any(item => item.Template.TemplateKey.EqualsI(Subject.Template.TemplateKey)))
            return;

        var countOfUniqueSetItems = aisling.Equipment
                                           .DistinctBy(item => item.Template.TemplateKey)
                                           .Count(item => SetItemTemplateKeys.Contains(item.Template.TemplateKey));

        //we have unequipped an item, so the previous bonus is from 1 more than the current number of set items
        var previousBonus = GetCumulativeBonus(countOfUniqueSetItems + 1);
        var newBonus = GetCumulativeBonus(countOfUniqueSetItems);

        aisling.UserStatSheet.SubtractBonus(previousBonus);
        aisling.UserStatSheet.AddBonus(newBonus);
    }
}