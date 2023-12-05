namespace Chaos.Cryptography.Abstractions.Definitions;

/// <summary>
///     Defines the possible encryption types
/// </summary>
public enum EncryptionType
{
    /// <summary>
    ///     These packets have no length
    /// </summary>
    None = 0,

    /// <summary>
    ///     These packets have a predetermined key
    /// </summary>
    Normal = 1,

    /// <summary>
    ///     These packets use a generated key that is salted by a repeated md5 hash
    /// </summary>
    MD5 = 2
}