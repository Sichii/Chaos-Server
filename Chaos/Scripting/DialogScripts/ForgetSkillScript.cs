using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts;

public class ForgetSkillScript : DialogScriptBase
{
    private readonly InputCollector InputCollector;
    private readonly ILogger<ForgetSkillScript> Logger;
    private Skill? SkillToForget;

    /// <inheritdoc />
    public ForgetSkillScript(Dialog subject, ILogger<ForgetSkillScript> logger)
        : base(subject)
    {
        Logger = logger;
        var requestOptionText = DialogString.From(() => $"Are you sure you want to forget {SkillToForget!.Template.Name}?");

        InputCollector = new InputCollectorBuilder()
                         .RequestOptionSelection(requestOptionText, DialogString.Yes, DialogString.No)
                         .HandleInput(ForgetSkill)
                         .Build();
    }

    private bool ForgetSkill(Aisling source, Dialog dialog, int? option = null)
    {
        if (option is 1)
        {
            source.SkillBook.Remove(SkillToForget!.Slot);

            Logger.LogDebug("{@Player} forgot {@Skill}", source, SkillToForget);

            dialog.Reply(source, "The path of learning is endless, come back at any time.");

            return true;
        }

        dialog.NextDialogKey = dialog.Template.TemplateKey;

        return false;
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (SkillToForget == null)
        {
            if (!Subject.MenuArgs.TryGet<byte>(0, out var slot))
                return;

            SkillToForget = source.SkillBook[slot];

            if (SkillToForget == null)
                return;
        }

        InputCollector.Collect(source, Subject, optionIndex);
    }
}