using System.Collections.Generic;
using Chaos.Objects.Panel;

namespace Chaos.Factories.Interfaces;

public interface IItemFactory
{
    Item CreateItem(string templateKey, ICollection<string>? extraScriptKeys = null);
}