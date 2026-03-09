#region
using Chaos.Networking.Entities.Server;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Networking.Tests;

public sealed class LoginClientBaseTests
{
    [Test]
    public void SendLoginControl_ShouldSendArgs()
    {
        var client = MockLoginClient.Create();

        var args = new LoginControlArgs
        {
            Message = "test"
        };
        client.SendLoginControl(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendLoginMessage_ShouldSendArgs()
    {
        var client = MockLoginClient.Create();

        var args = new LoginMessageArgs();
        client.SendLoginMessage(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendLoginNotice_ShouldSendArgs()
    {
        var client = MockLoginClient.Create();

        var args = new LoginNoticeArgs();
        client.SendLoginNotice(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }

    [Test]
    public void SendMetaData_ShouldSendArgs()
    {
        var client = MockLoginClient.Create();

        var args = new MetaDataArgs();
        client.SendMetaData(args);

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeSameAs(args);
    }
}