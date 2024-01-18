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
    ///     Decrypts a <see cref="Packet" />
    /// </summary>
    /// <param name="crypto">
    ///     This crypto service
    /// </param>
    /// <param name="packet">
    ///     The packet to decrypt
    /// </param>
    public static void Decrypt(this ICrypto crypto, ref Packet packet)
        => crypto.ServerDecrypt(ref packet.Buffer, packet.OpCode, packet.Sequence);

    /// <summary>
    ///     Encrypts a <see cref="Packet" />
    /// </summary>
    /// <param name="crypto">
    ///     This crypto service
    /// </param>
    /// <param name="packet">
    ///     The packet to encrypt
    /// </param>
    public static void Encrypt(this ICrypto crypto, ref Packet packet)
        => crypto.ServerEncrypt(ref packet.Buffer, packet.OpCode, packet.Sequence);
}