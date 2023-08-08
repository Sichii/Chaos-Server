using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.TrainerScripts;

public class LearnSkillScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<LearnSkillScript> Logger;
    private readonly ISkillFactory SkillFactory;
    private readonly ISkillTeacherSource SkillTeacherSource;
    private readonly ISpellFactory SpellFactory;

    /// <inheritdoc />
    public LearnSkillScript(
        Dialog subject,
        IItemFactory itemFactory,
        ISkillFactory skillFactory,
        ISpellFactory spellFactory,
        ILogger<LearnSkillScript> logger
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
        Logger = logger;
        SkillTeacherSource = (ISkillTeacherSource)Subject.DialogSource;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_learnskill_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "generic_learnskill_showrequirements":
            {
                OnDisplayingRequirements(source);

                break;
            }
            case "generic_learnskill_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<string>(out var skillName)
            || !SkillTeacherSource.TryGetSkill(skillName, out var skill)
            || source.SkillBook.Contains(skillName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (!ValidateAndTakeRequirements(source, Subject, skill))
            return;

        var skillToLearn = SkillFactory.Create(skill.Template.TemplateKey);

        var learnSkillResult = ComplexActionHelper.LearnSkill(source, skillToLearn);

        switch (learnSkillResult)
        {
            case ComplexActionHelper.LearnSkillResult.Success:
                Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Skill, Topics.Actions.Learn)
                      .WithProperty(Subject)
                      .WithProperty(Subject.DialogSource)
                      .WithProperty(source)
                      .WithProperty(skill)
                      .LogInformation("Aisling {@AislingName} learned skill {@SkillName}", source.Name, skill.Template.Name);

                var animation = new Animation
                {
                    AnimationSpeed = 50,
                    TargetAnimation = 22
                };

                source.Animate(animation);

                break;
            case ComplexActionHelper.LearnSkillResult.NoRoom:
                Subject.Reply(
                    source,
                    "Like the spilled contents of an unbalanced cup, some knowledge is best forgotten...",
                    "generic_learnskill_initial");

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnDisplayingInitial(Aisling source)
    {
        Subject.Skills = new List<Skill>();

        foreach (var skill in SkillTeacherSource.SkillsToTeach)
        {
            var requiredBaseClass = skill.Template.Class;
            var requiredAdvClass = skill.Template.AdvClass;

            //if this skill is not available to the player's class, skip it
            if (requiredBaseClass.HasValue && !source.HasClass(requiredBaseClass.Value))
                continue;

            //if this skill is not available to the player's adv class, skip it
            if (requiredAdvClass.HasValue && (requiredAdvClass.Value != source.UserStatSheet.AdvClass))
                continue;

            //if the player already knows this skill, skip it
            if (source.SkillBook.Contains(skill))
                continue;

            Subject.Skills.Add(skill);
        }
    }

    private void OnDisplayingRequirements(Aisling source)
    {
        if (!TryFetchArgs<string>(out var skillName)
            || !SkillTeacherSource.TryGetSkill(skillName, out var skill)
            || source.SkillBook.Contains(skillName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var learningRequirementsStr = skill.Template.LearningRequirements?.BuildRequirementsString(ItemFactory, SkillFactory, SpellFactory)
                                           .ToString();

        Subject.InjectTextParameters(skill.Template.Description ?? string.Empty, learningRequirementsStr ?? string.Empty);
    }

    public bool ValidateAndTakeRequirements(Aisling source, Dialog dialog, Skill skillToLearn)
    {
        var template = skillToLearn.Template;
        var requirements = template.LearningRequirements;

        if (requirements == null)
            return true;

        if (template.Class.HasValue && !source.HasClass(template.Class.Value))
        {
            dialog.ReplyToUnknownInput(source);

            Logger.WithTopics(
                      Topics.Entities.Aisling,
                      Topics.Entities.Skill,
                      Topics.Actions.Learn,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(Subject)
                  .WithProperty(Subject.DialogSource)
                  .WithProperty(source)
                  .WithProperty(skillToLearn)
                  .LogWarning(
                      "Aisling {@AislingName} tried to learn skill {@SkillName} but is not the correct class (possibly packeting)",
                      source.Name,
                      template.Name);

            return false;
        }

        if (template.AdvClass.HasValue && (template.AdvClass.Value != source.UserStatSheet.AdvClass))
        {
            dialog.ReplyToUnknownInput(source);

            Logger.WithTopics(
                      Topics.Entities.Aisling,
                      Topics.Entities.Skill,
                      Topics.Actions.Learn,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(Subject)
                  .WithProperty(Subject.DialogSource)
                  .WithProperty(source)
                  .WithProperty(skillToLearn)
                  .LogWarning(
                      "Aisling {@AislingName} tried to learn skill {@SkillName} but is not the correct adv class (possibly packeting)",
                      source.Name,
                      template.Name);

            return false;
        }

        if (source.StatSheet.Level < template.Level)
        {
            dialog.Reply(source, "Come back when you are more experienced.", "generic_learnskill_initial");

            return false;
        }

        if (template.RequiresMaster && !source.UserStatSheet.Master)
        {
            dialog.Reply(source, "Come back when you have mastered your art.", "generic_learnskill_initial");

            return false;
        }

        if (requirements.RequiredStats != null)
        {
            var requiredStats = requirements.RequiredStats;

            if (requiredStats.Str > source.StatSheet.EffectiveStr)
            {
                dialog.Reply(source, "Come back when you are stronger.", "generic_learnskill_initial");

                return false;
            }

            if (requiredStats.Int > source.StatSheet.EffectiveInt)
            {
                dialog.Reply(source, "Come back when you are smarter.", "generic_learnskill_initial");

                return false;
            }

            if (requiredStats.Wis > source.StatSheet.EffectiveWis)
            {
                dialog.Reply(source, "Come back when you are wiser.", "generic_learnskill_initial");

                return false;
            }

            if (requiredStats.Con > source.StatSheet.EffectiveCon)
            {
                dialog.Reply(source, "Come back when you are tougher.", "generic_learnskill_initial");

                return false;
            }

            if (requiredStats.Dex > source.StatSheet.EffectiveDex)
            {
                dialog.Reply(source, "Come back when you are more dexterous.", "generic_learnskill_initial");

                return false;
            }
        }

        foreach (var skillTemplateKey in requirements.PrerequisiteSkillTemplateKeys)
        {
            var requiredSkill = SkillFactory.CreateFaux(skillTemplateKey);

            if (!source.SkillBook.Contains(requiredSkill))
            {
                dialog.Reply(source, "Come back when you are more skillful.", "generic_learnskill_initial");

                return false;
            }
        }

        foreach (var spellTemplateKey in requirements.PrerequisiteSpellTemplateKeys)
        {
            var requiredSpell = SpellFactory.CreateFaux(spellTemplateKey);

            if (!source.SpellBook.Contains(requiredSpell))
            {
                dialog.Reply(source, "Come back when you are more knowledgeable.", "generic_learnskill_initial");

                return false;
            }
        }

        foreach (var itemRequirement in requirements.ItemRequirements)
        {
            var requiredItem = ItemFactory.CreateFaux(itemRequirement.ItemTemplateKey);

            if (!source.Inventory.HasCount(requiredItem.DisplayName, itemRequirement.AmountRequired))
            {
                dialog.Reply(source, "Come back when you have what is required.", "generic_learnskill_initial");

                return false;
            }
        }

        if (requirements.RequiredGold.HasValue && (source.Gold < requirements.RequiredGold.Value))
        {
            dialog.Reply(source, "Come back when you are more wealthy.", "generic_learnskill_initial");

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