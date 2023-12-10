using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Chaos.Common.Definitions;
using Chaos.Schemas.Templates;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Utility;
using ChaosTool.ViewModel;
using ChaosTool.ViewModel.Observables;

namespace ChaosTool.Controls.DialogTemplateControls;

public partial class DialogTemplatePropertyEditor
{
    private ListViewItem? DraggedItem;
    private Point DragPoint;
    private bool IsDragging;
    private int OriginalIndex;

    private DialogTemplateViewModel ViewModel
        => DataContext as DialogTemplateViewModel
           ?? throw new InvalidOperationException($"DataContext is not of type {nameof(DialogTemplateViewModel)}");

    public DialogTemplatePropertyEditor() { InitializeComponent(); }

    #region Tbox Validation
    private void TemplateKeyTbox_OnTextChanged(object sender, TextChangedEventArgs e)
        => Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
    #endregion

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        //set custom itemssources
        TypeCmbox.ItemsSource = Helpers.GetEnumNames<ChaosDialogType>();

        //tooltips
        TemplateKeyLbl.ToolTip = Helpers.GetPropertyDocs<DialogTemplateSchema>(nameof(DialogTemplateSchema.TemplateKey));
        TypeLbl.ToolTip = Helpers.GetPropertyDocs<DialogTemplateSchema>(nameof(DialogTemplateSchema.Type));
        TextLbl.ToolTip = Helpers.GetPropertyDocs<DialogTemplateSchema>(nameof(DialogTemplateSchema.Text));
        NextDialogKeyLbl.ToolTip = Helpers.GetPropertyDocs<DialogTemplateSchema>(nameof(DialogTemplateSchema.NextDialogKey));
        PrevDialogKeyLbl.ToolTip = Helpers.GetPropertyDocs<DialogTemplateSchema>(nameof(DialogTemplateSchema.PrevDialogKey));
        ContextualLbl.ToolTip = Helpers.GetPropertyDocs<DialogTemplateSchema>(nameof(DialogTemplateSchema.Contextual));
        TextBoxLengthLbl.ToolTip = Helpers.GetPropertyDocs<DialogTemplateSchema>(nameof(DialogTemplateSchema.TextBoxLength));
        TextBoxPromptLbl.ToolTip = Helpers.GetPropertyDocs<DialogTemplateSchema>(nameof(DialogTemplateSchema.TextBoxPrompt));
        OptionsLbl.ToolTip = Helpers.GetPropertyDocs<DialogTemplateSchema>(nameof(DialogTemplateSchema.Options));
        ScriptKeysLbl.ToolTip = Helpers.GetPropertyDocs<DialogTemplateSchema>(nameof(DialogTemplateSchema.ScriptKeys));
    }

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => ViewModel.RejectChanges();

    private void SaveBtn_Click(object sender, RoutedEventArgs e) => ViewModel.AcceptChanges();

    private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var parentList = this.FindVisualParent<DialogTemplateListView>();

        parentList?.Items.Remove(ViewModel);

        ViewModel.IsDeleted = true;
        ViewModel.AcceptChanges();
    }
    #endregion

    #region DialogOptions Controls
    private void DeleteDialogOptionBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not ObservableDialogOption option)
            return;

        ViewModel.Options.Remove(option);
    }

    private void AddDialogOptionBtn_Click(object sender, RoutedEventArgs e)
    {
        var option = new ObservableDialogOption();

        ViewModel.Options.Add(option);
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

    #region Drag Reorder
    private void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if ((e.LeftButton != MouseButtonState.Pressed) || DraggedItem is null || sender is not ListView listView)
        {
            Mouse.SetCursor(Cursors.Arrow);

            return;
        }

        var point = e.GetPosition(listView);
        var diff = DragPoint - point;

        if ((Math.Abs(diff.X) < SystemParameters.MinimumHorizontalDragDistance)
            && (Math.Abs(diff.Y) < SystemParameters.MinimumVerticalDragDistance))
            return;

        IsDragging = true;
        Mouse.SetCursor(Cursors.Hand);

        var closestIndex = GetClosestIndexToPoint(listView, point);
        var aboveIndex = closestIndex - 1;
        var belowIndex = closestIndex;

        if (aboveIndex >= 0)
        {
            var aboveItem = (ListViewItem)listView.ItemContainerGenerator.ContainerFromIndex(aboveIndex);

            if (aboveItem.BorderThickness
                != new Thickness(
                    0,
                    0,
                    0,
                    2))
            {
                ClearBorders(listView);

                aboveItem.BorderBrush = Brushes.LimeGreen;

                aboveItem.BorderThickness = new Thickness(
                    0,
                    0,
                    0,
                    2);
            }
        } else if (belowIndex < listView.Items.Count)
        {
            var belowItem = (ListViewItem)listView.ItemContainerGenerator.ContainerFromIndex(belowIndex);

            if (belowItem.BorderThickness
                != new Thickness(
                    0,
                    2,
                    0,
                    0))
            {
                belowItem.BorderBrush = Brushes.LimeGreen;

                belowItem.BorderThickness = new Thickness(
                    0,
                    2,
                    0,
                    0);
            }
        }
    }

    private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not ListViewItem listViewItem)
            return;

        DragPoint = e.GetPosition(this);
        DraggedItem = listViewItem;
        OriginalIndex = OptionsView.Items.IndexOf(DraggedItem.DataContext);
    }

    private void Editor_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (IsDragging && DraggedItem is not null)
        {
            IsDragging = false;

            var dropPoint = e.GetPosition(this);
            var listView = this.FindVisualElementAtPoint<ListView>(dropPoint);

            if (listView is not null && (listView == OptionsView))
            {
                var relativePoint = TranslatePoint(dropPoint, OptionsView);
                var index = GetClosestIndexToPoint(listView, relativePoint);

                //if the index is below the original index, we need to account for the fact that the item will be removed
                if (index > OriginalIndex)
                    index--;

                if ((index >= 0) && (index < ViewModel.Options.Count))
                    ViewModel.Options.Move(OriginalIndex, index);

                ClearBorders(listView);
            }
        }

        DragPoint = default;
        DraggedItem = null;
        IsDragging = false;
        OriginalIndex = -1;
    }

    private void ClearBorders(ListView listView)
    {
        for (var i = 0; i < listView.Items.Count; i++)
        {
            var item = (ListViewItem)listView.ItemContainerGenerator.ContainerFromIndex(i);

            if (item is null)
                continue;

            item.BorderBrush = Brushes.Transparent;

            item.BorderThickness = new Thickness(
                0,
                1,
                0,
                1);
        }
    }

    private int GetClosestIndexToPoint(ListView listView, Point listRelativePoint)
    {
        var index = -1;
        var shortestDistance = double.MaxValue;
        var bottom = new Point(0, listView.ActualHeight);

        for (var i = 0; i < listView.Items.Count; i++)
        {
            var item = (ListViewItem)listView.ItemContainerGenerator.ContainerFromIndex(i);

            if (item is null)
                continue;

            var topLeft = item.TranslatePoint(new Point(0, 0), listView);
            var distance = (listRelativePoint - topLeft).Length;

            if (shortestDistance > distance)
            {
                shortestDistance = distance;
                index = i;
            }
        }

        var distanceToBottom = (listRelativePoint - bottom).Length;

        if (distanceToBottom < shortestDistance)
            index = listView.Items.Count;

        return index;
    }
    #endregion
}