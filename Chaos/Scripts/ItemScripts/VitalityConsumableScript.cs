using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class VitalityConsumableScript : ConfigurableItemScriptBase
{
    protected int? HealthAmount { get; init; }
    protected int? HealthPercent { get; init; }
    protected int? ManaAmount { get; init; }
    protected int? ManaPercent { get; init; }

    /// <inheritdoc />
    public VitalityConsumableScript(Item subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnUse(Aisling source)
    {
        if (HealthAmount.HasValue)
            source.UserStatSheet.AddHp(HealthAmount.Value);

        if (HealthPercent.HasValue)
            source.UserStatSheet.AddHealthPct(HealthPercent.Value);

        if (ManaAmount.HasValue)
            source.UserStatSheet.AddMp(ManaAmount.Value);

        if (ManaPercent.HasValue)
            source.UserStatSheet.AddManaPct(ManaPercent.Value);

        source.Client.SendAttributes(StatUpdateType.Vitality);
        source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
    }
}