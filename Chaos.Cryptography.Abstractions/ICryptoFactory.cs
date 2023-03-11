namespace Chaos.Cryptography.Abstractions;

/// <summary>
///     Defines a factory for creating <see cref="ICrypto" /> instances
/// </summary>
public interface ICryptoFactory
{
    /// <summary>
    ///     Creates a new <see cref="ICrypto" /> instance
    /// </summary>
    /// <returns></returns>
    public ICrypto Create();
}