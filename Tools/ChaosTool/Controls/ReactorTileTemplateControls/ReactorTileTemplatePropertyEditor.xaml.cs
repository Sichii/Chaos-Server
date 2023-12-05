using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Chaos.Extensions.Common;
using Chaos.Schemas.Templates;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Model;

namespace ChaosTool.Controls.ReactorTileTemplateControls;

public sealed partial class ReactorTileTemplatePropertyEditor
{
    public ListViewItem<ReactorTileTemplateSchema, ReactorTileTemplatePropertyEditor> ListItem { get; }
    public ObservableCollection<BindableString> ScriptKeysViewItems { get; }
    public TraceWrapper<ReactorTileTemplateSchema> Wrapper => ListItem.Wrapper;

    public ReactorTileTemplatePropertyEditor(ListViewItem<ReactorTileTemplateSchema, ReactorTileTemplatePropertyEditor> listItem)
    {
        ListItem = listItem;
        ScriptKeysViewItems = new ObservableCollection<BindableString>();

        InitializeComponent();
    }

    #region Tbox Validation
    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
    #endregion

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
        template.ShouldBlockPathfinding = ShouldBlockPathfindingCbox.IsChecked ?? false;

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

        ShouldBlockPathfindingCbox.IsChecked = template.ShouldBlockPathfinding;
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
            var existing = JsonContext.ReactorTileTemplates
                                      .Objects
                                      .Where(wrapper => wrapper != Wrapper)
                                      .FirstOrDefault(wrapper => wrapper.Path.EqualsI(PathTbox.Text));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue($"Save failed. An item already exists at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.ReactorTileTemplates
                                  .Objects
                                  .Where(wrapper => wrapper != Wrapper)
                                  .FirstOrDefault(wrapper => wrapper.Object.TemplateKey.EqualsI(TemplateKeyTbox.Text));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue(
                    $"Save failed. An item already exists with template key \"{existing.Object.TemplateKey}\" at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.ReactorTileTemplates.Objects.FirstOrDefault(obj => ReferenceEquals(obj, Wrapper));

            if (existing is null)
                JsonContext.ReactorTileTemplates.Objects.Add(Wrapper);

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

        await JsonContext.ReactorTileTemplates.SaveItemAsync(Wrapper);
    }
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