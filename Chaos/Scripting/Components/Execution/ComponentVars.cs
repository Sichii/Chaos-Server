#region
using Chaos.Collections.Common;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
#endregion

namespace Chaos.Scripting.Components.Execution;

public class ComponentVars : StaticVars
{
    private const string CASCADE_ALL_POINTS_KEY = "all_points";
    private const string CASCADE_STAGE_KEY = "cascade_stage";
    private const string OPTIONS_KEY = "options";
    private const string POINTS_KEY = "points";
    private const string TARGETS_KEY = "targets";
    private const string SOURCE_SCRIPT_KEY = "source_script";
    private const string BASE_DAMAGE = "base_damage";
    private const string FINAL_DAMAGE = "final_damage";

    public virtual List<Point> GetAllPoints() => GetRequired<List<Point>>(CASCADE_ALL_POINTS_KEY);
    public virtual int GetBaseDamage(Creature creature) => Get<int>($"{BASE_DAMAGE}-{creature.Id}");
    public virtual int GetFinalDamage(Creature creature) => Get<int>($"{FINAL_DAMAGE}-{creature.Id}");

    public virtual TOptions GetOptions<TOptions>() => GetRequired<TOptions>(OPTIONS_KEY);

    public virtual IReadOnlyCollection<Point> GetPoints() => GetRequired<IReadOnlyCollection<Point>>(POINTS_KEY);

    public virtual IScript GetSourceScript() => GetRequired<IScript>(SOURCE_SCRIPT_KEY);

    public virtual int GetStage() => GetRequired<int>(CASCADE_STAGE_KEY);

    public virtual IReadOnlyCollection<T> GetTargets<T>()
        => GetRequired<IReadOnlyCollection<MapEntity>>(TARGETS_KEY)
           .OfType<T>()
           .ToList();

    public virtual void SetAllPoints(List<Point> points) => Set(CASCADE_ALL_POINTS_KEY, points);

    public virtual void SetBaseDamage(Creature creature, int baseDamage)
    {
        Set($"{BASE_DAMAGE}-{creature.Id}", baseDamage);
        Set($"{FINAL_DAMAGE}-{creature.Id}", baseDamage);
    }

    public virtual void SetFinalDamage(Creature creature, int finalDamage) => Set($"{FINAL_DAMAGE}-{creature.Id}", finalDamage);
    public virtual void SetOptions(object options) => Set(OPTIONS_KEY, options);
    public virtual void SetPoints(IReadOnlyCollection<Point> points) => Set(POINTS_KEY, points);
    public virtual void SetSourceScript(IScript script) => Set(SOURCE_SCRIPT_KEY, script);

    public virtual void SetStage(int stage) => Set(CASCADE_STAGE_KEY, stage);

    public virtual void SetTargets(IReadOnlyCollection<MapEntity> targets) => Set(TARGETS_KEY, targets);
}