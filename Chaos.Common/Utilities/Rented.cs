#region
using System;
using System.Buffers;
#endregion

namespace Chaos.Common.Utilities;

/// <summary>
///     Represents a rented span of memory, backed by an array from the shared <see cref="ArrayPool{T}" />.
/// </summary>
/// <typeparam name="T">
///     The type of elements stored in the rented span.
/// </typeparam>
public readonly struct Rented<T> : IDisposable
{
    /// <summary>
    ///     A span representing the underlying rented array
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
    ///     Initializes a new instance of the <see cref="Rented{T}" /> struct.
    /// </summary>
    public Rented(int size)
    {
        _array = ArrayPool<T>.Shared.Rent(size);
        Array = new ArraySegment<T>(_array, 0, size);
        Count = size;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rented{T}" /> struct.
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