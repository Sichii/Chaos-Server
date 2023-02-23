using Chaos.Containers;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.MapScripts.Abstractions;

public abstract class MapScriptBase : SubjectiveScriptBase<MapInstance>, IMapScript
{
    /// <inheritdoc />
    protected MapScriptBase(MapInstance subject)
        : base(subject) { }

    /// <inheritdoc />
    public virtual void OnEntered(Creature creature) { }

    /// <inheritdoc />
    public virtual void OnExited(Creature creature) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}