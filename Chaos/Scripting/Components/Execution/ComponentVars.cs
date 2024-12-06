#region
using Chaos.Collections.Common;
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Scripting.Components.Execution;

public class ComponentVars : StaticVars
{
    private const string CASCADE_ALL_POINTS_KEY = "all_points";
    private const string CASCADE_STAGE_KEY = "cascade_stage";
    private const string OPTIONS_KEY = "options";
    private const string POINTS_KEY = "points";
    private const string TARGETS_KEY = "targets";

    public virtual List<Point> GetAllPoints() => GetRequired<List<Point>>(CASCADE_ALL_POINTS_KEY);

    public virtual TOptions GetOptions<TOptions>() => GetRequired<TOptions>(OPTIONS_KEY);

    public virtual IReadOnlyCollection<Point> GetPoints() => GetRequired<IReadOnlyCollection<Point>>(POINTS_KEY);

    public virtual int GetStage() => GetRequired<int>(CASCADE_STAGE_KEY);

    public virtual IReadOnlyCollection<T> GetTargets<T>()
        => GetRequired<IReadOnlyCollection<MapEntity>>(TARGETS_KEY)
           .OfType<T>()
           .ToList();

    public virtual void SetAllPoints(List<Point> points) => Set(CASCADE_ALL_POINTS_KEY, points);
    public virtual void SetOptions(object options) => Set(OPTIONS_KEY, options);
    public virtual void SetPoints(IReadOnlyCollection<Point> points) => Set(POINTS_KEY, points);

    public virtual void SetStage(int stage) => Set(CASCADE_STAGE_KEY, stage);

    public virtual void SetTargets(IReadOnlyCollection<MapEntity> targets) => Set(TARGETS_KEY, targets);
}