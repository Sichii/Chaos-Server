using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripts.DialogScripts;

public class ForgetSpellScript : DialogScriptBase
{
    private readonly InputCollector InputCollector;
    private readonly ILogger<ForgetSpellScript> Logger;
    private Spell? SpellToForget;

    /// <inheritdoc />
    public ForgetSpellScript(Dialog subject, ILogger<ForgetSpellScript> logger)
        : base(subject)
    {
        Logger = logger;
        var requestOptionText = DialogString.From(() => $"Are you sure you want to forget {SpellToForget!.Template.Name}?");

        InputCollector = new InputCollectorBuilder()
                         .RequestOptionSelection(requestOptionText, DialogString.Yes, DialogString.No)
                         .HandleInput(ForgetSpell)
                         .Build();
    }

    private bool ForgetSpell(Aisling source, Dialog dialog, int? option = null)
    {
        if (option is 1)
        {
            source.SpellBook.Remove(SpellToForget!.Slot);

            Logger.LogDebug("{Player} forgot spell {Spell}", source, SpellToForget);

            dialog.Reply(source, "The path of learning is endless, come back at any time.");

            return true;
        }

        dialog.NextDialogKey = dialog.Template.TemplateKey;

        return false;
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (SpellToForget == null)
        {
            if (!Subject.MenuArgs.TryGet<byte>(0, out var slot))
                return;

            SpellToForget = source.SpellBook[slot];

            if (SpellToForget == null)
                return;
        }

        InputCollector.Collect(source, Subject, optionIndex);
    }
}