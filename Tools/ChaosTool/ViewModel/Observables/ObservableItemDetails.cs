using ChaosTool.ViewModel.Abstractions;

namespace ChaosTool.ViewModel.Observables;

public sealed class ObservableItemDetails : NotifyPropertyChangedBase
{
    private string _itemTemplateKey = string.Empty;
    private int _stock;

    public string ItemTemplateKey
    {
        get => _itemTemplateKey;
        set => SetField(ref _itemTemplateKey, value);
    }

    public int Stock
    {
        get => _stock;
        set => SetField(ref _stock, value);
    }
}