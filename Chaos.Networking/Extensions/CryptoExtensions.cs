#region
using Chaos.Cryptography.Abstractions;
using Chaos.Packets;
#endregion

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.Networking;

/// <summary>
///     Provides extension methods for <see cref="ICrypto" />
/// </summary>
public static class CryptoExtensions
{
    /// <param name="crypto">
    ///     This crypto service
    /// </param>
    extension(ICrypto crypto)
    {
        /// <summary>
        ///     Decrypts a <see cref="Packet" />
        /// </summary>
        /// <param name="packet">
        ///     The packet to decrypt
        /// </param>
        public void Decrypt(ref Packet packet) => crypto.ServerDecrypt(ref packet.Buffer, packet.OpCode, packet.Sequence);

        /// <summary>
        ///     Encrypts a <see cref="Packet" />
        /// </summary>
        /// <param name="packet">
        ///     The packet to encrypt
        /// </param>
        public void Encrypt(ref Packet packet)
            => crypto.ServerEncrypt(
                ref packet.MemoryOwner!,
                ref packet.Length,
                packet.OpCode,
                packet.Sequence);
    }
}