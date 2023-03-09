using Chaos.Cryptography.Abstractions;

namespace Chaos.Cryptography;

public sealed class CryptoFactory : ICryptoFactory
{
    /// <inheritdoc />
    public ICrypto Create() => new Crypto();
}