#region
using System.Reflection;
using Chaos.NLog.Logging.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.NLog.Logging.Tests;

public sealed class TopicsActionsTests
{
    [Test]
    public void All_public_static_action_properties_should_return_their_own_name()
    {
        var properties = typeof(Topics.Actions).GetProperties(BindingFlags.Public | BindingFlags.Static);

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
    public void Direct_property_access_should_cover_all_action_topics()
    {
        Topics.Actions
              .Accepted
              .Should()
              .Be("Accepted");

        Topics.Actions
              .Add
              .Should()
              .Be("Add");

        Topics.Actions
              .Admit
              .Should()
              .Be("Admit");

        Topics.Actions
              .Buy
              .Should()
              .Be("Buy");

        Topics.Actions
              .Canceled
              .Should()
              .Be("Canceled");

        Topics.Actions
              .Connect
              .Should()
              .Be("Connect");

        Topics.Actions
              .Create
              .Should()
              .Be("Create");

        Topics.Actions
              .Death
              .Should()
              .Be("Death");

        Topics.Actions
              .Delete
              .Should()
              .Be("Delete");

        Topics.Actions
              .Demote
              .Should()
              .Be("Demote");

        Topics.Actions
              .Deposit
              .Should()
              .Be("Deposit");

        Topics.Actions
              .Disband
              .Should()
              .Be("Disband");

        Topics.Actions
              .Disconnect
              .Should()
              .Be("Disconnect");

        Topics.Actions
              .Drop
              .Should()
              .Be("Drop");

        Topics.Actions
              .Execute
              .Should()
              .Be("Execute");

        Topics.Actions
              .Forget
              .Should()
              .Be("Forget");

        Topics.Actions
              .Highlight
              .Should()
              .Be("Highlight");

        Topics.Actions
              .Invite
              .Should()
              .Be("Invite");

        Topics.Actions
              .Join
              .Should()
              .Be("Join");

        Topics.Actions
              .Kick
              .Should()
              .Be("Kick");

        Topics.Actions
              .Learn
              .Should()
              .Be("Learn");

        Topics.Actions
              .Leave
              .Should()
              .Be("Leave");

        Topics.Actions
              .Listening
              .Should()
              .Be("Listening");

        Topics.Actions
              .Load
              .Should()
              .Be("Load");

        Topics.Actions
              .Login
              .Should()
              .Be("Login");

        Topics.Actions
              .Logout
              .Should()
              .Be("Logout");

        Topics.Actions
              .Penalty
              .Should()
              .Be("Penalty");

        Topics.Actions
              .Pickup
              .Should()
              .Be("Pickup");

        Topics.Actions
              .Processing
              .Should()
              .Be("Processing");

        Topics.Actions
              .Promote
              .Should()
              .Be("Promote");

        Topics.Actions
              .Read
              .Should()
              .Be("Read");

        Topics.Actions
              .Receive
              .Should()
              .Be("Receive");

        Topics.Actions
              .Redirect
              .Should()
              .Be("Redirect");

        Topics.Actions
              .Reload
              .Should()
              .Be("Reload");

        Topics.Actions
              .Remove
              .Should()
              .Be("Remove");

        Topics.Actions
              .Reward
              .Should()
              .Be("Reward");

        Topics.Actions
              .Save
              .Should()
              .Be("Save");

        Topics.Actions
              .Sell
              .Should()
              .Be("Sell");

        Topics.Actions
              .Send
              .Should()
              .Be("Send");

        Topics.Actions
              .Traverse
              .Should()
              .Be("Traverse");

        Topics.Actions
              .Update
              .Should()
              .Be("Update");

        Topics.Actions
              .Validation
              .Should()
              .Be("Validation");

        Topics.Actions
              .Walk
              .Should()
              .Be("Walk");

        Topics.Actions
              .Withdraw
              .Should()
              .Be("Withdraw");
    }

    [Test]
    public void Specific_properties_should_match_expected_strings()
    {
        Topics.Actions
              .Buy
              .Should()
              .Be("Buy");

        Topics.Actions
              .Login
              .Should()
              .Be("Login");

        Topics.Actions
              .Validation
              .Should()
              .Be("Validation");

        Topics.Actions
              .Withdraw
              .Should()
              .Be("Withdraw");
    }
}