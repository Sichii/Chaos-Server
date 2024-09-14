using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.FunctionalScripts.ApplyHealing;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class VitalityConsumableScript : ConfigurableItemScriptBase,
                                        GenericAbilityComponent<Aisling>.IAbilityComponentOptions,
                                        DamageAbilityComponent.IDamageComponentOptions,
                                        HealAbilityComponent.IHealComponentOptions,
                                        ManaDrainAbilityComponent.IManaDrainComponentOptions,
                                        ManaReplenishAbilityComponent.IManaReplenishComponentOptions,
                                        ConsumableAbilityComponent.IConsumableComponentOptions

{
    /// <inheritdoc />
    public VitalityConsumableScript(Item subject)
        : base(subject)
    {
        ApplyDamageScript = ApplyNonAttackDamageScript.Create();
        ApplyDamageScript.DamageFormula = DamageFormulae.PureDamage;
        ApplyHealScript = ApplyNonAlertingHealScript.Create();
        ApplyHealScript.HealFormula = HealFormulae.Default;
        SourceScript = this;
        ItemName = Subject.DisplayName;
    }

    /// <inheritdoc />
    public override void OnUse(Aisling source)
        => new ComponentExecutor(source, source).WithOptions(this)
                                                .ExecuteAndCheck<GenericAbilityComponent<Aisling>>()
                                                ?.Execute<DamageAbilityComponent>()
                                                .Execute<HealAbilityComponent>()
                                                .Execute<ManaDrainAbilityComponent>()
                                                .Execute<ManaReplenishAbilityComponent>()
                                                .Execute<ConsumableAbilityComponent>();

    #region ScriptVars
    /// <inheritdoc />
    public AoeShape Shape { get; init; }

    /// <inheritdoc />
    public bool SingleTarget { get; init; }

    /// <inheritdoc />
    public TargetFilter Filter { get; init; }

    /// <inheritdoc />
    public int Range { get; init; }

    /// <inheritdoc />
    public bool ExcludeSourcePoint { get; init; }

    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }

    /// <inheritdoc />
    public byte? Sound { get; init; }

    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }

    /// <inheritdoc />
    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }

    /// <inheritdoc />
    public ushort? AnimationSpeed { get; init; }

    /// <inheritdoc />
    public Animation? Animation { get; init; }

    /// <inheritdoc />
    public bool AnimatePoints { get; init; }

    /// <inheritdoc />
    public int? ManaCost { get; init; }

    /// <inheritdoc />
    public decimal PctManaCost { get; init; }

    /// <inheritdoc />
    public bool ShouldNotBreakHide { get; init; }

    /// <inheritdoc />
    public IApplyDamageScript ApplyDamageScript { get; init; }

    /// <inheritdoc />
    public int? BaseDamage { get; init; }

    /// <inheritdoc />
    public Stat? DamageStat { get; init; }

    /// <inheritdoc />
    public decimal? DamageStatMultiplier { get; init; }

    /// <inheritdoc />
    public Element? Element { get; init; }

    /// <inheritdoc />
    public decimal? PctHpDamage { get; init; }

    /// <inheritdoc />
    public IApplyHealScript ApplyHealScript { get; init; }

    /// <inheritdoc />
    public int? BaseHeal { get; init; }

    /// <inheritdoc />
    public Stat? HealStat { get; init; }

    /// <inheritdoc />
    public decimal? HealStatMultiplier { get; init; }

    /// <inheritdoc />
    public decimal? PctHpHeal { get; init; }

    public IScript SourceScript { get; init; }

    /// <inheritdoc />
    public int? ManaDrain { get; init; }

    /// <inheritdoc />
    public decimal PctManaDrain { get; init; }

    /// <inheritdoc />
    public int? ManaReplenish { get; init; }

    /// <inheritdoc />
    public decimal PctManaReplenish { get; init; }

    /// <inheritdoc />
    public string ItemName { get; init; }
    #endregion
}