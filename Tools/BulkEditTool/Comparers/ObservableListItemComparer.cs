using BulkEditTool.Model.Abstractions;

namespace BulkEditTool.Comparers;

public class ObservableListItemComparer : IComparer<IObservableListItem>
{
    public static IComparer<IObservableListItem> Instance { get; } = new ObservableListItemComparer();

    public int Compare(IObservableListItem? x, IObservableListItem? y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        if (ReferenceEquals(null, y))
            return 1;

        if (ReferenceEquals(null, x))
            return -1;

        return NaturalStringComparer.Instance.Compare(x.Key, y.Key);
    }
}