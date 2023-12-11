using ChaosTool.ViewModel.Abstractions;

namespace ChaosTool.ViewModel.Observables;

public class ObservableAbilityRequirement : NotifyPropertyChangedBase
{
    private byte? _level;
    private string _templateKey = string.Empty;

    public byte? Level
    {
        get => _level;
        set => SetField(ref _level, value);
    }

    public string TemplateKey
    {
        get => _templateKey;
        set => SetField(ref _templateKey, value);
    }
}