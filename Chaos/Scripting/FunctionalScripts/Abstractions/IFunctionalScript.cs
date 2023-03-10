using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.FunctionalScripts.Abstractions;

public interface IFunctionalScript : IScript
{
    static virtual string Key => throw new NotImplementedException("Override this property in your implementation");
}