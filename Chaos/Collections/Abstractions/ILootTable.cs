using Chaos.Models.Panel;

namespace Chaos.Collections.Abstractions;

public interface ILootTable
{
    IEnumerable<Item> GenerateLoot();
}