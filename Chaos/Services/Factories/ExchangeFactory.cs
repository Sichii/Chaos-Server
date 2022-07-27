using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Services.Factories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public class ExchangeFactory : IExchangeFactory
{
    private readonly ILoggerFactory LoggerFactory;

    public ExchangeFactory(ILoggerFactory loggerFactory) => LoggerFactory = loggerFactory;

    public Exchange CreateExchange(Aisling sender, Aisling receiver) => new(sender, receiver, LoggerFactory.CreateLogger<Exchange>());
}