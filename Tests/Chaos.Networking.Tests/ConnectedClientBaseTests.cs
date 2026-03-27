#region
using System.Buffers;
using System.Net;
using Chaos.DarkAges.Definitions;
using Chaos.Networking.Entities;
using Chaos.Networking.Entities.Server;
using Chaos.Networking.Options;
using Chaos.Packets;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Networking.Tests;

public sealed class ConnectedClientBaseTests
{
    [Test]
    public void Encrypt_ShouldDelegateToServerEncrypt()
    {
        var crypto = MockCrypto.Create();
        (var client, var clientSocket, var serverSocket, var listener) = MockConnectedClient.CreateWithSockets(crypto);

        try
        {
            var owner = MemoryPool<byte>.Shared.Rent(16);
            var packet = new Packet(0x01, owner, 5);

            client.Encrypt(ref packet);

            crypto.ServerEncryptCallCount
                  .Should()
                  .Be(1);
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void IsEncrypted_ShouldDelegateToCrypto()
    {
        var crypto = MockCrypto.Create();
        crypto.IsServerEncryptedResult = true;
        (var client, _, _, _) = MockConnectedClient.CreateWithSockets(crypto);

        var result = client.IsEncrypted(0x01);

        result.Should()
              .BeTrue();

        crypto.IsServerEncryptedCallCount
              .Should()
              .Be(1);
    }

    [Test]
    public void IsEncrypted_ShouldReturnFalse_WhenCryptoReturnsFalse()
    {
        var crypto = MockCrypto.Create();
        crypto.IsServerEncryptedResult = false;
        (var client, _, _, _) = MockConnectedClient.CreateWithSockets(crypto);

        var result = client.IsEncrypted(0x01);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void SendAcceptConnection_ShouldSendAcceptConnectionArgs()
    {
        var client = MockConnectedClient.Create();

        client.SendAcceptConnection("CONNECTED SERVER");

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeOfType<AcceptConnectionArgs>()
              .Which
              .Message
              .Should()
              .Be("CONNECTED SERVER");
    }

    [Test]
    public void SendHeartBeat_ShouldSendHeartBeatArgs()
    {
        var client = MockConnectedClient.Create();

        client.SendHeartBeat(1, 2);

        var args = client.SentArgs
                         .Should()
                         .ContainSingle()
                         .Which
                         .Should()
                         .BeOfType<HeartBeatArgs>()
                         .Subject;

        args.First
            .Should()
            .Be(1);

        args.Second
            .Should()
            .Be(2);
    }

    [Test]
    public void SendRedirect_ShouldSendRedirectArgs()
    {
        var client = MockConnectedClient.Create();

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
            42,
            "TestPlayer");

        client.SendRedirect(redirect);

        var args = client.SentArgs
                         .Should()
                         .ContainSingle()
                         .Which
                         .Should()
                         .BeOfType<RedirectArgs>()
                         .Subject;

        args.Seed
            .Should()
            .Be(42);

        args.Key
            .Should()
            .Be("key");

        args.Name
            .Should()
            .Be("TestPlayer");
    }

    [Test]
    public void SendSynchronizeTicks_ShouldSendSynchronizeTicksArgs()
    {
        var client = MockConnectedClient.Create();

        client.SendSynchronizeTicks();

        client.SentArgs
              .Should()
              .ContainSingle()
              .Which
              .Should()
              .BeOfType<SynchronizeTicksArgs>();
    }
}