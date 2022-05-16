using Chaos.Containers;
using Chaos.Factories.Interfaces;
using Chaos.Objects.World;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

public class ExchangeFactory : IExchangeFactory
{
    private readonly ILoggerFactory LoggerFactory;

    public ExchangeFactory(ILoggerFactory loggerFactory) => LoggerFactory = loggerFactory;

    public Exchange CreateExchange(User sender, User receiver) => new(sender, receiver, LoggerFactory.CreateLogger<Exchange>());
}