using System.Diagnostics.CodeAnalysis;
using Chaos.Cryptography;
using Chaos.Cryptography.Abstractions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     <see cref="Chaos.Cryptography" /> DI extensions
/// </summary>
[ExcludeFromCodeCoverage]
public static class CryptographyExtensions
{
    /// <summary>
    ///     Adds an <see cref="ICrypto" /> implementation to the
    ///     <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection" />
    /// </summary>
    /// <param name="services">
    ///     The service collection to add to
    /// </param>
    public static void AddCryptography(this IServiceCollection services) => services.AddTransient<ICrypto, Crypto>();
}