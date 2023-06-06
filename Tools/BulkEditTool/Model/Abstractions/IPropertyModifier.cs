using System.ComponentModel;

namespace BulkEditTool.Model.Abstractions;

public interface IObservableProperties : INotifyPropertyChanged { }

public interface IObservableProperties<TModifier> : IObservableProperties where TModifier: IPropertyModifier<IObservableProperties> { }

public interface IPropertyModifier<TProps> where TProps: IObservableProperties
{
    TProps ObservableProperties { get; set; }
}

public interface IObservableListItem : IObservableProperties
{
    string Key { get; set; }
}