#region
using System.Reflection;
using Chaos.NLog.Logging.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.NLog.Logging.Tests;

public sealed class TopicsQualifiersTests
{
    [Test]
    public void All_public_static_qualifier_properties_should_return_their_own_name()
    {
        var properties = typeof(Topics.Qualifiers).GetProperties(BindingFlags.Public | BindingFlags.Static);

        properties.Should()
                  .NotBeEmpty();

        foreach (var property in properties)
        {
            var value = property.GetValue(null) as string;

            value.Should()
                 .NotBeNull();

            value.Should()
                 .Be(property.Name);
        }
    }

    [Test]
    public void Direct_property_access_should_cover_all_qualifiers()
    {
        Topics.Qualifiers
              .Accepted
              .Should()
              .Be("Accepted");

        Topics.Qualifiers
              .Cheating
              .Should()
              .Be("Cheating");

        Topics.Qualifiers
              .Expired
              .Should()
              .Be("Expired");

        Topics.Qualifiers
              .Facade
              .Should()
              .Be("Facade");

        Topics.Qualifiers
              .Forced
              .Should()
              .Be("Forced");

        Topics.Qualifiers
              .Harassment
              .Should()
              .Be("Harassment");

        Topics.Qualifiers
              .Raw
              .Should()
              .Be("Raw");

        Topics.Qualifiers
              .Rejected
              .Should()
              .Be("Rejected");
    }

    [Test]
    public void Specific_properties_should_match_expected_strings()
    {
        Topics.Qualifiers
              .Accepted
              .Should()
              .Be("Accepted");

        Topics.Qualifiers
              .Cheating
              .Should()
              .Be("Cheating");

        Topics.Qualifiers
              .Forced
              .Should()
              .Be("Forced");

        Topics.Qualifiers
              .Rejected
              .Should()
              .Be("Rejected");
    }
}