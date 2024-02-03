using Chaos.Wpf.Abstractions;

namespace ChaosTool.ViewModel.Observables;

public sealed class ObservableDialogOption : NotifyPropertyChangedBase
{
    private string _dialogKey = null!;
    private string _optionOptionText = null!;

    public string DialogKey
    {
        get => _dialogKey;
        set => SetField(ref _dialogKey, value);
    }

    public string OptionText
    {
        get => _optionOptionText;
        set => SetField(ref _optionOptionText, value);
    }
}