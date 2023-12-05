using Chaos.Collections;
using Chaos.Collections.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts;

public class TeleportScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public TeleportScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!TryFetchArgs<ArgumentCollection>(out var locationArgs))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (!locationArgs.TryGetNext<string>(out var mapInstanceId))
        {
            Subject.ReplyToUnknownInput(source);

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

        var point = new Point(mapInstance.Template.Width / 2, mapInstance.Template.Height / 2);

        if (locationArgs.TryGetNext<int>(out var xPos) && locationArgs.TryGetNext<int>(out var yPos))
        {
            var possiblePoint = new Point(xPos, yPos);

            if (mapInstance.IsWithinMap(possiblePoint))
                point = possiblePoint;
        }

        source.TraverseMap(mapInstance, point, true);
    }
}