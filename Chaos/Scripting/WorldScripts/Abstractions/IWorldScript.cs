#region
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Scripting.WorldScripts.Abstractions;

public interface IWorldScript : IDeltaUpdatable
{
    bool Enabled { get; }
}