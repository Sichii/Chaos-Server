#region
using Chaos.Extensions.Networking;
using Chaos.Packets;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Networking.Tests;

public sealed class CryptoExtensionsTests
{
    [Test]
    public void Decrypt_ShouldDelegateToServerDecrypt()
    {
        var crypto = MockCrypto.Create();

        // Create a minimal packet buffer: signature(1) + lengthHi(1) + lengthLo(1) + opcode(1) + sequence(1) + data(1) = 6 bytes
        Span<byte> raw =
        [
            0xAA,
            0x00,
            0x03,
            0x01,
            0x00,
            0xFF
        ];
        var packet = new Packet(ref raw, true);

        crypto.Decrypt(ref packet);

        crypto.ServerDecryptCallCount
              .Should()
              .Be(1);
    }

    [Test]
    public void Encrypt_ShouldDelegateToServerEncrypt()
    {
        var crypto = MockCrypto.Create();

        var packet = new Packet(0x01);

        crypto.Encrypt(ref packet);

        crypto.ServerEncryptCallCount
              .Should()
              .Be(1);
    }
}