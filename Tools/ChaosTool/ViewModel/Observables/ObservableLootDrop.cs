using Chaos.Wpf.Abstractions;

namespace ChaosTool.ViewModel.Observables;

public sealed class ObservableLootDrop : NotifyPropertyChangedBase
{
    private decimal _dropChance;
    private string _itemTemplateKey = string.Empty;

    public decimal DropChance
    {
        get => _dropChance;
        set => SetField(ref _dropChance, value);
    }

    public string ItemTemplateKey
    {
        get => _itemTemplateKey;
        set => SetField(ref _itemTemplateKey, value);
    }
}