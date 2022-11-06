using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.MerchantScripts.Abstractions;

public abstract class MerchantScriptBase : SubjectiveScriptBase<Merchant>, IMerchantScript
{
    protected virtual string? InitialDialogKey => Subject.Template.DialogKey;
    protected virtual MapInstance Map => Subject.MapInstance;

    /// <inheritdoc />
    protected MerchantScriptBase(Merchant subject)
        : base(subject) { }

    /// <inheritdoc />
    public virtual void OnApproached(Creature source) { }

    /// <inheritdoc />
    public virtual void OnAttacked(Creature source, ref int damage) { }

    /// <inheritdoc />
    public virtual void OnClicked(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnDeparture(Creature source) { }

    /// <inheritdoc />
    public virtual void OnGoldDroppedOn(Aisling source, int amount) { }

    /// <inheritdoc />
    public virtual void OnItemDroppedOn(Aisling source, byte slot, byte count) { }

    /// <inheritdoc />
    public void OnPublicMessage(Creature source, string message) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}