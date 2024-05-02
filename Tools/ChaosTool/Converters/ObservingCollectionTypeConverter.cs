using System.ComponentModel;
using AutoMapper;
using Chaos.Wpf.Collections.ObjectModel;

namespace ChaosTool.Converters;

public sealed class
    ObservingCollectionTypeConverter<TSource, TDestination> : ITypeConverter<ObservingCollection<TSource>, ICollection<TDestination>>
    where TSource: INotifyPropertyChanged
{
    /// <inheritdoc />
    public ICollection<TDestination> Convert(
        ObservingCollection<TSource> source,
        ICollection<TDestination>? destination,
        ResolutionContext context)
    {
        destination ??= new List<TDestination>();

        foreach (var sourceItem in source)
        {
            var destItem = context.Mapper.Map<TDestination>(sourceItem);
            destination.Add(destItem);
        }

        return destination;
    }
}

public sealed class
    ReverseObservingCollectionTypeConverter<TSource, TDestination> : ITypeConverter<ICollection<TSource>, ObservingCollection<TDestination>>
    where TDestination: INotifyPropertyChanged
{
    /// <inheritdoc />
    public ObservingCollection<TDestination> Convert(
        ICollection<TSource> source,
        ObservingCollection<TDestination>? destination,
        ResolutionContext context)
    {
        destination ??= [];

        foreach (var sourceItem in source)
        {
            var destItem = context.Mapper.Map<TDestination>(sourceItem);
            destination.Add(destItem);
        }

        return destination;
    }
}