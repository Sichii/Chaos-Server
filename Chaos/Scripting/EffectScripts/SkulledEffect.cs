using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.AislingScripts;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class SkulledEffect(ISimpleCache simpleCache) : ContinuousAnimationEffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(10000);

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 24
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));

    /// <inheritdoc />
    public override byte Icon => 35;

    /// <inheritdoc />
    public override string Name => "Skulled";

    private const byte SOUND = 6;

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        //if the subject was a player, update their vit
        AislingSubject?.Client.SendSound(SOUND, false); 
    }

    public override void OnTerminated()
    {
        if (AislingSubject == null) return; 
        AislingSubject.IsDead = true;
        var currentPosition = AislingSubject.Trackers.LastPosition;
        if (currentPosition != null)
        {
            AislingSubject.TryDrop(currentPosition, AislingSubject.Inventory, out GroundItem[] itemsToDrop);
            AislingSubject.TryDropGold(currentPosition, AislingSubject.Gold, out _);
            if (itemsToDrop != null)
                foreach (var groundItem in itemsToDrop)
                {
                    AislingSubject.Inventory.RemoveByTemplateKey(groundItem.Item.Template.TemplateKey);
                }
        }
        
        var mapInstance = simpleCache.Get<MapInstance>("cthonicRoom2");
        var destination = new Location("cthonicRoom2",10, 10);
        AislingSubject.TraverseMap(mapInstance, destination);
        AislingSubject.Refresh(true);
        AislingSubject.Display();
    }
    
    public override void OnDispelled() {
        if (AislingSubject != null) AislingSubject.IsDead = false;
        AislingSubject?.StatSheet.SetHp(50);
        AislingSubject?.Refresh(true);
        AislingSubject?.Display();
    }
}
