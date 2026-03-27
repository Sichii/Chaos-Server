#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Chaos.Common.Attributes;

/// <summary>
///     Specifies an explicit bit index for a BigFlagsValue field. If not specified, fields are automatically assigned
///     sequential bit indices.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class BitIndexAttribute : Attribute
{
    public int Index { get; }

    public BitIndexAttribute(int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), "Bit index must be non-negative");

        Index = index;
    }
}