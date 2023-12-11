using System.Diagnostics.CodeAnalysis;
using Chaos.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Common.Services;

/// <inheritdoc />
[ExcludeFromCodeCoverage(Justification = "Wrapper around external code")]
public sealed class Factory<T> : IFactory<T>
{
    private readonly IServiceProvider Provider;
    private readonly ObjectFactory RuntimeFactory;

    /// <summary>
    ///     Creates a new instance of <see cref="Factory{T}" />.
    /// </summary>
    /// <param name="provider">The service provider used to source services</param>
    /// <param name="runtimeFactory">An object factory delegate used to create instances</param>
    public Factory(IServiceProvider provider, ObjectFactory runtimeFactory)
    {
        Provider = provider;
        RuntimeFactory = runtimeFactory;
    }

    /// <inheritdoc />
    public T Create(params object[] args) => (T)RuntimeFactory(Provider, args);
}