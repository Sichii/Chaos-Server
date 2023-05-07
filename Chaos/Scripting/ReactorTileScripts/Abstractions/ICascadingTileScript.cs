using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.ReactorTileScripts.Abstractions;

public interface ICascadingTileScript
{
    ComponentExecutor Executor { get; init; }
}