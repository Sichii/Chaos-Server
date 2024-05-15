using System.Numerics;
using System.Text.Json;
using Chaos.Common.Abstractions;
using Chaos.IO.FileSystem;

namespace Chaos.Common.Identity;

/// <summary>
///     Represents a generic class for generating unique sequential numeric identifiers that persist through application
///     restarts.
/// </summary>
/// <typeparam name="T">
///     The numeric type to generate
/// </typeparam>
public sealed class PersistentIdGenerator<T> : IIdGenerator<T> where T: INumber<T>
{
    private static readonly string SharedName = $"PersistentId{typeof(T).Name}";
    private readonly IIdGenerator<T> IdGenerator;
    private readonly string Name;

    // ReSharper disable once NotAccessedField.Local
    private readonly Task SaveTask;
    private bool ShouldSave;

    /// <inheritdoc />
    public T NextId
    {
        get
        {
            var ret = IdGenerator.NextId;
            ShouldSave = true;

            return ret;
        }
    }

    /// <inheritdoc />
    public static IIdGenerator<T> Shared { get; } = new PersistentIdGenerator<T>(SharedName);

    /// <summary>
    ///     Initializes a new instance of the <see cref="PersistentIdGenerator{T}" /> class.
    /// </summary>
    public PersistentIdGenerator(string name, T? persistedValue = default)
    {
        Name = name;
        var path = $"{Name}.json";

        if (!File.Exists(path) || persistedValue is not null)
            IdGenerator = new SequentialIdGenerator<T>(persistedValue);
        else
        {
            var json = File.ReadAllText(path);
            persistedValue = JsonSerializer.Deserialize<T>(json);
            IdGenerator = new SequentialIdGenerator<T>(persistedValue ?? T.Zero);
        }

        SaveTask = PersistentSaveAsync();
    }

    private async Task PersistentSaveAsync()
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        while (true)
            try
            {
                await timer.WaitForNextTickAsync();

                if (ShouldSave)
                {
                    var path = $"{Name}.json";

                    await path.SafeExecuteAsync(innerPath => FileEx.SafeWriteAllTextAsync(innerPath, NextId.ToString()!));

                    ShouldSave = false;
                }
            } catch
            {
                //ignored
            }
    }
}