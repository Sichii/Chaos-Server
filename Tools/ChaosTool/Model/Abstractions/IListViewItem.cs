using System.ComponentModel;

namespace ChaosTool.Model.Abstractions;

public interface IListViewItem : INotifyPropertyChanged
{
    string Name { get; set; }
}