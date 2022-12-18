using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class EquipmentScript : ConfigurableItemScriptBase
{
    protected AdvClass AdvClass { get; init; } = AdvClass.None;
    protected BaseClass BaseClass { get; init; } = BaseClass.Any;
    protected EquipmentType EquipmentType { get; init; }
    protected Gender Gender { get; init; } = Gender.Unisex;
    protected int? MinLevel { get; init; }
    protected int? StatAmountRequired { get; init; }
    protected Stat? StatRequired { get; init; }

    public EquipmentScript(Item subject)
        : base(subject) { }

    public override void OnUse(Aisling source)
    {
        if (!source.IsAlive)
        {
            source.SendOrangeBarMessage("You can't do that");

            return;
        }

        //gender check
        if ((Gender != source.Gender) && (Gender != Gender.Unisex))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if ((source.UserStatSheet.BaseClass != BaseClass.Diacht)
            && (BaseClass != BaseClass.Any)
            && (BaseClass != source.UserStatSheet.BaseClass))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if ((AdvClass != AdvClass.None) && (AdvClass != source.UserStatSheet.AdvClass))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if (MinLevel.HasValue && (MinLevel.Value > source.UserStatSheet.Level))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you, but you could grow into it");

            return;
        }

        if (StatRequired.HasValue
            && StatAmountRequired.HasValue
            && (source.StatSheet.GetBaseStat(StatRequired.Value) < StatAmountRequired.Value))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you, but you could grow into it");

            return;
        }

        source.Equip(EquipmentType, Subject);
    }
}