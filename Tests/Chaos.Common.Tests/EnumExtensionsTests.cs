#region
using FluentAssertions;
#endregion

namespace Chaos.Extensions.Common.Tests;

public sealed class EnumExtensionsTests
{
    [Test]
    public void GetEnumNames_Should_Prefix_Empty_For_Nullable_Enum()
    {
        // Act
        var names = EnumExtensions.GetEnumNames<Color?>()
                                  .ToArray();

        // Assert
        names.Should()
             .Equal(
                 string.Empty,
                 "Red",
                 "Green",
                 "Blue");
    }

    // GetEnumNames<T>() tests

    [Test]
    public void GetEnumNames_Should_Return_All_Names_For_NonNullable_Enum()
    {
        // Act
        var names = EnumExtensions.GetEnumNames<Color>()
                                  .ToArray();

        // Assert
        names.Should()
             .Equal("Red", "Green", "Blue");
    }

    [Test]
    public void GetEnumNames_Should_Throw_For_NonEnum_Type()
    {
        // Act
        var act = () => EnumExtensions.GetEnumNames<int>()
                                      .ToArray();

        // Assert
        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage("Int32 is not an enum");
    }

    [Test]
    public void GetEnumNames_Should_Throw_For_Nullable_NonEnum_Type()
    {
        // Act
        var act = () => EnumExtensions.GetEnumNames<int?>()
                                      .ToArray();

        // Assert
        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage("Int32 is not an enum");
    }

    [Test]
    public void GetFlags_Should_Include_Composite_When_All_Bits_Are_Set()
    {
        // Arrange
        var input = TestFlags.All; // A|B|C

        // Act
        var result = input.GetFlags()
                          .ToArray();

        // Assert
        // Expected to include None, all individual flags, and the composite All
        result.Should()
              .Equal(
                  TestFlags.None,
                  TestFlags.A,
                  TestFlags.B,
                  TestFlags.C,
                  TestFlags.All);
    }

    // GetFlags<T>(this T input) tests

    [Test]
    public void GetFlags_Should_Return_None_And_Set_Flags_When_Input_Has_Multiple_Flags()
    {
        // Arrange
        var input = TestFlags.A | TestFlags.C;

        // Act
        var result = input.GetFlags()
                          .ToArray();

        // Assert
        // Enum.GetValues order: None, A, B, C, All
        // HasFlag(None)=true; HasFlag(A)=true; HasFlag(B)=false; HasFlag(C)=true; HasFlag(All)=false
        result.Should()
              .Equal(TestFlags.None, TestFlags.A, TestFlags.C);
    }

    [Test]
    public void GetFlags_Should_Return_None_When_Input_Is_None()
    {
        // Act
        var result = TestFlags.None
                              .GetFlags()
                              .ToArray();

        // Assert
        result.Should()
              .Equal(TestFlags.None);
    }

    [Test]
    public void GetFlags_Should_Work_For_NonFlags_Enums_Including_Zero_Value()
    {
        // Arrange
        var input = PlainEnum.Two;

        // Act
        var result = input.GetFlags()
                          .ToArray();

        // Assert
        // For non-flags enums, HasFlag(0) is true, and HasFlag(value) is true only for the exact value.
        result.Should()
              .Equal(PlainEnum.Zero, PlainEnum.Two);
    }

    private enum Color
    {
        Red,
        Green,
        Blue
    }

    private enum PlainEnum
    {
        Zero = 0,
        One = 1,
        Two = 2
    }

    [Flags]
    private enum TestFlags
    {
        None = 0,
        A = 1,
        B = 2,
        C = 4,
        All = A | B | C
    }
}