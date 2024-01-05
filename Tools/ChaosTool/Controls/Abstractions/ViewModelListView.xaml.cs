using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ChaosTool.ViewModel.Abstractions;
using MaterialDesignThemes.Wpf;

namespace ChaosTool.Controls.Abstractions;

public abstract partial class ViewModelListView : UserControl
{
    public ObservableCollection<ViewModelBase> Items { get; } = [];
    protected ViewModelListView() => InitializeComponent();

    protected abstract void AddButton_Click(object sender, RoutedEventArgs e);

    protected abstract void PopulateListView();

    private async void UserControl_Initialized(object sender, EventArgs e)
    {
        AddBtn.IsEnabled = false;

        await WaitForJsonContextAsync(Snackbar);

        PopulateListView();
        AddFindBindings(FindTbox);

        AddBtn.IsEnabled = true;
    }

    protected virtual async Task WaitForJsonContextAsync(Snackbar snackbar)
    {
        snackbar.MessageQueue?.Enqueue(
            "Loading...",
            null,
            null,
            null,
            false,
            true,
            TimeSpan.FromHours(1));

        await JsonContext.LoadingTask;

        snackbar.MessageQueue?.Clear();
    }

    #region CTRL+F (Find)
    protected virtual void AddFindBindings(TextBox findTbox)
    {
        var findCommand = new RoutedCommand();
        var escCommand = new RoutedCommand();

        var findInputBinding = new InputBinding(findCommand, new KeyGesture(Key.F, ModifierKeys.Control));
        var escInputBinding = new InputBinding(escCommand, new KeyGesture(Key.Escape));

        var findCmdBinding = new CommandBinding(findCommand, (_, e) => Show_FindTbox(findTbox, e));
        var escCmdBinding = new CommandBinding(escCommand, (_, e) => Hide_FindTBox(findTbox, e));

        InputBindings.Add(findInputBinding);
        InputBindings.Add(escInputBinding);
        CommandBindings.Add(findCmdBinding);
        CommandBindings.Add(escCmdBinding);
    }

    protected virtual void Show_FindTbox(TextBoxBase sender, RoutedEventArgs e)
    {
        e.Handled = true;

        sender.Visibility = Visibility.Visible;
        sender.Focus();
        sender.SelectAll();
    }

    protected virtual void Hide_FindTBox(TextBox sender, RoutedEventArgs e)
    {
        e.Handled = true;

        sender.Text = string.Empty;
        sender.Visibility = Visibility.Collapsed;

        if (ViewModelEditor.Content is UIElement element)
            Keyboard.Focus(element);
    }

    protected abstract void FindTbox_OnTextChanged(object sender, TextChangedEventArgs e);
    #endregion
}