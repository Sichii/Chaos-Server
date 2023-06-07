using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using BulkEditTool.Model.Abstractions;

namespace BulkEditTool.Model;

public class ListViewItem<T, TControl> : IListViewItem where TControl: Control
{
    private string _name = null!;

    public TControl? Control { get; set; }

    public required string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public string Path
    {
        get => Wrapper.Path;
        set => Wrapper.Path = value;
    }

    public required TraceWrapper<T> Wrapper { get; init; }

    public event PropertyChangedEventHandler? PropertyChanged;
    public T Object => Wrapper.Object;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetField<TField>(ref TField field, TField value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<TField>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);

        return true;
    }
}