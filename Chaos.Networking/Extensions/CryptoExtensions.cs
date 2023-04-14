using Chaos.Cryptography.Abstractions;
using Chaos.Packets;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.Networking;

/// <summary>
///     Provides extension methods for <see cref="ICrypto" />
/// </summary>
public static class CryptoExtensions
{
    /// <summary>
    ///     Decrypts a <see cref="ClientPacket" />
    /// </summary>
    /// <param name="crypto">This crypto service</param>
    /// <param name="packet">The packet to decrypt</param>
    public static void Decrypt(this ICrypto crypto, ref ClientPacket packet) =>
        crypto.Decrypt(ref packet.Buffer, (byte)packet.OpCode, packet.Sequence);

    /// <summary>
    ///     Encrypts a <see cref="ServerPacket" />
    /// </summary>
    /// <param name="crypto">This crypto service</param>
    /// <param name="packet">The packet to encrypt</param>
    public static void Encrypt(this ICrypto crypto, ref ServerPacket packet) =>
        crypto.Encrypt(ref packet.Buffer, (byte)packet.OpCode, packet.Sequence);
}