#region
using Chaos.Networking.Entities.Server;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Networking.Tests;

public sealed class LobbyClientBaseTests
{
    [Test]
    public void SendConnectionInfo_ShouldSendArgs()
    {
        var client = MockLobbyClient.Create();

        var args = new ConnectionInfoArgs();
        client.SendConnectionInfo(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendServerTableResponse_ShouldSendArgs()
    {
        var client = MockLobbyClient.Create();

        var args = new ServerTableResponseArgs();
        client.SendServerTableResponse(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }
}