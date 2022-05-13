using System.Collections.Generic;
using Chaos.Objects.Panel;
using Chaos.Scripts.Interfaces;

namespace Chaos.Factories.Interfaces;

public interface IItemScriptFactory
{
    IItemScript CreateScript(ICollection<string> scriptKeys, Item source);
}