using Chaos.WorldObjects;

namespace Chaos.Interfaces;

public interface ICanDropGoldOn
{
    void GoldDroppedOn(int amount, User source);
}