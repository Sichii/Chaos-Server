using System.Text;
using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts;

public class LearnSkillScript : ConfigurableDialogScriptBase
{
    private readonly InputCollector InputCollector;
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<LearnSkillScript> Logger;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;
    private Skill? SkillToLearn;
    protected List<string> SkillTemplateKeys { get; init; } = null!;

    /// <inheritdoc />
    public LearnSkillScript(
        Dialog subject,
        ISkillFactory skillFactory,
        IItemFactory itemFactory,
        ISpellFactory spellFactory,
        ILogger<LearnSkillScript> logger
    )
        : base(subject)
    {
        SkillFactory = skillFactory;
        ItemFactory = itemFactory;
        SpellFactory = spellFactory;
        Logger = logger;

        InputCollector = new InputCollectorBuilder()
                         .RequestOptionSelection(
                             DialogString.From(DisplayRequirementsAndAskForConfirmation),
                             DialogString.Yes,
                             DialogString.No)
                         .HandleInput(HandleLearningConfirmation)
                         .Build();
    }

    private string DisplayRequirementsAndAskForConfirmation()
    {
        var builder = new StringBuilder();

        builder.AppendLine(SkillToLearn!.Template.Description);
        builder.AppendLine();

        if (SkillToLearn.Template.LearningRequirements != null)
        {
            builder.AppendLine("Ensure you have what is required.");
            builder.AppendLine();

            var requirementsStrBuilder =
                SkillToLearn.Template.LearningRequirements.BuildRequirementsString(ItemFactory, SkillFactory, SpellFactory);

            builder.Append(requirementsStrBuilder);
            builder.AppendLine();
        }

        builder.AppendLine("Do you wish to learn this skill?");

        return builder.ToString();
    }

    public bool HandleLearningConfirmation(Aisling source, Dialog dialog, int? option = null)
    {
        if (option is not 1)
        {
            dialog.Reply(source, "Come back when you are ready.");

            return false;
        }

        if (source.SkillBook.IsFull)
        {
            dialog.Reply(source, "Come back when you have room to grow.");

            return false;
        }

        if (!ValidateAndTakeRequirements(source, dialog))
            return false;

        var skill = SkillFactory.Create(SkillToLearn!.Template.TemplateKey);
        source.SkillBook.TryAddToNextSlot(skill);

        Logger.LogDebug("{@Player} learned {@Skill}", source, skill);

        var animation = new Animation
        {
            AnimationSpeed = 50,
            TargetAnimation = 22,
            TargetId = source.Id
        };

        source.MapInstance.ShowAnimation(animation);
        dialog.Reply(source, "something something secrets for evil idk");

        return true;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        if (Subject.Type == MenuOrDialogType.ShowSkills)
        {
            Subject.Skills.Clear();

            foreach (var skillTemplateKey in SkillTemplateKeys)
            {
                var skill = SkillFactory.CreateFaux(skillTemplateKey);
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
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (SkillToLearn == null)
        {
            if (!Subject.MenuArgs.TryGet<string>(0, out var skillName))
                return;

            SkillToLearn = Subject.Skills.FirstOrDefault(skill => skill.Template.Name.EqualsI(skillName));

            if (SkillToLearn == null)
                return;
        }

        InputCollector.Collect(source, Subject, optionIndex);
    }

    public bool ValidateAndTakeRequirements(Aisling source, Dialog dialog)
    {
        var template = SkillToLearn!.Template;
        var requirements = template.LearningRequirements;

        if (requirements == null)
            return true;

        if (template.Class.HasValue && !source.HasClass(template.Class.Value))
        {
            dialog.Reply(source, "Heh, nice try kid...");
            Logger.LogWarning("{@Client} tried to learn {@Skill} but is not the correct class", source.Client, template.Name);

            return false;
        }

        if (template.AdvClass.HasValue && (template.AdvClass.Value != source.UserStatSheet.AdvClass))
        {
            dialog.Reply(source, "Heh, nice try kid...");
            Logger.LogWarning("{@Client} tried to learn {@Skill} but is not the correct adv class", source.Client, template.Name);

            return false;
        }

        if (source.StatSheet.Level < template.Level)
        {
            dialog.Reply(source, "Come back when you are more experienced.");

            return false;
        }

        if (template.RequiresMaster && !source.UserStatSheet.Master)
        {
            dialog.Reply(source, "Come back when you have mastered your art.");

            return false;
        }

        if (requirements.RequiredStats != null)
        {
            var requiredStats = requirements.RequiredStats;

            if (requiredStats.Str > source.StatSheet.EffectiveStr)
            {
                dialog.Reply(source, "Come back when you are stronger.");

                return false;
            }

            if (requiredStats.Int > source.StatSheet.EffectiveInt)
            {
                dialog.Reply(source, "Come back when you are smarter.");

                return false;
            }

            if (requiredStats.Wis > source.StatSheet.EffectiveWis)
            {
                dialog.Reply(source, "Come back when you are wiser.");

                return false;
            }

            if (requiredStats.Con > source.StatSheet.EffectiveCon)
            {
                dialog.Reply(source, "Come back when you are tougher.");

                return false;
            }

            if (requiredStats.Dex > source.StatSheet.EffectiveDex)
            {
                dialog.Reply(source, "Come back when you are more dexterous.");

                return false;
            }
        }

        foreach (var skillTemplateKey in requirements.PrerequisiteSkillTemplateKeys)
        {
            var requiredSkill = SkillFactory.CreateFaux(skillTemplateKey);

            if (!source.SkillBook.Contains(requiredSkill))
            {
                dialog.Reply(source, "Come back when you are more skillful.");

                return false;
            }
        }

        foreach (var spellTemplateKey in requirements.PrerequisiteSpellTemplateKeys)
        {
            var requiredSpell = SpellFactory.CreateFaux(spellTemplateKey);

            if (!source.SpellBook.Contains(requiredSpell))
            {
                dialog.Reply(source, "Come back when you are more knowledgeable.");

                return false;
            }
        }

        foreach (var itemRequirement in requirements.ItemRequirements)
        {
            var requiredItem = ItemFactory.CreateFaux(itemRequirement.ItemTemplateKey);

            if (!source.Inventory.HasCount(requiredItem.DisplayName, itemRequirement.AmountRequired))
            {
                dialog.Reply(source, "Come back when you have what is required.");

                return false;
            }
        }

        if (requirements.RequiredGold.HasValue && (source.Gold < requirements.RequiredGold.Value))
        {
            dialog.Reply(source, "Come back when you are more wealthy.");

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