#region
using System.Text.Json;
using Chaos.Common.Converters;
using Chaos.Common.CustomTypes;
using Chaos.Testing.Infrastructure.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class BigFlagsValueJsonConverterFactoryTests
{
    private readonly BigFlagsValueJsonConverterFactory Factory = new();

    [Test]
    public void CanConvert_BigFlagsValue_ReturnsTrue()
        => Factory.CanConvert(typeof(BigFlagsValue<TestFeatures>))
                  .Should()
                  .BeTrue();

    [Test]
    public void CanConvert_NonGenericType_ReturnsFalse()
        => Factory.CanConvert(typeof(int))
                  .Should()
                  .BeFalse();

    [Test]
    public void CanConvert_OtherGenericType_ReturnsFalse()
        => Factory.CanConvert(typeof(List<int>))
                  .Should()
                  .BeFalse();

    [Test]
    public void CreateConverter_ReturnsCorrectConverterType()
    {
        var converter = Factory.CreateConverter(typeof(BigFlagsValue<TestFeatures>), new JsonSerializerOptions());

        converter.Should()
                 .BeOfType<BigFlagsValueJsonConverter<TestFeatures>>();
    }
}