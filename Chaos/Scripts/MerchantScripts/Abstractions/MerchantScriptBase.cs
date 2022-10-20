using Chaos.Containers;
using Chaos.Objects.Dialog;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.MerchantScripts.Abstractions;

public abstract class MerchantScriptBase : ScriptBase, IMerchantScript
{
    protected MerchantScriptBase(Merchant subject) => Subject = subject;
    protected Merchant Subject { get; }
    protected virtual MapInstance Map => Subject.MapInstance;
    protected virtual Dialog? InitialDialog => Subject.Template.Dialog;
    
    /// <inheritdoc />
    public virtual void OnItemDroppedOn(Aisling source, byte slot, byte count) { }
    
    /// <inheritdoc />
    public virtual void OnGoldDroppedOn(Aisling source, int amount) { }

    /// <inheritdoc />
    public virtual void OnApproached(Creature source) { }

    /// <inheritdoc />
    public virtual void OnClicked(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnDeparture(Creature source) { }

    /// <inheritdoc />
    public virtual void OnAttacked(Creature source, ref int damage) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}