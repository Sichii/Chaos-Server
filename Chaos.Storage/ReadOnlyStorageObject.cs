#region
using System.Collections.Concurrent;
using Chaos.Common.Utilities;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Chaos.Storage;

internal class ReadOnlyStorageObject<T> : IReadOnlyStorage<T> where T: class, new()
{
    protected ConcurrentDictionary<string, T> Data { get; }
    protected LocalStorageManager Manager { get; }
    internal string Name { get; }

    /// <inheritdoc />
    public T Value { get; }

    [ActivatorUtilitiesConstructor]
    public ReadOnlyStorageObject(IStorageManager manager)
    {
        Manager = (LocalStorageManager)manager;
        Data = Manager.GetOrAddEntry<T>();
        Name = "default";
        Value = DeepClone.CreateRequired(Data.GetValueOrDefault(Name, new T()));
    }

    internal ReadOnlyStorageObject(LocalStorageManager manager, ConcurrentDictionary<string, T> data, string name = "default")
    {
        Manager = manager;
        Data = data;
        Name = name;
        Value = DeepClone.CreateRequired(Data.GetValueOrDefault(name, new T()));
    }

    /// <inheritdoc />
    public IReadOnlyStorage<T> GetInstance(string name) => new ReadOnlyStorageObject<T>(Manager, Data, name);
}