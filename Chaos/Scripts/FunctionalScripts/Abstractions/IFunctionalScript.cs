using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.FunctionalScripts.Abstractions;

public interface IFunctionalScript : IScript
{
    static virtual string Key => string.Empty;
}