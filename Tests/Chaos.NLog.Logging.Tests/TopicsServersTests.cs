#region
using System.Reflection;
using Chaos.NLog.Logging.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.NLog.Logging.Tests;

public sealed class TopicsServersTests
{
    [Test]
    public void All_public_static_server_properties_should_return_their_own_name()
    {
        var properties = typeof(Topics.Servers).GetProperties(BindingFlags.Public | BindingFlags.Static);

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
    public void Direct_property_access_should_cover_all_servers()
    {
        Topics.Servers
              .LobbyServer
              .Should()
              .Be("LobbyServer");

        Topics.Servers
              .LoginServer
              .Should()
              .Be("LoginServer");

        Topics.Servers
              .WorldServer
              .Should()
              .Be("WorldServer");
    }

    [Test]
    public void Specific_properties_should_match_expected_strings()
    {
        Topics.Servers
              .LobbyServer
              .Should()
              .Be("LobbyServer");

        Topics.Servers
              .LoginServer
              .Should()
              .Be("LoginServer");

        Topics.Servers
              .WorldServer
              .Should()
              .Be("WorldServer");
    }
}