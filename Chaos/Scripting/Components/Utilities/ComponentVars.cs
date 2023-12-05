using Chaos.Collections.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World.Abstractions;

namespace Chaos.Scripting.Components.Utilities;

public class ComponentVars : StaticVars
{
    private const string CASCADE_ALL_POINTS_KEY = "all_points";
    private const string CASCADE_STAGE_KEY = "cascade_stage";
    private const string OPTIONS_KEY = "options";
    private const string POINTS_KEY = "points";
    private const string TARGETS_KEY = "targets";

    public virtual List<IPoint> GetAllPoints() => GetRequired<List<IPoint>>(CASCADE_ALL_POINTS_KEY);

    public virtual TOptions GetOptions<TOptions>() => GetRequired<TOptions>(OPTIONS_KEY);

    public virtual IReadOnlyCollection<IPoint> GetPoints() => GetRequired<IReadOnlyCollection<IPoint>>(POINTS_KEY);

    public virtual int GetStage() => GetRequired<int>(CASCADE_STAGE_KEY);

    public virtual IReadOnlyCollection<T> GetTargets<T>()
        => GetRequired<IReadOnlyCollection<MapEntity>>(TARGETS_KEY)
           .OfType<T>()
           .ToList();

    public virtual void SetAllPoints(List<IPoint> points) => Set(CASCADE_ALL_POINTS_KEY, points);
    public virtual void SetOptions(object options) => Set(OPTIONS_KEY, options);
    public virtual void SetPoints(IReadOnlyCollection<IPoint> points) => Set(POINTS_KEY, points);

    public virtual void SetStage(int stage) => Set(CASCADE_STAGE_KEY, stage);

    public virtual void SetTargets(IReadOnlyCollection<MapEntity> targets) => Set(TARGETS_KEY, targets);
}