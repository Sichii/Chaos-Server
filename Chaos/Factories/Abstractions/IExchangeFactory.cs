using Chaos.Containers;
using Chaos.Objects.World;

namespace Chaos.Factories.Abstractions;

public interface IExchangeFactory
{
    Exchange CreateExchange(Aisling sender, Aisling receiver);
}