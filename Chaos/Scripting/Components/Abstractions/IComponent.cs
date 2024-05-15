using Chaos.Models.Data;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.Abstractions;

public interface IComponent
{
    void Execute(ActivationContext context, ComponentVars vars);
}