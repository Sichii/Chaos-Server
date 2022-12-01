using Chaos.Containers;
using Chaos.Objects.World;

namespace Chaos.Services.Factories.Abstractions;

public interface IExchangeFactory
{
    Exchange CreateExchange(Aisling sender, Aisling receiver);
}