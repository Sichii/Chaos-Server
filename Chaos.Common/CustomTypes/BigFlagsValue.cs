#region
using System.Numerics;
using System.Text.Json.Serialization;
using Chaos.Common.Abstractions;
using Chaos.Common.Converters;
#endregion

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Chaos.Common.CustomTypes;

/// <summary>
///     A wrapper around BigInteger that provides flag-like operations for unlimited bit flags.
/// </summary>
[JsonConverter(typeof(BigFlagsValueJsonConverterFactory))]
public readonly struct BigFlagsValue<TMarker> : IBigFlagsValue, IEquatable<BigFlagsValue<TMarker>> where TMarker: class
{
    /// <summary>
    ///     Represents an empty value with no flags set
    /// </summary>
    public static readonly BigFlagsValue<TMarker> None = new(BigInteger.Zero);

    private readonly BigInteger _value;

    /// <summary>
    ///     Returns true if no flags are set
    /// </summary>
    public bool IsEmpty => _value.IsZero;

    /// <summary>
    ///     Creates a BigFlagsValue with the specified BigInteger value
    /// </summary>
    public BigFlagsValue(BigInteger value) => _value = value;

    /// <summary>
    ///     Creates a BigFlagsValue with a single bit set at the specified index
    /// </summary>
    public BigFlagsValue(int bitIndex) => _value = BigInteger.One << bitIndex;

    /// <inheritdoc />
    public bool Equals(BigFlagsValue<TMarker> other) => _value.Equals(other._value);

    /// <summary>
    ///     Returns a new BigFlagsValue with the specified bit index cleared
    /// </summary>
    public BigFlagsValue<TMarker> ClearFlag(int bitIndex) => new(_value & ~(BigInteger.One << bitIndex));

    /// <summary>
    ///     Returns a new BigFlagsValue with the specified flag cleared
    /// </summary>
    public BigFlagsValue<TMarker> ClearFlag(BigFlagsValue<TMarker> flag) => new(_value & ~flag._value);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is BigFlagsValue<TMarker> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => _value.GetHashCode();

    /// <summary>
    ///     Returns an enumerable of all bit indices that are currently set
    /// </summary>
    public IEnumerable<int> GetSetBitIndices()
    {
        if (_value.IsZero)
            yield break;

        var bytes = _value.ToByteArray();

        for (var byteIndex = 0; byteIndex < bytes.Length; byteIndex++)
        {
            var b = bytes[byteIndex];

            if (b == 0)
                continue;

            for (var bitIndex = 0; bitIndex < 8; bitIndex++)
                if ((b & (1 << bitIndex)) != 0)
                    yield return byteIndex * 8 + bitIndex;
        }
    }

    /// <summary>
    ///     Checks if all of the specified bit indices are set
    /// </summary>
    public bool HasAllFlags(params int[] bitIndices)
    {
        foreach (var bitIndex in bitIndices)
            if (!HasFlag(bitIndex))
                return false;

        return true;
    }

    /// <summary>
    ///     Checks if any of the specified bit indices are set
    /// </summary>
    public bool HasAnyFlag(params int[] bitIndices)
    {
        foreach (var bitIndex in bitIndices)
            if (HasFlag(bitIndex))
                return true;

        return false;
    }

    /// <summary>
    ///     Checks if the specified bit index is set
    /// </summary>
    public bool HasFlag(int bitIndex)
    {
        var mask = BigInteger.One << bitIndex;

        return (_value & mask) == mask;
    }

    /// <summary>
    ///     Checks if the specified flag value is set
    /// </summary>
    public bool HasFlag(BigFlagsValue<TMarker> flag) => (_value & flag._value) == flag._value;

    public static BigFlagsValue<TMarker> operator &(BigFlagsValue<TMarker> a, BigFlagsValue<TMarker> b) => new(a._value & b._value);

    // Bitwise operators
    public static BigFlagsValue<TMarker> operator |(BigFlagsValue<TMarker> a, BigFlagsValue<TMarker> b) => new(a._value | b._value);

    // Equality operators
    public static bool operator ==(BigFlagsValue<TMarker> left, BigFlagsValue<TMarker> right) => left._value == right._value;

    public static BigFlagsValue<TMarker> operator ^(BigFlagsValue<TMarker> a, BigFlagsValue<TMarker> b) => new(a._value ^ b._value);

    // Implicit conversions
    public static implicit operator BigFlagsValue<TMarker>(BigInteger value) => new(value);
    public static implicit operator BigInteger(BigFlagsValue<TMarker> flags) => flags._value;
    public static implicit operator BigFlagsValue<TMarker>(int value) => new((BigInteger)value);
    public static implicit operator BigFlagsValue<TMarker>(long value) => new(value);

    public static bool operator !=(BigFlagsValue<TMarker> left, BigFlagsValue<TMarker> right) => left._value != right._value;
    public static BigFlagsValue<TMarker> operator ~(BigFlagsValue<TMarker> a) => new(~a._value);

    /// <summary>
    ///     Returns a new BigFlagsValue with the specified bit index set
    /// </summary>
    public BigFlagsValue<TMarker> SetFlag(int bitIndex) => new(_value | (BigInteger.One << bitIndex));

    /// <summary>
    ///     Returns a new BigFlagsValue with the specified flag set
    /// </summary>
    public BigFlagsValue<TMarker> SetFlag(BigFlagsValue<TMarker> flag) => new(_value | flag._value);

    /// <summary>
    ///     Returns a binary string representation of the flags (for debugging)
    /// </summary>
    public string ToBinaryString()
    {
        if (_value.IsZero)
            return "0";

        var bytes = _value.ToByteArray();

        // Reverse bytes (little-endian to big-endian) and skip leading zero bytes
        var significantBytes = bytes.AsEnumerable()
                                    .Reverse()
                                    .SkipWhile(b => b == 0);

        return string.Join(
            "",
            significantBytes.Select(b => Convert.ToString(b, 2)
                                                .PadLeft(8, '0')));
    }

    /// <summary>
    ///     Returns a new BigFlagsValue with the specified bit index toggled
    /// </summary>
    public BigFlagsValue<TMarker> ToggleFlag(int bitIndex) => new(_value ^ (BigInteger.One << bitIndex));

    /// <summary>
    ///     Returns a new BigFlagsValue with the specified flag toggled
    /// </summary>
    public BigFlagsValue<TMarker> ToggleFlag(BigFlagsValue<TMarker> flag) => new(_value ^ flag._value);

    /// <inheritdoc />
    public override string ToString() => BigFlags<TMarker>.ToString(this);

    /// <inheritdoc />
    Type IBigFlagsValue.Type
        => GetType()
            .GetGenericArguments()[0];

    /// <inheritdoc />
    public BigInteger Value => _value;
}