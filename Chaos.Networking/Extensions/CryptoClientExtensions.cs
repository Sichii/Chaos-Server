using Chaos.Cryptography.Abstractions;
using Chaos.Packets;

namespace Chaos.Networking.Extensions;

internal static class CryptoClientExtensions
{
    internal static void Decrypt(this ICryptoClient crypto, ref ClientPacket packet) =>
        crypto.Decrypt(ref packet.Buffer, (byte)packet.OpCode, packet.Sequence);

    internal static void Encrypt(this ICryptoClient crypto, ref ServerPacket packet) =>
        crypto.Encrypt(ref packet.Buffer, (byte)packet.OpCode, packet.Sequence);
}