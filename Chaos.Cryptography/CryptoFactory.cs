using Chaos.Cryptography.Abstractions;

namespace Chaos.Cryptography;

/// <summary>
///     Creates implementations for <see cref="ICrypto" />
/// </summary>
public sealed class CryptoFactory : ICryptoFactory
{
    /// <inheritdoc />
    public ICrypto Create() => new Crypto();
}