using Chaos.Containers;
using Chaos.Objects.World;

namespace Chaos.Factories.Interfaces;

public interface IExchangeFactory
{
    Exchange CreateExchange(Aisling sender, Aisling receiver);
}