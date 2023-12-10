using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ChaosTool.Utility;

public sealed class ObservingCollection<T> : ObservableCollection<T> where T: INotifyPropertyChanged
{
    /// <inheritdoc />
    protected override void ClearItems()
    {
        foreach (var item in this)
            item.PropertyChanged -= OnItemPropertyChanged;

        base.ClearItems();
    }

    /// <inheritdoc />
    protected override void InsertItem(int index, T item)
    {
        item.PropertyChanged += OnItemPropertyChanged;

        base.InsertItem(index, item);
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender));

    /// <inheritdoc />
    protected override void RemoveItem(int index)
    {
        var item = this[index];

        item.PropertyChanged -= OnItemPropertyChanged;

        base.RemoveItem(index);
    }

    /// <inheritdoc />
    protected override void SetItem(int index, T item)
    {
        var oldItem = this.ElementAtOrDefault(index);

        if (oldItem is not null)
            oldItem.PropertyChanged -= OnItemPropertyChanged;

        item.PropertyChanged += OnItemPropertyChanged;

        base.SetItem(index, item);
    }
}