using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Chaos.Extensions.Common;
using Chaos.Schemas.Templates;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Model;

namespace ChaosTool.Controls.MapTemplateControls;

public sealed partial class MapTemplatePropertyEditor
{
    public ListViewItem<MapTemplateSchema, MapTemplatePropertyEditor> ListItem { get; }
    public ObservableCollection<BindableString> ScriptKeysViewItems { get; }
    public TraceWrapper<MapTemplateSchema> Wrapper => ListItem.Wrapper;

    public MapTemplatePropertyEditor(ListViewItem<MapTemplateSchema, MapTemplatePropertyEditor> listItem)
    {
        ListItem = listItem;
        ScriptKeysViewItems = new ObservableCollection<BindableString>();

        InitializeComponent();
    }

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        ScriptKeysView.ItemsSource = ScriptKeysViewItems;

        PopulateControlsFromItem();
    }

    #region Controls > Template > Controls
    public void CopySelectionsToItem()
    {
        var template = Wrapper.Object;

        Wrapper.Path = PathTbox.Text;
        template.TemplateKey = TemplateKeyTbox.Text;
        template.Width = ParsePrimitive<byte>(WidthTbox.Text);
        template.Height = ParsePrimitive<byte>(HeightTbox.Text);

        template.ScriptKeys = ScriptKeysViewItems.ToStrings()
                                                 .ToList();

        ListItem.Name = template.TemplateKey;
    }

    public void PopulateControlsFromItem()
    {
        var template = Wrapper.Object;

        PathTbox.Text = Wrapper.Path;

        TemplateKeyTbox.IsEnabled = false;
        TemplateKeyTbox.Text = template.TemplateKey;
        TemplateKeyTbox.IsEnabled = true;

        WidthTbox.Text = template.Width.ToString();
        HeightTbox.Text = template.Height.ToString();

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
            var existing = JsonContext.MapTemplates
                                      .Objects
                                      .Where(wrapper => wrapper != Wrapper)
                                      .FirstOrDefault(wrapper => wrapper.Path.EqualsI(PathTbox.Text));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue($"Save failed. An item already exists at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.MapTemplates
                                  .Objects
                                  .Where(wrapper => wrapper != Wrapper)
                                  .FirstOrDefault(wrapper => wrapper.Object.TemplateKey.EqualsI(TemplateKeyTbox.Text));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue(
                    $"Save failed. An item already exists with template key \"{existing.Object.TemplateKey}\" at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.MapTemplates.Objects.FirstOrDefault(obj => ReferenceEquals(obj, Wrapper));

            if (existing is null)
                JsonContext.MapTemplates.Objects.Add(Wrapper);

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

        await JsonContext.MapTemplates.SaveItemAsync(Wrapper);
    }
    #endregion

    #region Tbox Validation
    private void TboxNumberValidator(object sender, TextCompositionEventArgs e) => Validators.NumberValidationTextBox(sender, e);

    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
    #endregion

    #region ScriptKeys Controls
    private void AddScriptKeyBtn_Click(object sender, RoutedEventArgs e) => ScriptKeysViewItems.Add(string.Empty);

    private void DeleteScriptKeyBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not BindableString scriptKey)
            return;

        ScriptKeysViewItems.Remove(scriptKey);
    }
    #endregion
}