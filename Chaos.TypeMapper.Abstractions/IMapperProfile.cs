namespace Chaos.TypeMapper.Abstractions;

/// <summary>
///     Defines a pattern for a profile used by <see cref="Chaos.TypeMapper.Abstractions.ITypeMapper" /> that is used to
///     map objects to and
///     from eachother
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
public interface IMapperProfile<T1, T2>
{
    /// <summary>
    ///     Maps <paramref name="obj" /> to an instance of type <typeparamref name="T1" />
    /// </summary>
    /// <param name="obj">The object to convert</param>
    T1 Map(T2 obj);

    /// <summary>
    ///     Maps <paramref name="obj" /> to an instance of type <typeparamref name="T2" />
    /// </summary>
    /// <param name="obj">The object to convert</param>
    T2 Map(T1 obj);
}