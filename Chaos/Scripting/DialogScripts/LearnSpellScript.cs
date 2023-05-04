using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts;

public class LearnSpellScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<LearnSpellScript> Logger;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;
    private readonly ISpellTeacherSource SpellTeacherSource;

    /// <inheritdoc />
    public LearnSpellScript(
        Dialog subject,
        IItemFactory itemFactory,
        ISkillFactory skillFactory,
        ISpellFactory spellFactory,
        ILogger<LearnSpellScript> logger
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
        Logger = logger;
        SpellTeacherSource = (ISpellTeacherSource)Subject.SourceEntity;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_learnspell_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "generic_learnspell_showrequirements":
            {
                OnDisplayingRequirements(source);

                break;
            }
            case "generic_learnspell_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<string>(out var spellName)
            || !SpellTeacherSource.TryGetSpell(spellName, out var spell)
            || source.SpellBook.Contains(spellName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (!ValidateAndTakeRequirements(source, Subject, spell))
            return;

        var spellToLearn = SpellFactory.Create(spell.Template.TemplateKey);

        var learnSpellResult = ComplexActionHelper.LearnSpell(source, spellToLearn);

        switch (learnSpellResult)
        {
            case ComplexActionHelper.LearnSpellResult.Success:
                Logger.LogDebug("{@Player} learned {@Spell}", source, spell);

                var animation = new Animation
                {
                    AnimationSpeed = 50,
                    TargetAnimation = 22,
                    TargetId = source.Id
                };

                source.MapInstance.ShowAnimation(animation);

                break;
            case ComplexActionHelper.LearnSpellResult.NoRoom:
                Subject.Reply(
                    source,
                    "Like the spilled contents of an unbalanced cup, some knowledge is best forgotten...",
                    "generic_learnspell_initial");

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnDisplayingInitial(Aisling source)
    {
        Subject.Spells = new List<Spell>();

        foreach (var spell in SpellTeacherSource.SpellsToTeach)
        {
            var requiredBaseClass = spell.Template.Class;
            var requiredAdvClass = spell.Template.AdvClass;

            //if this spell is not available to the player's class, skip it
            if (requiredBaseClass.HasValue && !source.HasClass(requiredBaseClass.Value))
                continue;

            //if this spell is not available to the player's adv class, skip it
            if (requiredAdvClass.HasValue && (requiredAdvClass.Value != source.UserStatSheet.AdvClass))
                continue;

            //if the player already knows this spell, skip it
            if (source.SpellBook.Contains(spell))
                continue;

            Subject.Spells.Add(spell);
        }
    }

    private void OnDisplayingRequirements(Aisling source)
    {
        if (!TryFetchArgs<string>(out var spellName)
            || !SpellTeacherSource.TryGetSpell(spellName, out var spell)
            || source.SpellBook.Contains(spellName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var learningRequirementsStr = spell.Template.LearningRequirements?.BuildRequirementsString(ItemFactory, SkillFactory, SpellFactory)
                                           .ToString();

        Subject.InjectTextParameters(spell.Template.Description ?? string.Empty, learningRequirementsStr ?? string.Empty);
    }

    public bool ValidateAndTakeRequirements(Aisling source, Dialog dialog, Spell spellToLearn)
    {
        var template = spellToLearn.Template;
        var requirements = template.LearningRequirements;

        if (requirements == null)
            return true;

        if (template.Class.HasValue && !source.HasClass(template.Class.Value))
        {
            dialog.ReplyToUnknownInput(source);
            Logger.LogWarning("{@Player} tried to learn {@Spell} but is not the correct class", source, template.Name);

            return false;
        }

        if (template.AdvClass.HasValue && (template.AdvClass.Value != source.UserStatSheet.AdvClass))
        {
            dialog.ReplyToUnknownInput(source);
            Logger.LogWarning("{@Player} tried to learn {@Spell} but is not the correct adv class", source, template.Name);

            return false;
        }

        if (source.StatSheet.Level < template.Level)
        {
            dialog.Reply(source, "Come back when you are more experienced.", "generic_learnspell_initial");

            return false;
        }

        if (template.RequiresMaster && !source.UserStatSheet.Master)
        {
            dialog.Reply(source, "Come back when you have mastered your art.", "generic_learnspell_initial");

            return false;
        }

        if (requirements.RequiredStats != null)
        {
            var requiredStats = requirements.RequiredStats;

            if (requiredStats.Str > source.StatSheet.EffectiveStr)
            {
                dialog.Reply(source, "Come back when you are stronger.", "generic_learnspell_initial");

                return false;
            }

            if (requiredStats.Int > source.StatSheet.EffectiveInt)
            {
                dialog.Reply(source, "Come back when you are smarter.", "generic_learnspell_initial");

                return false;
            }

            if (requiredStats.Wis > source.StatSheet.EffectiveWis)
            {
                dialog.Reply(source, "Come back when you are wiser.", "generic_learnspell_initial");

                return false;
            }

            if (requiredStats.Con > source.StatSheet.EffectiveCon)
            {
                dialog.Reply(source, "Come back when you are tougher.", "generic_learnspell_initial");

                return false;
            }

            if (requiredStats.Dex > source.StatSheet.EffectiveDex)
            {
                dialog.Reply(source, "Come back when you are more dexterous.", "generic_learnspell_initial");

                return false;
            }
        }

        foreach (var skillTemplateKey in requirements.PrerequisiteSkillTemplateKeys)
        {
            var requiredSkill = SkillFactory.CreateFaux(skillTemplateKey);

            if (!source.SkillBook.Contains(requiredSkill))
            {
                dialog.Reply(source, "Come back when you are more skillful.", "generic_learnspell_initial");

                return false;
            }
        }

        foreach (var spellTemplateKey in requirements.PrerequisiteSpellTemplateKeys)
        {
            var requiredSpell = SpellFactory.CreateFaux(spellTemplateKey);

            if (!source.SpellBook.Contains(requiredSpell))
            {
                dialog.Reply(source, "Come back when you are more knowledgeable.", "generic_learnspell_initial");

                return false;
            }
        }

        foreach (var itemRequirement in requirements.ItemRequirements)
        {
            var requiredItem = ItemFactory.CreateFaux(itemRequirement.ItemTemplateKey);

            if (!source.Inventory.HasCount(requiredItem.DisplayName, itemRequirement.AmountRequired))
            {
                dialog.Reply(source, "Come back when you have what is required.", "generic_learnspell_initial");

                return false;
            }
        }

        if (requirements.RequiredGold.HasValue && (source.Gold < requirements.RequiredGold.Value))
        {
            dialog.Reply(source, "Come back when you are more wealthy.", "generic_learnspell_initial");

            return false;
        }

        foreach (var itemRequirement in requirements.ItemRequirements)
        {
            var requiredItem = ItemFactory.CreateFaux(itemRequirement.ItemTemplateKey);

            source.Inventory.RemoveQuantity(requiredItem.DisplayName, itemRequirement.AmountRequired);
        }

        if (requirements.RequiredGold.HasValue)
            source.TryTakeGold(requirements.RequiredGold.Value);

        return true;
    }
}