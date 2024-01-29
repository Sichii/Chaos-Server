using FluentAssertions;
using Xunit;

namespace Chaos.Extensions.Common.Tests;

public sealed class ArrayExtensionsTests
{
    [Fact]
    public void Flatten_Should_Return_Elements_In_Correct_Order()
    {
        // Arrange
        var multiDimensionalArray = new[,]
        {
            {
                1,
                2,
                3
            },
            {
                4,
                5,
                6
            },
            {
                7,
                8,
                9
            }
        };

        // Act
        var flattenedArray = multiDimensionalArray.Flatten()
                                                  .ToArray();

        // Assert
        flattenedArray.Should()
                      .BeEquivalentTo(
                          new[]
                          {
                              1,
                              2,
                              3,
                              4,
                              5,
                              6,
                              7,
                              8,
                              9
                          },
                          options => options.WithStrictOrdering(),
                          "because the array should be flattened left to right, top to bottom");
    }

    [Fact]
    public void Flatten_Should_Return_Elements_In_Correct_Order_For_Jagged_Array()
    {
        // Arrange
        var jaggedArray = new[]
        {
            [
                1,
                2,
                3
            ],
            [
                4,
                5,
                6
            ],
            new[]
            {
                7,
                8,
                9
            }
        };

        // Act
        var flattenedArray = jaggedArray.Flatten()
                                        .ToArray();

        // Assert
        flattenedArray.Should()
                      .BeEquivalentTo(
                          new[]
                          {
                              1,
                              2,
                              3,
                              4,
                              5,
                              6,
                              7,
                              8,
                              9
                          },
                          options => options.WithStrictOrdering(),
                          "because the array should be flattened left to right, top to bottom");
    }

    [Fact]
    public void ShuffleInPlace_Should_Randomize_Elements_In_List()
    {
        // Arrange
        var list = new List<int>
        {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13,
            14,
            15,
            16,
            17,
            18,
            19,
            20
        };

        // Act
        list.ShuffleInPlace();

        // Assert
        list.Should()
            .NotBeInAscendingOrder("because the elements should be shuffled");

        list.Should()
            .HaveCount(20, "because no elements should be added or removed");
    }
}