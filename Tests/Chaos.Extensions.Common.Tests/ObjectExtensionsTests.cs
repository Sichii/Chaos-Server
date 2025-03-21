#region
using System.ComponentModel;
using FluentAssertions;
#endregion

namespace Chaos.Extensions.Common.Tests;

public sealed class ObjectExtensionsTests
{
    [Test]
    public void GetDescription_InvalidMemberName_ReturnsEmptyString()
    {
        var description = typeof(SampleEnum).GetDescription("InvalidName");

        description.Should()
                   .BeEmpty();
    }

    [Test]
    public void GetDescription_MemberWithDescription_ReturnsDescriptionText()
    {
        var description = typeof(SampleEnum).GetDescription(SampleEnum.WithDescription);

        description.Should()
                   .Be("This is a sample description");
    }

    [Test]
    public void GetDescription_MemberWithoutDescription_ReturnsEmptyString()
    {
        var description = typeof(SampleEnum).GetDescription(SampleEnum.WithoutDescription);

        description.Should()
                   .BeEmpty();
    }

    private enum SampleEnum
    {
        [Description("This is a sample description")]
        WithDescription,
        WithoutDescription
    }
}