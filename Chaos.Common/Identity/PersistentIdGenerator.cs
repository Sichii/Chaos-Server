using System.Numerics;
using System.Text.Json;
using Chaos.Common.Abstractions;

namespace Chaos.Common.Identity;

/// <summary>
///     Represents a generic class for generating unique sequential numeric identifiers that persist through application
///     restarts.
/// </summary>
/// <typeparam name="T">The numeric type to generate</typeparam>
public sealed class PersistentIdGenerator<T> : IIdGenerator<T> where T: INumber<T>
{
    private static readonly string FilePath = $"PersistentId{typeof(T).Name}.json";
    private readonly IIdGenerator<T> IdGenerator;

    /// <inheritdoc />
    public static IIdGenerator<T> Shared { get; }
    /// <inheritdoc />
    public T NextId => IdGenerator.NextId;

    static PersistentIdGenerator()
    {
        if (!File.Exists(FilePath))
            Shared = new PersistentIdGenerator<T>(T.Zero);
        else
        {
            var json = File.ReadAllText(FilePath);
            var persistedValue = JsonSerializer.Deserialize<T>(json);
            Shared = new PersistentIdGenerator<T>(persistedValue ?? T.Zero);
        }

        AppDomain.CurrentDomain.ProcessExit += (_, _) => File.WriteAllText(FilePath, JsonSerializer.Serialize(Shared.NextId));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PersistentIdGenerator{T}" /> class.
    /// </summary>
    public PersistentIdGenerator(T persistedValue) => IdGenerator = new SequentialIdGenerator<T>(persistedValue);
}