using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Schemas.Aisling;
using Chaos.Schemas.Templates;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Model;

namespace ChaosTool.Controls.ItemTemplateControls;

/// <summary>
///     Interaction logic for ItemTemplatePropertyEditor.xaml
/// </summary>
public sealed partial class ItemTemplatePropertyEditor
{
    public ListViewItem<ItemTemplateSchema, ItemTemplatePropertyEditor> ListItem { get; }
    public ObservableCollection<BindableString> ScriptKeysViewItems { get; }
    public TraceWrapper<ItemTemplateSchema> Wrapper => ListItem.Wrapper;

    public ItemTemplatePropertyEditor(ListViewItem<ItemTemplateSchema, ItemTemplatePropertyEditor> listItem)
    {
        ListItem = listItem;
        ScriptKeysViewItems = new ObservableCollection<BindableString>();

        InitializeComponent();
    }

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        ColorCmbox.ItemsSource = GetEnumNames<DisplayColor>();
        EquipmentTypeCmbox.ItemsSource = GetEnumNames<EquipmentType?>();
        GenderCmbox.ItemsSource = GetEnumNames<Gender?>();
        PantsColorCmbox.ItemsSource = GetEnumNames<DisplayColor?>().Take(17); //16 color + empty
        ClassCmbox.ItemsSource = GetEnumNames<BaseClass?>();
        AdvClassCmbox.ItemsSource = GetEnumNames<AdvClass?>();

        ScriptKeysView.ItemsSource = ScriptKeysViewItems;

        PopulateControlsFromItem();
    }

    #region Controls > Template > Controls
    public void CopySelectionsToItem()
    {
        var template = Wrapper.Object;

        Wrapper.Path = PathTbox.Text;
        template.TemplateKey = TemplateKeyTbox.Text;
        template.Name = NameTbox.Text;
        template.PanelSprite = ParsePrimitive<ushort>(PanelSpriteTbox.Text);
        template.DisplaySprite = ParsePrimitive<ushort?>(DisplaySpriteTbox.Text);
        template.Color = ParsePrimitive<DisplayColor>(ColorCmbox.Text);
        template.PantsColor = ParsePrimitive<DisplayColor?>(PantsColorCmbox.Text);
        template.MaxStacks = ParsePrimitive<int>(MaxStacksTbox.Text);
        template.AccountBound = AccountBoundCbox.IsChecked!.Value;
        template.BuyCost = ParsePrimitive<int>(BuyCostTbox.Text);
        template.SellValue = ParsePrimitive<int>(SellValueTbox.Text);
        template.Weight = ParsePrimitive<byte>(WeightTbox.Text);
        template.MaxDurability = ParsePrimitive<int?>(MaxDurabilityTbox.Text);
        template.Class = ParsePrimitive<BaseClass?>(ClassCmbox.Text);
        template.AdvClass = ParsePrimitive<AdvClass?>(AdvClassCmbox.Text);
        template.Level = ParsePrimitive<int>(LevelTbox.Text);
        template.RequiresMaster = RequiresMasterCbox.IsChecked!.Value;

        var modifiers = new AttributesSchema
        {
            AtkSpeedPct = ParsePrimitive<int>(AtkSpeedPctTbox.Text),
            Ac = ParsePrimitive<int>(AcTbox.Text),
            MagicResistance = ParsePrimitive<int>(MagicResistanceTbox.Text),
            Hit = ParsePrimitive<int>(HitTbox.Text),
            Dmg = ParsePrimitive<int>(DmgTbox.Text),
            FlatSkillDamage = ParsePrimitive<int>(FlatSkillDamageTbox.Text),
            FlatSpellDamage = ParsePrimitive<int>(FlatSpellDamageTbox.Text),
            SkillDamagePct = ParsePrimitive<int>(SkillDamagePctTbox.Text),
            SpellDamagePct = ParsePrimitive<int>(SpellDamagePctTbox.Text),
            MaximumHp = ParsePrimitive<int>(MaximumHpTbox.Text),
            MaximumMp = ParsePrimitive<int>(MaximumMpTbox.Text),
            Str = ParsePrimitive<int>(StrTbox.Text),
            Int = ParsePrimitive<int>(IntTbox.Text),
            Wis = ParsePrimitive<int>(WisTbox.Text),
            Con = ParsePrimitive<int>(ConTbox.Text),
            Dex = ParsePrimitive<int>(DexTbox.Text)
        };

        var defaultModifiers = new AttributesSchema();

        //set modifiers if any modifier was set
        //or there are existing modifiers(that arent also default) that we need to overwrite
        if (!defaultModifiers.Equals(modifiers) || (template.Modifiers is not null && !defaultModifiers.Equals(template.Modifiers)))
            template.Modifiers = modifiers;

        template.CooldownMs = ParsePrimitive<int?>(CooldownMsTbox.Text);
        template.EquipmentType = ParsePrimitive<EquipmentType?>(EquipmentTypeCmbox.Text);
        template.Gender = ParsePrimitive<Gender?>(GenderCmbox.Text);
        template.IsDyeable = IsDyeableCbox.IsChecked!.Value;
        template.IsModifiable = IsModifiableCbox.IsChecked!.Value;
        template.Category = CategoryTbox.Text;
        template.Description = DescriptionTbox.Text;
        template.ScriptKeys = ScriptKeysViewItems.ToStrings().ToList();

        ListItem.Name = template.TemplateKey;
    }

    public void PopulateControlsFromItem()
    {
        var template = Wrapper.Object;

        PathTbox.Text = Wrapper.Path;

        TemplateKeyTbox.IsEnabled = false;
        TemplateKeyTbox.Text = template.TemplateKey;
        TemplateKeyTbox.IsEnabled = true;

        NameTbox.Text = template.Name;
        PanelSpriteTbox.Text = template.PanelSprite.ToString();
        DisplaySpriteTbox.Text = template.DisplaySprite?.ToString();
        ColorCmbox.SelectedItem = SelectPrimitive(template.Color, ColorCmbox.ItemsSource);
        PantsColorCmbox.SelectedItem = SelectPrimitive(template.PantsColor, PantsColorCmbox.ItemsSource);
        MaxStacksTbox.Text = template.MaxStacks.ToString();
        AccountBoundCbox.IsChecked = template.AccountBound;
        BuyCostTbox.Text = template.BuyCost.ToString();
        SellValueTbox.Text = template.SellValue.ToString();
        WeightTbox.Text = template.Weight.ToString();
        MaxDurabilityTbox.Text = template.MaxDurability?.ToString();
        ClassCmbox.SelectedItem = SelectPrimitive(template.Class, ClassCmbox.ItemsSource);
        AdvClassCmbox.SelectedItem = SelectPrimitive(template.AdvClass, AdvClassCmbox.ItemsSource);
        LevelTbox.Text = template.Level.ToString();
        RequiresMasterCbox.IsChecked = template.RequiresMaster;

        var modifiers = template.Modifiers;

        if (modifiers is not null)
        {
            AtkSpeedPctTbox.Text = modifiers.AtkSpeedPct.ToString();
            AcTbox.Text = modifiers.Ac.ToString();
            MagicResistanceTbox.Text = modifiers.MagicResistance.ToString();
            HitTbox.Text = modifiers.Hit.ToString();
            DmgTbox.Text = modifiers.Dmg.ToString();
            FlatSkillDamageTbox.Text = modifiers.FlatSkillDamage.ToString();
            FlatSpellDamageTbox.Text = modifiers.FlatSpellDamage.ToString();
            SkillDamagePctTbox.Text = modifiers.SkillDamagePct.ToString();
            SpellDamagePctTbox.Text = modifiers.SpellDamagePct.ToString();
            MaximumHpTbox.Text = modifiers.MaximumHp.ToString();
            MaximumMpTbox.Text = modifiers.MaximumMp.ToString();
            StrTbox.Text = modifiers.Str.ToString();
            IntTbox.Text = modifiers.Int.ToString();
            WisTbox.Text = modifiers.Wis.ToString();
            ConTbox.Text = modifiers.Con.ToString();
            DexTbox.Text = modifiers.Dex.ToString();
        }

        CooldownMsTbox.Text = template.CooldownMs?.ToString();
        EquipmentTypeCmbox.SelectedItem = SelectPrimitive(template.EquipmentType, EquipmentTypeCmbox.ItemsSource);
        GenderCmbox.SelectedItem = SelectPrimitive(template.Gender, GenderCmbox.ItemsSource);
        IsDyeableCbox.IsChecked = template.IsDyeable;
        IsModifiableCbox.IsChecked = template.IsModifiable;
        CategoryTbox.Text = template.Category;
        DescriptionTbox.Text = template.Description;

        ScriptKeysViewItems.Clear();
        ScriptKeysViewItems.AddRange(template.ScriptKeys.ToBindableStrings());
    }
    #endregion

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => PopulateControlsFromItem();

    private async void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var existing = JsonContext.ItemTemplates.Objects.Where(wrapper => wrapper != Wrapper)
                                      .FirstOrDefault(wrapper => wrapper.Path.EqualsI(PathTbox.Text));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue($"Save failed. An item already exists at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.ItemTemplates.Objects.Where(wrapper => !ReferenceEquals(wrapper, Wrapper))
                                  .FirstOrDefault(wrapper => wrapper.Object.TemplateKey.EqualsI(TemplateKeyTbox.Text));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue(
                    $"Save failed. An item already exists with template key \"{existing.Object.TemplateKey}\" at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.ItemTemplates.Objects.FirstOrDefault(obj => ReferenceEquals(obj, Wrapper));

            if (existing is null)
                JsonContext.ItemTemplates.Objects.Add(Wrapper);

            if (!ValidatePreSave(Wrapper, PathTbox, TemplateKeyTbox))
            {
                Snackbar.MessageQueue?.Enqueue("Filename does not match the template key");

                return;
            }

            CopySelectionsToItem();
            PopulateControlsFromItem();
        } catch (Exception ex)
        {
            Snackbar.MessageQueue?.Enqueue(ex.ToString());
        }

        await JsonContext.ItemTemplates.SaveItemAsync(Wrapper);
    }
    #endregion

    #region Tbox Validation
    private void TboxNumberValidator(object sender, TextCompositionEventArgs e) => Validators.NumberValidationTextBox(sender, e);

    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e) =>
        Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
    #endregion

    #region ScriptKeys Control
    private void DeleteScriptKeyBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        ScriptKeysViewItems.Remove(scriptKey);
    }

    private void AddScriptKeyBtn_Click(object sender, RoutedEventArgs e) => ScriptKeysViewItems.Add(string.Empty);
    #endregion
}