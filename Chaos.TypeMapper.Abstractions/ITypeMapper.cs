namespace Chaos.TypeMapper.Abstractions;

/// <summary>
///     Defines the methods needed to map one or many objects of one type to another type
/// </summary>
public interface ITypeMapper
{
    /// <summary>
    ///     Maps the specified source object to a new instance of the destination type
    /// </summary>
    /// <param name="obj">
    ///     The object to convert
    /// </param>
    /// <typeparam name="T">
    ///     The type of the object to convert
    /// </typeparam>
    /// <typeparam name="TResult">
    ///     The type to convert the object to
    /// </typeparam>
    TResult Map<T, TResult>(T obj);

    /// <summary>
    ///     Maps the specified source object to a new instance of the destination type
    /// </summary>
    /// <param name="obj">
    ///     The object to convert
    /// </param>
    /// <typeparam name="TResult">
    ///     The type to convert the object to
    /// </typeparam>
    TResult Map<TResult>(object obj);

    /// <summary>
    ///     Manys multiple source objects of the same type to new instances of the destination type
    /// </summary>
    /// <param name="obj">
    ///     The objects to convert
    /// </param>
    /// <typeparam name="TResult">
    ///     The type to convert the objects to
    /// </typeparam>
    IEnumerable<TResult> MapMany<TResult>(IEnumerable<object> obj);

    /// <summary>
    ///     Manys multiple source objects of the same type to new instances of the destination type
    /// </summary>
    /// <param name="obj">
    ///     The objects to convert
    /// </param>
    /// <typeparam name="T">
    ///     The type of the objects to convert
    /// </typeparam>
    /// <typeparam name="TResult">
    ///     The type to convert the objects to
    /// </typeparam>
    IEnumerable<TResult> MapMany<T, TResult>(IEnumerable<T> obj);
}