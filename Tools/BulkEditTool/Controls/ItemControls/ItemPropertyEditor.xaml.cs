using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using BulkEditTool.Model;
using BulkEditTool.Model.Abstractions;
using BulkEditTool.Model.Observables;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Schemas.Aisling;
using Chaos.Schemas.Templates;

namespace BulkEditTool.Controls.ItemControls;

/// <summary>
///     Interaction logic for ItemPropertyEditor.xaml
/// </summary>
public sealed partial class ItemPropertyEditor : IPropertyModifier<ObservableListItem<ItemPropertyEditor>>
{
    private readonly ObservableCollection<ObservableListItem<ItemPropertyEditor>> ListItems;
    /// <inheritdoc />
    public ObservableListItem<ItemPropertyEditor> ObservableProperties { get; set; } = null!;
    public TraceWrapper<ItemTemplateSchema> Item { get; }

    public ItemPropertyEditor(
        ObservableCollection<ObservableListItem<ItemPropertyEditor>> listItems,
        TraceWrapper<ItemTemplateSchema> wrapper
    )
    {
        ListItems = listItems;
        Item = wrapper;

        InitializeComponent();
    }

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        ColorCmbox.ItemsSource = Enum.GetNames<DisplayColor>();
        EquipmentTypeCmbox.ItemsSource = Enum.GetNames<EquipmentType>().Prepend(string.Empty);
        GenderCmbox.ItemsSource = Enum.GetNames<Gender>().Prepend(string.Empty);
        PantsColorCmbox.ItemsSource = Enum.GetNames<DisplayColor>().Take(16).Prepend(string.Empty);
        ClassCmbox.ItemsSource = Enum.GetNames<BaseClass>().Prepend(string.Empty);
        AdvClassCmbox.ItemsSource = Enum.GetNames<BaseClass>().Prepend(string.Empty);

        PopulateControlsFromItem();
    }

    #region Controls > Template > Controls
    public void CopySelectionsToItem()
    {
        var template = Item.Obj;

        Item.Path = PathTbox.Text;
        template.TemplateKey = TemplateKeyTbox.Text;
        template.Name = NameTbox.Text;
        template.PanelSprite = ushort.Parse(PanelSpriteTbox.Text);
        template.DisplaySprite = ushort.TryParse(DisplaySpriteTbox.Text, out var displaySprite) ? displaySprite : default(ushort?);
        template.Color = Enum.Parse<DisplayColor>(ColorCmbox.SelectedItem?.ToString()!);

        template.PantsColor = Enum.TryParse<DisplayColor>(PantsColorCmbox.SelectedItem?.ToString(), out var pantsColor)
            ? pantsColor
            : default(DisplayColor?);

        template.MaxStacks = int.Parse(MaxStacksTbox.Text);
        template.AccountBound = AccountBoundCbox.IsChecked!.Value;
        template.BuyCost = int.Parse(BuyCostTbox.Text);
        template.SellValue = int.Parse(SellValueTbox.Text);
        template.Weight = byte.Parse(WeightTbox.Text);
        template.MaxDurability = int.TryParse(MaxDurabilityTbox.Text, out var maxDurability) ? maxDurability : default(int?);
        template.Class = Enum.TryParse<BaseClass>(ClassCmbox.SelectedItem?.ToString(), out var @class) ? @class : default(BaseClass?);

        template.AdvClass = Enum.TryParse<AdvClass>(AdvClassCmbox.SelectedItem?.ToString(), out var advClass)
            ? advClass
            : default(AdvClass?);

        template.Level = int.Parse(LevelTbox.Text);
        template.RequiresMaster = RequiresMasterCbox.IsChecked!.Value;

        var modifiers = new AttributesSchema
        {
            AtkSpeedPct = int.TryParse(AtkSpeedPctTbox.Text, out var atkSpeedPct) ? atkSpeedPct : default,
            Ac = int.TryParse(AcTbox.Text, out var ac) ? ac : default,
            MagicResistance = int.TryParse(MagicResistanceTbox.Text, out var magicResistance) ? magicResistance : default,
            Hit = int.TryParse(HitTbox.Text, out var hit) ? hit : default,
            Dmg = int.TryParse(DmgTbox.Text, out var dmg) ? dmg : default,
            FlatSkillDamage = int.TryParse(FlatSkillDamageTbox.Text, out var flatSkillDamage) ? flatSkillDamage : default,
            FlatSpellDamage = int.TryParse(FlatSpellDamageTbox.Text, out var flatSpellDamage) ? flatSpellDamage : default,
            SkillDamagePct = int.TryParse(SkillDamagePctTbox.Text, out var skillDamagePct) ? skillDamagePct : default,
            SpellDamagePct = int.TryParse(SpellDamagePctTbox.Text, out var spellDamagePct) ? spellDamagePct : default,
            MaximumHp = int.TryParse(MaximumHpTbox.Text, out var maximumHp) ? maximumHp : default,
            MaximumMp = int.TryParse(MaximumMpTbox.Text, out var maximumMp) ? maximumMp : default,
            Str = int.TryParse(StrTbox.Text, out var str) ? str : default,
            Int = int.TryParse(IntTbox.Text, out var @int) ? @int : default,
            Wis = int.TryParse(WisTbox.Text, out var wis) ? wis : default,
            Con = int.TryParse(ConTbox.Text, out var con) ? con : default,
            Dex = int.TryParse(DexTbox.Text, out var dex) ? dex : default
        };

        var defaultModifiers = new AttributesSchema();

        //set modifiers if any modifier was set
        //or there are existing modifiers(that arent also default) that we need to overwrite
        if (!defaultModifiers.Equals(modifiers) || (template.Modifiers is not null && !defaultModifiers.Equals(template.Modifiers)))
            template.Modifiers = modifiers;

        template.CooldownMs = int.TryParse(CooldownMsTbox.Text, out var cooldownMs) ? cooldownMs : default(int?);

        template.EquipmentType = Enum.TryParse<EquipmentType>(EquipmentTypeCmbox.SelectedItem?.ToString(), out var equipmentType)
            ? equipmentType
            : default(EquipmentType?);

        template.Gender = Enum.TryParse<Gender>(GenderCmbox.SelectedItem?.ToString(), out var gender) ? gender : default(Gender?);
        template.IsDyeable = IsDyeableCbox.IsChecked!.Value;
        template.IsModifiable = IsModifiableCbox.IsChecked!.Value;
        template.Category = CategoryTbox.Text;
        template.Description = DescriptionTbox.Text;

        ObservableProperties.Key = template.TemplateKey;
    }

    public void PopulateControlsFromItem()
    {
        var template = Item.Obj;

        PathTbox.Text = Item.Path;
        TemplateKeyTbox.Text = template.TemplateKey;
        NameTbox.Text = template.Name;
        PanelSpriteTbox.Text = template.PanelSprite.ToString();
        DisplaySpriteTbox.Text = template.DisplaySprite?.ToString();
        ColorCmbox.SelectedItem = ColorCmbox.ItemsSource.OfType<string>().FirstOrDefault(str => str.EqualsI(template.Color.ToString()));

        PantsColorCmbox.SelectedItem = PantsColorCmbox.ItemsSource.OfType<string>()
                                                      .FirstOrDefault(str => str.EqualsI(template.PantsColor?.ToString() ?? string.Empty));

        MaxStacksTbox.Text = template.MaxStacks.ToString();
        AccountBoundCbox.IsChecked = template.AccountBound;
        BuyCostTbox.Text = template.BuyCost.ToString();
        SellValueTbox.Text = template.SellValue.ToString();
        WeightTbox.Text = template.Weight.ToString();
        MaxDurabilityTbox.Text = template.MaxDurability?.ToString();

        ClassCmbox.SelectedItem = ClassCmbox.ItemsSource.OfType<string>()
                                            .FirstOrDefault(str => str.EqualsI(template.Class?.ToString() ?? string.Empty));

        AdvClassCmbox.SelectedItem = AdvClassCmbox.ItemsSource.OfType<string>()
                                                  .FirstOrDefault(str => str.EqualsI(template.AdvClass?.ToString() ?? string.Empty));

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

        EquipmentTypeCmbox.SelectedItem = EquipmentTypeCmbox.ItemsSource.OfType<string>()
                                                            .FirstOrDefault(
                                                                str => str.EqualsI(template.EquipmentType?.ToString() ?? string.Empty));

        GenderCmbox.SelectedItem = GenderCmbox.ItemsSource.OfType<string>()
                                              .FirstOrDefault(str => str.EqualsI(template.Gender?.ToString() ?? string.Empty));

        IsDyeableCbox.IsChecked = template.IsDyeable;
        IsModifiableCbox.IsChecked = template.IsModifiable;
        CategoryTbox.Text = template.Category;
        DescriptionTbox.Text = template.Description;
    }
    #endregion

    #region Buttons
    private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        JsonContext.ItemTemplates.Remove(Item.Obj.TemplateKey);
        ListItems.Remove(ObservableProperties);
    }

    private void RevertButton_Click(object sender, RoutedEventArgs e) => PopulateControlsFromItem();

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var existing = JsonContext.ItemTemplates.Objects.Where(obj => !ReferenceEquals(obj, Item))
                                      .FirstOrDefault(obj => obj.Path == Item.Path);

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue($"Save failed. An item already exists at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.ItemTemplates.Objects.Where(obj => !ReferenceEquals(obj, Item))
                                  .FirstOrDefault(obj => obj.Obj.TemplateKey.EqualsI(Item.Obj.TemplateKey));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue(
                    $"Save failed. An item already exists with template key \"{existing.Obj.TemplateKey}\" at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.ItemTemplates.Objects.FirstOrDefault(obj => ReferenceEquals(obj, Item));

            if (existing is null)
                JsonContext.ItemTemplates.Objects.Add(Item);

            if (!ValidatePreSave())
                return;

            CopySelectionsToItem();
            PopulateControlsFromItem();
        } catch (Exception ex)
        {
            Snackbar.MessageQueue?.Enqueue(ex.ToString());
        }

        await JsonContext.ItemTemplates.SaveItemAsync(Item);
    }

    private bool ValidatePreSave()
    {
        var fileName = Path.GetFileNameWithoutExtension(PathTbox.Text);

        if (!fileName.EqualsI(TemplateKeyTbox.Text))
        {
            Snackbar.MessageQueue?.Enqueue("Filename does not match the template key");

            return false;
        }

        if (!Item.Path.EqualsI(PathTbox.Text))
            if (File.Exists(Item.Path))
                File.Delete(Item.Path);

        return true;
    }
    #endregion

    #region Tbox Validation
    private void TboxNumberValidator(object sender, TextCompositionEventArgs e) => Validators.NumberValidationTextBox(sender, e);
    private void TemplateKeyTbox_OnKeyUp(object sender, KeyEventArgs e) => Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
    #endregion
}