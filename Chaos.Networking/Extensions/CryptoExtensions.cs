using Chaos.Cryptography.Abstractions;
using Chaos.Packets;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.Networking;

public static class CryptoExtensions
{
    public static void Decrypt(this ICrypto crypto, ref ClientPacket packet) =>
        crypto.Decrypt(ref packet.Buffer, (byte)packet.OpCode, packet.Sequence);

    public static void Encrypt(this ICrypto crypto, ref ServerPacket packet) =>
        crypto.Encrypt(ref packet.Buffer, (byte)packet.OpCode, packet.Sequence);
}