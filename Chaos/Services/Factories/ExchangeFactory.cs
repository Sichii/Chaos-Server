using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public sealed class ExchangeFactory : IExchangeFactory
{
    private readonly ILoggerFactory LoggerFactory;

    public ExchangeFactory(ILoggerFactory loggerFactory) => LoggerFactory = loggerFactory;

    public Exchange Create(Aisling sender, Aisling receiver) => new(sender, receiver, LoggerFactory.CreateLogger<Exchange>());
}