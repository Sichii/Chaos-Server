using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("destroy", helpText: "<id|name>")]
public class DestroyCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        //if a number was given
        if (args.TryGetNext<uint>(out var id))
        {
            //if the entity is not found or is an aisling
            if (!source.MapInstance.TryGetEntity<VisibleEntity>(id, out var obj) || obj is Aisling)
                source.SendOrangeBarMessage("Invalid entity.");
            else
                source.MapInstance.RemoveObject(obj);

            return default;
        }

        //if a string was given (that isnt convertable to a number)
        if (args.TryGetNext<string>(out var entityName))
        {
            //grab the closest entity with that name
            var entity = source.MapInstance
                               .GetEntities<NamedEntity>()
                               .Where(entity => entity.Name.EqualsI(entityName))
                               .MinBy(entity => entity.DistanceFrom(source));

            //if the entity is not found or is an aisling
            if (entity is null or Aisling)
                source.SendOrangeBarMessage("Invalid entity.");
            else
                source.MapInstance.RemoveObject(entity);
        }

        return default;
    }
}