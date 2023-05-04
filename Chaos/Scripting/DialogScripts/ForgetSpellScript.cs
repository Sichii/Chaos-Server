using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts;

public class ForgetSpellScript : DialogScriptBase
{
    private readonly ILogger<ForgetSpellScript> Logger;

    /// <inheritdoc />
    public ForgetSpellScript(Dialog subject, ILogger<ForgetSpellScript> logger)
        : base(subject) =>
        Logger = logger;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_forgetspell_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "generic_forgetspell_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.SpellBook.TryGetObject(slot, out var spell))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        source.SpellBook.Remove(slot);
        Logger.LogDebug("{@Player} forgot {@Spell}", source, spell);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.SpellBook.TryGetObject(slot, out var spell))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        Subject.InjectTextParameters(spell.Template.Name);
    }
}