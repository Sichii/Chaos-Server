using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class EquipmentScript : ConfigurableItemScriptBase
{
    public EquipmentScript(Item subject)
        : base(subject) { }

    public override void OnUse(Aisling source)
    {
        var template = Subject.Template;

        if (!source.IsAlive)
        {
            source.SendOrangeBarMessage("You can't do that");

            return;
        }

        if (template.EquipmentType is null or EquipmentType.NotEquipment)
        {
            source.SendOrangeBarMessage("You can't equip that");

            return;
        }

        //gender check
        if (template.Gender.HasValue && !template.Gender.Value.HasFlag(source.Gender))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if (template.Class.HasValue && !source.HasClass(template.Class.Value))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if (template.AdvClass.HasValue && (template.AdvClass.Value != source.UserStatSheet.AdvClass))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if (template.Level > source.UserStatSheet.Level)
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you, but you could grow into it");

            return;
        }

        if (template.RequiresMaster && !source.UserStatSheet.Master)
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

        source.Equip(template.EquipmentType.Value, Subject);
    }

    #region ScriptVars
    protected int? StatAmountRequired { get; init; }
    protected Stat? StatRequired { get; init; }
    #endregion
}