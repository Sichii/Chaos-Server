#region
using System.Collections;
using System.Net;
using System.Net.Sockets;
using Chaos.Common.Synchronization;
using Chaos.Cryptography.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Networking.Tests;

public sealed class ClientRegistryTests
{
    private readonly ClientRegistry<FakeSocketClient> Registry = new();

    [Test]
    public void GetClient_ShouldReturnClient_WhenClientExists()
    {
        var client = new FakeSocketClient(1);
        Registry.TryAdd(client);

        var result = Registry.GetClient(1);

        result.Should()
              .BeSameAs(client);
    }

    [Test]
    public void GetClient_ShouldReturnNull_WhenIdDoesNotExist()
    {
        var client = new FakeSocketClient(1);
        Registry.TryAdd(client);

        var result = Registry.GetClient(99);

        result.Should()
              .BeNull();
    }

    [Test]
    public void GetClient_ShouldReturnNull_WhenRegistryIsEmpty()
    {
        var result = Registry.GetClient(1);

        result.Should()
              .BeNull();
    }

    [Test]
    public void GetEnumerator_NonGeneric_ShouldEnumerateAllClients()
    {
        var client1 = new FakeSocketClient(1);
        var client2 = new FakeSocketClient(2);

        Registry.TryAdd(client1);
        Registry.TryAdd(client2);

        IEnumerable enumerable = Registry;
        var enumerator = enumerable.GetEnumerator();
        using var disposable = enumerator as IDisposable;

        var count = 0;

        while (enumerator.MoveNext())
            count++;

        count.Should()
             .Be(2);
    }

    [Test]
    public void GetEnumerator_ShouldReturnAllClients()
    {
        var client1 = new FakeSocketClient(1);
        var client2 = new FakeSocketClient(2);
        var client3 = new FakeSocketClient(3);

        Registry.TryAdd(client1);
        Registry.TryAdd(client2);
        Registry.TryAdd(client3);

        var clients = Registry.ToList();

        clients.Should()
               .HaveCount(3);

        clients.Should()
               .Contain(client1)
               .And
               .Contain(client2)
               .And
               .Contain(client3);
    }

    [Test]
    public void GetEnumerator_ShouldReturnEmpty_WhenRegistryIsEmpty()
    {
        var clients = Registry.ToList();

        clients.Should()
               .BeEmpty();
    }

    [Test]
    public void TryAdd_ShouldReturnFalse_WhenDuplicateId()
    {
        var client1 = new FakeSocketClient(1);
        var client2 = new FakeSocketClient(1);

        Registry.TryAdd(client1);
        var result = Registry.TryAdd(client2);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void TryAdd_ShouldReturnTrue_WhenClientIsNew()
    {
        var client = new FakeSocketClient(1);

        var result = Registry.TryAdd(client);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void TryAdd_ThenRemove_ThenReAdd_ShouldSucceed()
    {
        var client = new FakeSocketClient(1);

        Registry.TryAdd(client);
        Registry.TryRemove(1, out _);
        var result = Registry.TryAdd(client);

        result.Should()
              .BeTrue();

        Registry.GetClient(1)
                .Should()
                .BeSameAs(client);
    }

    [Test]
    public void TryRemove_ShouldMakeClientUnretrievable()
    {
        var client = new FakeSocketClient(1);
        Registry.TryAdd(client);

        Registry.TryRemove(1, out _);
        var result = Registry.GetClient(1);

        result.Should()
              .BeNull();
    }

    [Test]
    public void TryRemove_ShouldReturnFalse_WhenClientDoesNotExist()
    {
        var result = Registry.TryRemove(99, out var removed);

        result.Should()
              .BeFalse();

        removed.Should()
               .BeNull();
    }

    [Test]
    public void TryRemove_ShouldReturnTrue_WhenClientExists()
    {
        var client = new FakeSocketClient(1);
        Registry.TryAdd(client);

        var result = Registry.TryRemove(1, out var removed);

        result.Should()
              .BeTrue();

        removed.Should()
               .BeSameAs(client);
    }

    internal sealed class FakeSocketClient(uint id) : ISocketClient
    {
        public ICrypto Crypto { get; set; } = null!;
        public uint Id { get; } = id;
        public FifoSemaphoreSlim ReceiveSync { get; } = new(1, 1);
        public bool Connected => false;
        public IPAddress RemoteIp => IPAddress.Loopback;
        public Socket Socket => null!;

        public void BeginReceive() { }

        public void Disconnect() { }

        #pragma warning disable CS0067
        public event EventHandler? OnDisconnected;
        #pragma warning restore CS0067

        public void Send<T>(T obj) where T: IPacketSerializable { }

        public void Send(ref Packet packet) { }

        public void SetSequence(byte newSequence) { }
    }
}