using Chaos.Collections;
using Chaos.Models.World;

namespace Chaos.Services.Factories.Abstractions;

public interface IExchangeFactory
{
    Exchange Create(Aisling sender, Aisling receiver);
}