using Chaos.Collections.Common;
using Chaos.Containers;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.DialogScripts;

public class TeleportScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public TeleportScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!Subject.MenuArgs.TryGetNext<string>(out var locationString))
        {
            Subject.Reply(source, DialogString.UnknownInput.Value);

            return;
        }

        var args = new ArgumentCollection(locationString);

        if (!args.TryGetNext<string>(out var mapInstanceId))
        {
            Subject.Reply(source, DialogString.UnknownInput.Value);

            return;
        }

        MapInstance mapInstance;

        try
        {
            mapInstance = SimpleCache.Get<MapInstance>(mapInstanceId);
        } catch
        {
            Subject.Reply(source, $"No map instance with the id of {mapInstanceId} was found");

            return;
        }

        Point point;

        if (args.TryGet<int>(1, out var xPos) && args.TryGet<int>(2, out var yPos))
            point = new Point(xPos, yPos);
        else
            point = new Point(mapInstance.Template.Width / 2, mapInstance.Template.Height / 2);

        source.TraverseMap(mapInstance, point, true);
    }
}