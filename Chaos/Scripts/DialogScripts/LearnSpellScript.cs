using System.Text;
using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Factories.Abstractions;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripts.DialogScripts;

public class LearnSpellScript : ConfigurableDialogScriptBase
{
    private readonly InputCollector InputCollector;
    private readonly IItemFactory ItemFactory;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;
    private Spell? SpellToLearn;
    protected List<string> SpellTemplateKeys { get; init; } = null!;

    /// <inheritdoc />
    public LearnSpellScript(
        Dialog subject,
        ISkillFactory skillFactory,
        IItemFactory itemFactory,
        ISpellFactory spellFactory
    )
        : base(subject)
    {
        SkillFactory = skillFactory;
        ItemFactory = itemFactory;
        SpellFactory = spellFactory;

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

        builder.AppendLine(SpellToLearn!.Template.Description);
        builder.AppendLine();

        if (SpellToLearn.Template.LearningRequirements != null)
        {
            builder.AppendLine("Ensure you have what is required.");
            builder.AppendLine();

            var requirementsStrBuilder =
                SpellToLearn.Template.LearningRequirements.BuildRequirementsString(ItemFactory, SkillFactory, SpellFactory);

            builder.Append(requirementsStrBuilder);
            builder.AppendLine();
        }

        builder.AppendLine("Do you wish to learn this spell?");

        return builder.ToString();
    }

    public bool HandleLearningConfirmation(Aisling aisling, Dialog dialog, int? option = null)
    {
        if (option is not 1)
        {
            dialog.Reply(aisling, "Come back when you are ready.");

            return false;
        }

        if (aisling.SkillBook.IsFull)
        {
            dialog.Reply(aisling, "Come back when you have room to grow.");

            return false;
        }

        if (!ValidateAndTakeRequirements(aisling, dialog))
            return false;

        var spell = SpellFactory.Create(SpellToLearn!.Template.TemplateKey);
        aisling.SpellBook.TryAddToNextSlot(spell);

        var animation = new Animation
        {
            AnimationSpeed = 50,
            TargetAnimation = 22,
            TargetId = aisling.Id
        };

        aisling.MapInstance.ShowAnimation(animation);
        dialog.Reply(aisling, "something something secrets for evil idk");

        return true;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        if (Subject.Type == MenuOrDialogType.ShowSpells)
        {
            Subject.Spells.Clear();

            foreach (var spellTemplateKey in SpellTemplateKeys)
            {
                var spell = SpellFactory.CreateFaux(spellTemplateKey);
                var requiredClass = spell.Template.LearningRequirements?.RequiredClass;

                //if this spell is not available to the player's class, skip it
                if (requiredClass.HasValue && (source.UserStatSheet.BaseClass != requiredClass.Value))
                    continue;

                //if the player already knows this spell, skip it
                if (source.SpellBook.Contains(spell))
                    continue;

                Subject.Spells.Add(spell);
            }
        }
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (SpellToLearn == null)
        {
            if (!Subject.MenuArgs.TryGet<string>(0, out var spellName))
                return;

            SpellToLearn = Subject.Spells.FirstOrDefault(spell => spell.Template.Name.EqualsI(spellName));

            if (SpellToLearn == null)
                return;
        }

        InputCollector.Collect(source, Subject, optionIndex);
    }

    public bool ValidateAndTakeRequirements(Aisling aisling, Dialog dialog)
    {
        var requirements = SpellToLearn!.Template.LearningRequirements;

        if (requirements == null)
            return true;

        if (requirements.RequiredLevel.HasValue && (aisling.StatSheet.Level < requirements.RequiredLevel.Value))
        {
            dialog.Reply(aisling, "Come back when you are more experienced.");

            return false;
        }

        if (requirements.RequiredStats != null)
        {
            var requiredStats = requirements.RequiredStats;

            if (requiredStats.Str < aisling.StatSheet.EffectiveStr)
            {
                dialog.Reply(aisling, "Come back when you are stronger.");

                return false;
            }

            if (requiredStats.Int < aisling.StatSheet.EffectiveInt)
            {
                dialog.Reply(aisling, "Come back when you are smarter.");

                return false;
            }

            if (requiredStats.Wis < aisling.StatSheet.EffectiveWis)
            {
                dialog.Reply(aisling, "Come back when you are wiser.");

                return false;
            }

            if (requiredStats.Con < aisling.StatSheet.EffectiveCon)
            {
                dialog.Reply(aisling, "Come back when you are tougher.");

                return false;
            }

            if (requiredStats.Dex < aisling.StatSheet.EffectiveDex)
            {
                dialog.Reply(aisling, "Come back when you are more dexterous.");

                return false;
            }
        }

        foreach (var skillTemplateKey in requirements.PrerequisiteSkillTemplateKeys)
        {
            var requiredSkill = SkillFactory.CreateFaux(skillTemplateKey);

            if (!aisling.SkillBook.Contains(requiredSkill))
            {
                dialog.Reply(aisling, "Come back when you are more skillful.");

                return false;
            }
        }

        foreach (var spellTemplateKey in requirements.PrerequisiteSpellTemplateKeys)
        {
            var requiredSpell = SpellFactory.CreateFaux(spellTemplateKey);

            if (!aisling.SpellBook.Contains(requiredSpell))
            {
                dialog.Reply(aisling, "Come back when you are more knowledgeable.");

                return false;
            }
        }

        foreach (var itemRequirement in requirements.ItemRequirements)
        {
            var requiredItem = ItemFactory.CreateFaux(itemRequirement.ItemTemplateKey);

            if (!aisling.Inventory.HasCount(requiredItem.DisplayName, itemRequirement.AmountRequired))
            {
                dialog.Reply(aisling, "Come back when you have what is required.");

                return false;
            }
        }

        if (requirements.RequiredGold.HasValue && (aisling.Gold < requirements.RequiredGold.Value))
        {
            dialog.Reply(aisling, "Come back when you are more wealthy.");

            return false;
        }

        foreach (var itemRequirement in requirements.ItemRequirements)
        {
            var requiredItem = ItemFactory.CreateFaux(itemRequirement.ItemTemplateKey);
            
            aisling.Inventory.RemoveQuantity(requiredItem.DisplayName, itemRequirement.AmountRequired, out _);
        }

        if (requirements.RequiredGold.HasValue)
            aisling.TryTakeGold(requirements.RequiredGold.Value);

        return true;
    }
}