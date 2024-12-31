#region
using Chaos.Models.Panel;
#endregion

namespace Chaos.Collections.Abstractions;

/// <summary>
///     Provides methods needed for generating loot
/// </summary>
public interface ILootTable
{
    /// <summary>
    ///     Generates loot
    /// </summary>
    IEnumerable<Item> GenerateLoot();
}