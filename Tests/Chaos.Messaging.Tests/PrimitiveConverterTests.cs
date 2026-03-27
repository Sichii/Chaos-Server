#region
using System.Globalization;
using Chaos.Common.Converters;
using FluentAssertions;
#endregion

namespace Chaos.Messaging.Tests;

public sealed class PrimitiveConverterTests
{
    [Test]
    public void Convert_Generic_String_With_FormatProvider_Works()
        => PrimitiveConverter.Convert<double>("3.14", CultureInfo.InvariantCulture)
                             .Should()
                             .Be(3.14);

    [Test]
    public void Convert_WithType_Nullable_Enum_And_Primitive_Are_Parsed()
    {
        PrimitiveConverter.Convert(typeof(DayOfWeek?), "Friday")
                          .Should()
                          .Be(DayOfWeek.Friday);

        PrimitiveConverter.Convert(typeof(int?), "42")
                          .Should()
                          .Be(42);
    }

    [Test]
    public void TryConvert_Generic_String_Null_Empty_Returns_Default()
    {
        PrimitiveConverter.TryConvert<int>(null!)
                          .Should()
                          .Be(0);

        PrimitiveConverter.TryConvert<int>(string.Empty)
                          .Should()
                          .Be(0);
    }
}