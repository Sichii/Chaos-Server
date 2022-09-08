using Chaos.Cryptography.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Cryptography.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCryptography(this IServiceCollection serviceCollection) =>
        serviceCollection.AddTransient<ICryptoClient, CryptoClient>();
}