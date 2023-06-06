using System.ComponentModel;
using BulkEditTool.Model.Abstractions;

namespace BulkEditTool.Model.Observables;

public sealed class ObservableListItem<TControl> : IObservableListItem
    where TControl: class, IPropertyModifier<ObservableListItem<TControl>>
{
    private string _key;

    public string Key
    {
        get => _key;
        set
        {
            _key = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Key)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public TControl Control { get; }

    public ObservableListItem(TControl editor)
    {
        _key = "NEW";
        Control = editor;
        Control.ObservableProperties = this;
    }
}