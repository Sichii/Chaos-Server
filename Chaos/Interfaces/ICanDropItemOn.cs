using Chaos.WorldObjects;

namespace Chaos.Interfaces;

public interface ICanDropItemOn
{
    void ItemDroppedOn(byte slot, int count, User source);
}