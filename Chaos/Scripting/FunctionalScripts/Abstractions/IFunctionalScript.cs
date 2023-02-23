using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.Abstractions;

public interface IFunctionalScript : IScript
{
    static virtual string Key => string.Empty;
}