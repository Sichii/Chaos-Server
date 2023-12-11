using System.Windows;
using System.Windows.Controls;
using Chaos.Common.Definitions;
using Chaos.Schemas.Data;
using Chaos.Schemas.Templates;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Utility;
using ChaosTool.ViewModel;
using ChaosTool.ViewModel.Observables;

namespace ChaosTool.Controls.SpellTemplateControls;

public sealed partial class SpellTemplatePropertyEditor
{
    private SpellTemplateViewModel ViewModel
        => DataContext as SpellTemplateViewModel
           ?? throw new InvalidOperationException($"DataContext is not of type {nameof(SpellTemplateViewModel)}");

    public SpellTemplatePropertyEditor() => InitializeComponent();

    #region Tbox Validation
    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
    #endregion

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        SpellTypeCmbox.ItemsSource = Helpers.GetEnumNames<SpellType>();
        ClassCmbox.ItemsSource = Helpers.GetEnumNames<BaseClass?>();
        AdvClassCmbox.ItemsSource = Helpers.GetEnumNames<AdvClass?>();

        ClassCmbox.ItemsSource = Helpers.GetEnumNames<BaseClass?>();
        AdvClassCmbox.ItemsSource = Helpers.GetEnumNames<AdvClass?>();

        RequiredGoldLbl.ToolTip
            = Helpers.GetPropertyDocs<LearningRequirementsSchema>(nameof(SkillTemplateSchema.LearningRequirements.RequiredGold));
        StrLbl.ToolTip = Helpers.GetPropertyDocs<StatsSchema>(nameof(SkillTemplateSchema.LearningRequirements.RequiredStats.Str));
        IntLbl.ToolTip = Helpers.GetPropertyDocs<StatsSchema>(nameof(SkillTemplateSchema.LearningRequirements.RequiredStats.Int));
        WisLbl.ToolTip = Helpers.GetPropertyDocs<StatsSchema>(nameof(SkillTemplateSchema.LearningRequirements.RequiredStats.Wis));
        ConLbl.ToolTip = Helpers.GetPropertyDocs<StatsSchema>(nameof(SkillTemplateSchema.LearningRequirements.RequiredStats.Con));
        DexLbl.ToolTip = Helpers.GetPropertyDocs<StatsSchema>(nameof(SkillTemplateSchema.LearningRequirements.RequiredStats.Dex));
        CooldownMsLbl.ToolTip = Helpers.GetPropertyDocs<SkillTemplateSchema>(nameof(SkillTemplateSchema.CooldownMs));
        DescriptionLbl.ToolTip = Helpers.GetPropertyDocs<SkillTemplateSchema>(nameof(SkillTemplateSchema.Description));
        LevelsUpLbl.ToolTip = Helpers.GetPropertyDocs<SkillTemplateSchema>(nameof(SkillTemplateSchema.LevelsUp));
        MaxLevelLbl.ToolTip = Helpers.GetPropertyDocs<SkillTemplateSchema>(nameof(SkillTemplateSchema.MaxLevel));

        ItemRequirementsLbl.ToolTip
            = Helpers.GetPropertyDocs<LearningRequirementsSchema>(nameof(SkillTemplateSchema.LearningRequirements.ItemRequirements));

        PrerequisiteSpellsLbl.ToolTip
            = Helpers.GetPropertyDocs<LearningRequirementsSchema>(nameof(SkillTemplateSchema.LearningRequirements.PrerequisiteSpells));

        ScriptKeysLbl.ToolTip = Helpers.GetPropertyDocs<SkillTemplateSchema>(nameof(SkillTemplateSchema.ScriptKeys));

        PrerequisiteSkillsLbl.ToolTip
            = Helpers.GetPropertyDocs<LearningRequirementsSchema>(nameof(SkillTemplateSchema.LearningRequirements.PrerequisiteSkills));
    }

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => ViewModel.RejectChanges();

    private void SaveBtn_Click(object sender, RoutedEventArgs e) => ViewModel.AcceptChanges();

    private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var parentList = this.FindVisualParent<SpellTemplateListView>();

        parentList?.Items.Remove(ViewModel);

        ViewModel.IsDeleted = true;
        ViewModel.AcceptChanges();
    }
    #endregion

    #region ScriptKeys Controls
    private void AddScriptKeyBtn_Click(object sender, RoutedEventArgs e) => ViewModel.ScriptKeys.Add(new BindableString());

    private void DeleteScriptKeyBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        ViewModel.ScriptKeys.Remove(scriptKey);
    }
    #endregion

    #region PrereqSpellTemplateKeys Controls
    private void DeleteSpellTemplateKeyBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not ObservableAbilityRequirement requiredSpell)
            return;

        ViewModel.LearningRequirements.PrerequisiteSpells.Remove(requiredSpell);
    }

    private void AddSpellTemplateKeyBtn_Click(object sender, RoutedEventArgs e)
        => ViewModel.LearningRequirements.PrerequisiteSpells.Add(new ObservableAbilityRequirement());
    #endregion

    #region PrereqSpellTemplateKeys Controls
    private void DeleteSkillTemplateKeyBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not ObservableAbilityRequirement requiredSkill)
            return;

        ViewModel.LearningRequirements.PrerequisiteSkills.Remove(requiredSkill);
    }

    private void AddSkillTemplateKeyBtn_Click(object sender, RoutedEventArgs e)
        => ViewModel.LearningRequirements.PrerequisiteSkills.Add(new ObservableAbilityRequirement());
    #endregion

    #region ItemRequirements Controls
    private void DeleteItemRequirementBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not ObservableItemRequirement itemRequirement)
            return;

        ViewModel.LearningRequirements.ItemRequirements.Remove(itemRequirement);
    }

    private void AddItemRequirementBtn_Click(object sender, RoutedEventArgs e)
        => ViewModel.LearningRequirements.ItemRequirements.Add(new ObservableItemRequirement());
    #endregion
}