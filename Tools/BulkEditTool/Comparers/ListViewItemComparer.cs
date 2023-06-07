using BulkEditTool.Model.Abstractions;

namespace BulkEditTool.Comparers;

public sealed class ListViewItemComparer : IComparer<IListViewItem>
{
    public static IComparer<IListViewItem> Instance => new ListViewItemComparer();

    public int Compare(IListViewItem? x, IListViewItem? y)
    {
        if (x is null)
            return y is null ? 0 : -1;

        if (y is null)
            return 1;

        return NaturalStringComparer.Instance.Compare(x.Name, y.Name);
    }
}