#region
using System.Buffers;
#endregion

namespace Chaos.Common.Utilities;

/// <summary>
///     Encapsulates a rented span of memory that provides access to a contiguous block of elements of type
///     <typeparamref name="T" /> where <typeparamref name="T" /> is unmanaged and implements <see cref="IDisposable" />.
/// </summary>
/// <typeparam name="T">
///     The type of elements in the memory span, restricted to unmanaged types that implement <see cref="IDisposable" />.
/// </typeparam>
public readonly struct Rented<T> : IDisposable
{
    /// <summary>
    ///     Represents a span of rented memory that provides access to a contiguous region of memory storing elements of type
    ///     <typeparamref name="T" /> where <typeparamref name="T" /> is unmanaged and implements <see cref="IDisposable" />.
    /// </summary>
    public Span<T> Span => _array.AsSpan(0, Count);

    /// <summary>
    ///     The number of elements in the rented span.
    /// </summary>
    public int Count { get; }

    private readonly T[] _array;

    /// <summary>
    ///     Represents an array of elements, providing access to a fixed-size collection of contiguous memory locations for
    ///     storing elements of the specified type.
    /// </summary>
    public readonly ArraySegment<T> Array;

    /// <summary>
    ///     Rents a span of memory of the specified size from the shared <see cref="ArrayPool{T}" />.
    /// </summary>
    public Rented(int size)
    {
        _array = ArrayPool<T>.Shared.Rent(size);
        Array = new ArraySegment<T>(_array, 0, size);
        Count = size;
    }

    /// <summary>
    ///     Wraps a pre-rented array with the specified count of valid elements.
    /// </summary>
    internal Rented(T[] array, int count)
    {
        _array = array;
        Array = new ArraySegment<T>(array, 0, count);
        Count = count;
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose() => ArrayPool<T>.Shared.Return(_array);

    /// <summary>
    ///     Deconstructs the <see cref="Rented{T}" /> into its constituent span and array.
    /// </summary>
    /// <param name="span">
    ///     The span representing the rented memory.
    /// </param>
    /// <param name="array">
    ///     The array backing the rented memory.
    /// </param>
    public void Deconstruct(out Span<T> span, out T[] array)
    {
        span = Span;
        array = _array;
    }

    /// <summary>
    ///     Implicitly converts a <see cref="Rented{T}" /> to a <see cref="Span{T}" />.
    /// </summary>
    /// <param name="rented">
    ///     The rented span to convert.
    /// </param>
    public static implicit operator Span<T>(Rented<T> rented) => rented.Span;
}