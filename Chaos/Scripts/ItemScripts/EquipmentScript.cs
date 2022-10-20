using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class EquipmentScript : ConfigurableItemScriptBase
{
    protected AdvClass AdvClass { get; init; } = AdvClass.None;
    protected BaseClass BaseClass { get; init; } = BaseClass.Peasant;
    protected EquipmentType EquipmentType { get; init; }
    protected Gender Gender { get; init; } = Gender.Unisex;
    protected int? MinLevel { get; init; }
    protected int? StatAmountRequired { get; init; }
    protected Stat? StatRequired { get; init; }

    public EquipmentScript(Item subject)
        : base(subject) { }

    public override void OnUse(Aisling source)
    {
        //gender check
        if ((Gender != source.Gender) && (Gender != Gender.Unisex))
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if ((BaseClass != BaseClass.Peasant) && (BaseClass != source.UserStatSheet.BaseClass))
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if ((AdvClass != AdvClass.None) && (AdvClass != source.UserStatSheet.AdvClass))
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if (MinLevel.HasValue && (MinLevel.Value > source.UserStatSheet.Level))
        {
            source.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{Subject.DisplayName} does not seem to fit you, but you could grow into it");

            return;
        }

        if (StatRequired.HasValue
            && StatAmountRequired.HasValue
            && (source.StatSheet.GetBaseStat(StatRequired.Value) < StatAmountRequired.Value))
        {
            source.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{Subject.DisplayName} does not seem to fit you, but you could grow into it");

            return;
        }

        source.Equip(EquipmentType, Subject);
    }
}