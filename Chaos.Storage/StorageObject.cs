#region
using System.Collections.Concurrent;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Chaos.Storage;

internal sealed class StorageObject<T> : ReadOnlyStorageObject<T>, IStorage<T> where T: class, new()
{
    [ActivatorUtilitiesConstructor]
    public StorageObject(IStorageManager manager)
        : base(manager) { }

    internal StorageObject(LocalStorageManager manager, ConcurrentDictionary<string, T> data, string name = "default")
        : base(manager, data, name) { }

    /// <inheritdoc />
    IReadOnlyStorage<T> IReadOnlyStorage<T>.GetInstance(string name) => new ReadOnlyStorageObject<T>(Manager, Data, name);

    /// <inheritdoc />
    public new IStorage<T> GetInstance(string name) => new StorageObject<T>(Manager, Data, name);

    /// <inheritdoc />
    public void Save() => Manager.Save(this);

    public Task SaveAsync() => Manager.SaveAsync(this);
}