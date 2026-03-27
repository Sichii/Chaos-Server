#region
using System.Net;
using Chaos.DarkAges.Definitions;
using Chaos.Networking.Entities;
using Chaos.Networking.Options;
using FluentAssertions;
#endregion

namespace Chaos.Networking.Tests;

public sealed class RedirectTests
{
    [Test]
    public void Redirect_RecordEquality_SameValues_ShouldNotBeEqual_DueToDifferentCreatedTimes()
    {
        var serverInfo = new ConnectionInfo
        {
            Address = IPAddress.Parse("10.0.0.1"),
            Port = 4200
        };

        var redirect1 = new Redirect(
            1,
            serverInfo,
            ServerType.Login,
            "key",
            0);

        var redirect2 = new Redirect(
            1,
            serverInfo,
            ServerType.Login,
            "key",
            0);

        // Created is set to DateTime.UtcNow in constructor, so two instances will almost
        // certainly differ. Record equality compares all properties including Created.
        // They could theoretically be equal if created at the exact same tick.
        (redirect1 == redirect2).Should()
                                .BeFalse();
    }

    [Test]
    public void Redirect_ShouldAssignAllProperties()
    {
        var serverInfo = new ConnectionInfo
        {
            Address = IPAddress.Parse("192.168.1.1"),
            Port = 4200
        };

        var redirect = new Redirect(
            42,
            serverInfo,
            ServerType.Login,
            "testkey",
            7,
            "TestName",
            100u,
            200);

        redirect.Id
                .Should()
                .Be(42);

        redirect.Type
                .Should()
                .Be(ServerType.Login);

        redirect.Key
                .Should()
                .Be("testkey");

        redirect.Seed
                .Should()
                .Be(7);

        redirect.Name
                .Should()
                .Be("TestName");

        redirect.LoginId1
                .Should()
                .Be(100u);

        redirect.LoginId2
                .Should()
                .Be(200);

        redirect.EndPoint
                .Address
                .Should()
                .BeEquivalentTo(IPAddress.Parse("192.168.1.1"));

        redirect.EndPoint
                .Port
                .Should()
                .Be(4200);
    }

    [Test]
    public void Redirect_ShouldDefaultNameToLogin_WhenNameIsNull()
    {
        var serverInfo = new ConnectionInfo
        {
            Address = IPAddress.Parse("10.0.0.1"),
            Port = 4201
        };

        var redirect = new Redirect(
            1,
            serverInfo,
            ServerType.Lobby,
            "key",
            0);

        redirect.Name
                .Should()
                .Be("Login");
    }

    [Test]
    public void Redirect_ShouldDefaultOptionalParametersToNull()
    {
        var serverInfo = new ConnectionInfo
        {
            Address = IPAddress.Loopback,
            Port = 4200
        };

        var redirect = new Redirect(
            1,
            serverInfo,
            ServerType.Login,
            "key",
            0);

        redirect.LoginId1
                .Should()
                .BeNull();

        redirect.LoginId2
                .Should()
                .BeNull();
    }

    [Test]
    public void Redirect_ShouldNormalizeIPv6LoopbackAddress()
    {
        var serverInfo = new ConnectionInfo
        {
            Address = IPAddress.IPv6Loopback,
            Port = 4200
        };

        var redirect = new Redirect(
            1,
            serverInfo,
            ServerType.Login,
            "key",
            0);

        // IPAddress.IsLoopback returns true for ::1
        redirect.EndPoint
                .Address
                .Should()
                .BeEquivalentTo(IPAddress.Loopback);
    }

    [Test]
    public void Redirect_ShouldNormalizeLoopbackAddress()
    {
        var serverInfo = new ConnectionInfo
        {
            Address = IPAddress.Parse("127.0.0.1"),
            Port = 4200
        };

        var redirect = new Redirect(
            1,
            serverInfo,
            ServerType.Login,
            "key",
            0);

        redirect.EndPoint
                .Address
                .Should()
                .BeEquivalentTo(IPAddress.Loopback);
    }

    [Test]
    public void Redirect_ShouldPreserveNonLoopbackAddress()
    {
        var address = IPAddress.Parse("10.20.30.40");

        var serverInfo = new ConnectionInfo
        {
            Address = address,
            Port = 5000
        };

        var redirect = new Redirect(
            1,
            serverInfo,
            ServerType.World,
            "key",
            0);

        redirect.EndPoint
                .Address
                .Should()
                .BeEquivalentTo(address);
    }

    [Test]
    public void Redirect_ShouldSetCreatedToApproximatelyNow()
    {
        var before = DateTime.UtcNow;

        var serverInfo = new ConnectionInfo
        {
            Address = IPAddress.Loopback,
            Port = 4200
        };

        var redirect = new Redirect(
            1,
            serverInfo,
            ServerType.Login,
            "key",
            0);

        var after = DateTime.UtcNow;

        redirect.Created
                .Should()
                .BeOnOrAfter(before)
                .And
                .BeOnOrBefore(after);
    }
}