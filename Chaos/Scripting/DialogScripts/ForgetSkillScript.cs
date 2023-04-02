using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts;

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

        source.SkillBook.Remove(slot);
        Logger.LogDebug("{@Player} forgot {@Skill}", source, skill);
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