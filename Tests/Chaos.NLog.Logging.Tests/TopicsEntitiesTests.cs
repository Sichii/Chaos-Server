#region
using System.Reflection;
using Chaos.NLog.Logging.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.NLog.Logging.Tests;

public sealed class TopicsEntitiesTests
{
    [Test]
    public void All_public_static_entity_properties_should_return_their_own_name()
    {
        var properties = typeof(Topics.Entities).GetProperties(BindingFlags.Public | BindingFlags.Static);

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
    public void Direct_property_access_should_cover_all_entities()
    {
        Topics.Entities
              .AbilityExp
              .Should()
              .Be("AbilityExp");

        Topics.Entities
              .Aisling
              .Should()
              .Be("Aisling");

        Topics.Entities
              .Backup
              .Should()
              .Be("Backup");

        Topics.Entities
              .BulletinBoard
              .Should()
              .Be("BulletinBoard");

        Topics.Entities
              .Channel
              .Should()
              .Be("Channel");

        Topics.Entities
              .Client
              .Should()
              .Be("Client");

        Topics.Entities
              .Command
              .Should()
              .Be("Command");

        Topics.Entities
              .Creature
              .Should()
              .Be("Creature");

        Topics.Entities
              .DeltaMonitor
              .Should()
              .Be("DeltaMonitor");

        Topics.Entities
              .Dialog
              .Should()
              .Be("Dialog");

        Topics.Entities
              .Effect
              .Should()
              .Be("Effect");

        Topics.Entities
              .Exchange
              .Should()
              .Be("Exchange");

        Topics.Entities
              .Experience
              .Should()
              .Be("Experience");

        Topics.Entities
              .Gold
              .Should()
              .Be("Gold");

        Topics.Entities
              .Group
              .Should()
              .Be("Group");

        Topics.Entities
              .Guild
              .Should()
              .Be("Guild");

        Topics.Entities
              .Item
              .Should()
              .Be("Item");

        Topics.Entities
              .LootTable
              .Should()
              .Be("LootTable");

        Topics.Entities
              .Mail
              .Should()
              .Be("Mail");

        Topics.Entities
              .MailBox
              .Should()
              .Be("MailBox");

        Topics.Entities
              .MapInstance
              .Should()
              .Be("MapInstance");

        Topics.Entities
              .MapTemplate
              .Should()
              .Be("MapTemplate");

        Topics.Entities
              .Merchant
              .Should()
              .Be("Merchant");

        Topics.Entities
              .Message
              .Should()
              .Be("Message");

        Topics.Entities
              .MetaData
              .Should()
              .Be("MetaData");

        Topics.Entities
              .Monster
              .Should()
              .Be("Monster");

        Topics.Entities
              .NetworkMonitor
              .Should()
              .Be("NetworkMonitor");

        Topics.Entities
              .Options
              .Should()
              .Be("Options");

        Topics.Entities
              .Packet
              .Should()
              .Be("Packet");

        Topics.Entities
              .Post
              .Should()
              .Be("Post");

        Topics.Entities
              .Quest
              .Should()
              .Be("Quest");

        Topics.Entities
              .Script
              .Should()
              .Be("Script");

        Topics.Entities
              .Skill
              .Should()
              .Be("Skill");

        Topics.Entities
              .Spell
              .Should()
              .Be("Spell");

        Topics.Entities
              .WorldMap
              .Should()
              .Be("WorldMap");
    }
}