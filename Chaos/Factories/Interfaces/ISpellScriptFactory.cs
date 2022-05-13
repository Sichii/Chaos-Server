using System.Collections.Generic;
using Chaos.Objects.Panel;
using Chaos.Scripts.Interfaces;

namespace Chaos.Factories.Interfaces;

public interface ISpellScriptFactory
{
    ISpellScript CreateScript(ICollection<string> scriptKeys, Spell source);
}