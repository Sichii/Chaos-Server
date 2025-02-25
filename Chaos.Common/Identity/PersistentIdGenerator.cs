#region
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using Chaos.Common.Abstractions;
using Chaos.Common.Utilities;
using Chaos.IO.FileSystem;
#endregion

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

    // ReSharper disable once StaticMemberInGenericType
    private static readonly string SpecialDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "Chaos",
        "PersistentIdentity",
        Assembly.GetEntryAssembly()
                ?.GetName()
                .Name
        ?? "Unknown");

    private readonly string FilePath;
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
        FilePath = Path.Combine(SpecialDirectory, $"{Name}.json");

        if (!Directory.Exists(SpecialDirectory))
            Directory.CreateDirectory(SpecialDirectory);

        if (!File.Exists(FilePath) || (persistedValue != default))
            IdGenerator = new SequentialIdGenerator<T>(persistedValue);
        else
        {
            persistedValue = FilePath.SafeExecute(innerPath => JsonSerializerEx.Deserialize<T>(innerPath, JsonSerializerOptions.Default));
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
                    await FilePath.SafeExecuteAsync(
                        innerPath => JsonSerializerEx.SerializeAsync(innerPath, IdGenerator.NextId, JsonSerializerOptions.Default));

                    ShouldSave = false;
                }
            } catch
            {
                //ignored
            }
    }
}