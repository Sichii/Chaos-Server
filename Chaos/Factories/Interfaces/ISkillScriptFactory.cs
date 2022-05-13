using System.Collections.Generic;
using Chaos.Objects.Panel;
using Chaos.Scripts.Interfaces;

namespace Chaos.Factories.Interfaces;

public interface ISkillScriptFactory
{
    ISkillScript CreateScript(ICollection<string> scriptKeys, Skill source);
}