using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class SetVitalityConsumable : ConfigurableItemScriptBase
{
    /// <inheritdoc />
    public SetVitalityConsumable(Item subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnUse(Aisling source)
    {
        if (SetHealthAmount.HasValue)
            source.UserStatSheet.SetHp(SetHealthAmount.Value);

        if (SetHealthPercent.HasValue)
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