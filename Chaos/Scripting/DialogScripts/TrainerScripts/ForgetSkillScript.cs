using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.TrainerScripts;

public class ForgetSkillScript : DialogScriptBase
{
    private readonly ILogger<ForgetSkillScript> Logger;

    /// <inheritdoc />
    public ForgetSkillScript(Dialog subject, ILogger<ForgetSkillScript> logger)
        : base(subject) =>
        Logger = logger;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_forgetskill_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "generic_forgetskill_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.SkillBook.TryGetObject(slot, out var skill))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (source.SkillBook.Remove(slot))
            Logger.WithProperty(source)
                  .WithProperty(skill)
                  .WithProperty(Subject.DialogSource)
                  .LogDebug("Aisling {@AislingName} forgot skill {@SkillName}", source.Name, skill.Template.Name);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.SkillBook.TryGetObject(slot, out var skill))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        Subject.InjectTextParameters(skill.Template.Name);
    }
}