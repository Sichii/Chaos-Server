using Chaos.Containers;
using Chaos.Factories.Abstractions;
using Chaos.Objects.World;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

public sealed class ExchangeFactory : IExchangeFactory
{
    private readonly ILoggerFactory LoggerFactory;

    public ExchangeFactory(ILoggerFactory loggerFactory) => LoggerFactory = loggerFactory;

    public Exchange CreateExchange(Aisling sender, Aisling receiver) => new(sender, receiver, LoggerFactory.CreateLogger<Exchange>());
}