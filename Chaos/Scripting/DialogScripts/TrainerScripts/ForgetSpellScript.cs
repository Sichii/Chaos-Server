using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.TrainerScripts;

public class ForgetSpellScript : DialogScriptBase
{
    private readonly ILogger<ForgetSpellScript> Logger;

    /// <inheritdoc />
    public ForgetSpellScript(Dialog subject, ILogger<ForgetSpellScript> logger)
        : base(subject)
        => Logger = logger;

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

        if (source.SpellBook.Remove(slot))
            Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Spell, Topics.Actions.Forget)
                  .WithProperty(Subject)
                  .WithProperty(Subject.DialogSource)
                  .WithProperty(source)
                  .WithProperty(spell)
                  .LogInformation("Aisling {@AislingName} forgot spell {@SpellName}", source.Name, spell.Template.Name);
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