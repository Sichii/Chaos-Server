using System.Collections.Immutable;
using FluentAssertions;
using Xunit;

namespace Chaos.Cryptography.Tests;

public sealed class TableTests
{
    public static ImmutableArray<ushort> GenerateTable16()
    {
        var table = new ushort[256];

        for (ushort i = 0; i < 256; i++)
        {
            var value = (ushort)(i << 8);

            for (var j = 0; j < 8; j++)
                if ((value & 0x8000) != 0)
                    value = (ushort)((value << 1) ^ 0x1021);
                else
                    value <<= 1;

            table[i] = value;
        }

        return [..table];
    }

    public static ImmutableArray<uint> GenerateTable32()
    {
        var table = new uint[256];

        for (uint i = 0; i < 256; i++)
        {
            var value = i;

            for (var j = 0; j < 8; j++)
                if ((value & 1) != 0)
                    value = (value >> 1) ^ 0xEDB88320;
                else
                    value >>= 1;

            table[i] = value;
        }

        return [..table];
    }

    [Fact]
    public void Table_ShouldMatch_GeneratedTable16()
        => Tables.TABLE16
                 .Should()
                 .BeEquivalentTo(GenerateTable16(), opt => opt.WithStrictOrdering());

    [Fact]
    public void Table_ShouldMatch_GeneratedTable32()
        => Tables.TABLE32
                 .Should()
                 .BeEquivalentTo(GenerateTable32(), opt => opt.WithStrictOrdering());
}