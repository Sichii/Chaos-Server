using Chaos.Cryptography;
using Chaos.Cryptography.Abstractions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     <see cref="Chaos.Cryptography"/> DI extensions
/// </summary>
public static class CryptographyExtensions
{
    /// <summary>
    ///     Adds an <see cref="ICryptoClient"/> implementation to the <see cref="IServiceCollection"/>
    /// </summary>
    /// <param name="serviceCollection">The service collection to add to</param>
    public static void AddCryptography(this IServiceCollection serviceCollection) =>
        serviceCollection.AddTransient<ICryptoClient, CryptoClient>();
}