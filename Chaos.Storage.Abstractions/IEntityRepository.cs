namespace Chaos.Storage.Abstractions;

/// <summary>
///     Provides the methods to load and save objects that map to a different type when serialized
/// </summary>
public interface IEntityRepository
{
    /// <summary>
    ///     Loads a schema object from the specified path, without mapping it to a different type
    /// </summary>
    /// <param name="path">
    ///     The path to load the schema object from
    /// </param>
    /// <typeparam name="TSchema">
    ///     The type of the schema object
    /// </typeparam>
    TSchema Load<TSchema>(string path);

    /// <summary>
    ///     Loads a schema object from the specified path, mapping it to the specified type
    /// </summary>
    /// <param name="path">
    ///     The path to load the schema object from
    /// </param>
    /// <param name="preMapAction">
    ///     An optional action to perform on the schema before mapping
    /// </param>
    /// <typeparam name="T">
    ///     The type to map the schema object to
    /// </typeparam>
    /// <typeparam name="TSchema">
    ///     The type of the schema object
    /// </typeparam>
    T LoadAndMap<T, TSchema>(string path, Action<TSchema>? preMapAction = null);

    /// <summary>
    ///     Asynchronously loads a schema object from the specified path, mapping it to the specified type
    /// </summary>
    /// <param name="path">
    ///     The path to load the schema object from
    /// </param>
    /// <param name="preMapAction">
    ///     An optional action to perform on the schema before mapping
    /// </param>
    /// <typeparam name="T">
    ///     The type to map the schema object to
    /// </typeparam>
    /// <typeparam name="TSchema">
    ///     The type of the schema object
    /// </typeparam>
    Task<T> LoadAndMapAsync<T, TSchema>(string path, Func<TSchema, Task>? preMapAction = null);

    /// <summary>
    ///     Loads a collection of schema objects from the specified path, mapping them to the specified type
    /// </summary>
    /// <param name="path">
    ///     The path to load the schema object from
    /// </param>
    /// <param name="preMapAction">
    ///     An optional action to perform on the schema before mapping
    /// </param>
    /// <typeparam name="T">
    ///     The type to map the schema object to
    /// </typeparam>
    /// <typeparam name="TSchema">
    ///     The type of the schema object
    /// </typeparam>
    IEnumerable<T> LoadAndMapMany<T, TSchema>(string path, Action<TSchema>? preMapAction = null);

    /// <summary>
    ///     Asynchronously loads a collection of schema objects from the specified path, mapping them to the specified type
    /// </summary>
    /// <param name="path">
    ///     The path to load the schema object from
    /// </param>
    /// <param name="preMapAction">
    ///     An optional action to perform on the schema before mapping
    /// </param>
    /// <typeparam name="T">
    ///     The type to map the schema object to
    /// </typeparam>
    /// <typeparam name="TSchema">
    ///     The type of the schema object
    /// </typeparam>
    IAsyncEnumerable<T> LoadAndMapManyAsync<T, TSchema>(string path, Func<TSchema, Task>? preMapAction = null);

    /// <summary>
    ///     Asynchronously loads a schema object from the specified path, without mapping it to a different type
    /// </summary>
    /// <param name="path">
    ///     The path to load the schema object from
    /// </param>
    /// <typeparam name="TSchema">
    ///     The type of the schema object
    /// </typeparam>
    Task<TSchema> LoadAsync<TSchema>(string path);

    /// <summary>
    ///     Loads a collection schema objects from the specified path, without mapping them to a different type
    /// </summary>
    /// <param name="path">
    ///     The path to load the schema object from
    /// </param>
    /// <typeparam name="TSchema">
    ///     The type of the schema object
    /// </typeparam>
    IEnumerable<TSchema> LoadMany<TSchema>(string path);

    /// <summary>
    ///     Asynchronously loads a collection of schema objects from the specified path, without mapping them to a different
    ///     type
    /// </summary>
    /// <param name="path">
    ///     The path to load the schema object from
    /// </param>
    /// <typeparam name="TSchema">
    ///     The type of the schema object
    /// </typeparam>
    IAsyncEnumerable<TSchema> LoadManyAsync<TSchema>(string path);

    /// <summary>
    ///     Saves a schema object to the specified path, without mapping it to a different type
    /// </summary>
    /// <param name="obj">
    ///     The object to save
    /// </param>
    /// <param name="path">
    ///     The path to save the object to
    /// </param>
    /// <typeparam name="TSchema">
    ///     The type of the object to save
    /// </typeparam>
    void Save<TSchema>(TSchema obj, string path);

    /// <summary>
    ///     Saves a schema object to the specified path, mapping it from the specified type to the schema type
    /// </summary>
    /// <param name="obj">
    ///     The object to save
    /// </param>
    /// <param name="path">
    ///     The path to save the object to
    /// </param>
    /// <typeparam name="T">
    ///     The type of the object to save
    /// </typeparam>
    /// <typeparam name="TSchema">
    ///     The type of the schema object to map the object to
    /// </typeparam>
    void SaveAndMap<T, TSchema>(T obj, string path);

    /// <summary>
    ///     Asynchronously saves a schema object to the specified path, mapping it from the specified type to the schema type
    /// </summary>
    /// <param name="obj">
    ///     The object to save
    /// </param>
    /// <param name="path">
    ///     The path to save the object to
    /// </param>
    /// <typeparam name="T">
    ///     The type of the object to save
    /// </typeparam>
    /// <typeparam name="TSchema">
    ///     The type of the schema object to map the object to
    /// </typeparam>
    Task SaveAndMapAsync<T, TSchema>(T obj, string path);

    /// <summary>
    ///     Saves a collection of schema objects to the specified path, mapping them from the specified type to the schema type
    /// </summary>
    /// <param name="obj">
    ///     The object to save
    /// </param>
    /// <param name="path">
    ///     The path to save the object to
    /// </param>
    /// <typeparam name="T">
    ///     The type of the object to save
    /// </typeparam>
    /// <typeparam name="TSchema">
    ///     The type of the schema object to map the object to
    /// </typeparam>
    void SaveAndMapMany<T, TSchema>(IEnumerable<T> obj, string path);

    /// <summary>
    ///     Asynchronously saves a collection of schema object to the specified path, mapping them from the specified type to
    ///     the schema type
    /// </summary>
    /// <param name="obj">
    ///     The object to save
    /// </param>
    /// <param name="path">
    ///     The path to save the object to
    /// </param>
    /// <typeparam name="T">
    ///     The type of the object to save
    /// </typeparam>
    /// <typeparam name="TSchema">
    ///     The type of the schema object to map the object to
    /// </typeparam>
    Task SaveAndMapManyAsync<T, TSchema>(IEnumerable<T> obj, string path);

    /// <summary>
    ///     Asynchronously saves a schema object to the specified path, without mapping it to a different type
    /// </summary>
    /// <param name="obj">
    ///     The object to save
    /// </param>
    /// <param name="path">
    ///     The path to save the object to
    /// </param>
    /// <typeparam name="TSchema">
    ///     The type of the object to save
    /// </typeparam>
    Task SaveAsync<TSchema>(TSchema obj, string path);

    /// <summary>
    ///     Saves a collection of schema objects to the specified path, without mapping them to a different type
    /// </summary>
    /// <param name="obj">
    ///     The object to save
    /// </param>
    /// <param name="path">
    ///     The path to save the object to
    /// </param>
    /// <typeparam name="TSchema">
    ///     The type of the object to save
    /// </typeparam>
    void SaveMany<TSchema>(IEnumerable<TSchema> obj, string path);

    /// <summary>
    ///     Asynchronously saves a collection of schema objects to the specified path, without mapping them to a different type
    /// </summary>
    /// <param name="obj">
    ///     The object to save
    /// </param>
    /// <param name="path">
    ///     The path to save the object to
    /// </param>
    /// <typeparam name="TSchema">
    ///     The type of the object to save
    /// </typeparam>
    Task SaveManyAsync<TSchema>(IEnumerable<TSchema> obj, string path);
}