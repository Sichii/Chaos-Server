using System.ComponentModel;

namespace BulkEditTool.Model.Abstractions;

public interface IListViewItem : INotifyPropertyChanged
{
    string Name { get; set; }
}