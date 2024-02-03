using Chaos.Wpf.Abstractions;

namespace ChaosTool.ViewModel.Observables;

public sealed class ObservableItemRequirement : NotifyPropertyChangedBase
{
    private int _amountRequired = 1;
    private string _itemTemplateKey = string.Empty;

    public int AmountRequired
    {
        get => _amountRequired;
        set => SetField(ref _amountRequired, value);
    }

    public string ItemTemplateKey
    {
        get => _itemTemplateKey;
        set => SetField(ref _itemTemplateKey, value);
    }
}