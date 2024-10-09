using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class SetVitalityConsumable : ConfigurableItemScriptBase
{
    /// <inheritdoc />
    public SetVitalityConsumable(Item subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnUse(Aisling source)
    {
        if (SetHealthAmount.HasValue)
            if (SetHealthAmount == 0)
                throw new InvalidOperationException("Cannot set health to 0, use a death script instead");
            else
                source.UserStatSheet.SetHp(SetHealthAmount.Value);

        if (SetHealthPercent.HasValue)
            if (SetHealthPercent == 0)
                throw new InvalidOperationException("Cannot set health to 0, use a death script instead");
            else
                source.UserStatSheet.SetHealthPct(SetHealthPercent.Value);

        if (SetManaAmount.HasValue)
            source.UserStatSheet.SetMp(SetManaAmount.Value);

        if (SetManaPercent.HasValue)
            source.UserStatSheet.SetManaPct(SetManaPercent.Value);

        source.Client.SendAttributes(StatUpdateType.Vitality);
        source.Inventory.RemoveQuantity(Subject.Slot, 1);
    }

    #region ScriptVars
    protected int? SetHealthAmount { get; init; }
    protected int? SetHealthPercent { get; init; }
    protected int? SetManaAmount { get; init; }
    protected int? SetManaPercent { get; init; }
    #endregion
}