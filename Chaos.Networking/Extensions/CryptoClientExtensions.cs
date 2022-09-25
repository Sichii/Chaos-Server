using Chaos.Cryptography.Abstractions;
using Chaos.Packets;

namespace Chaos.Networking.Extensions;

public static class CryptoClientExtensions
{
    public static void Decrypt(this ICryptoClient crypto, ref ClientPacket packet) =>
        crypto.Decrypt(ref packet.Buffer, (byte)packet.OpCode, packet.Sequence);

    public static void Encrypt(this ICryptoClient crypto, ref ServerPacket packet) =>
        crypto.Encrypt(ref packet.Buffer, (byte)packet.OpCode, packet.Sequence);
}