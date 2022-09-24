using Chaos.Cryptography;
using Chaos.Cryptography.Abstractions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

public static class CryptographyExtensions
{
    public static void AddCryptography(this IServiceCollection serviceCollection) =>
        serviceCollection.AddTransient<ICryptoClient, CryptoClient>();
}