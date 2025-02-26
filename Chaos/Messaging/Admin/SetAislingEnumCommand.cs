#region
using Chaos.Collections.Common;
using Chaos.Common.Converters;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
#endregion

namespace Chaos.Messaging.Admin;

[Command("setAislingEnum", helpText: "<EnumType.EnumValue>")]
public class SetAislingEnumCommand(IClientRegistry<IChaosWorldClient> clients) : ICommand<Aisling>
{
    private readonly IClientRegistry<IChaosWorldClient> Clients = clients;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var aislingName))
            return default;

        if (!args.TryGetNext<string>(out var enumTypeAndValue))
            return default;

        var client = Clients.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(aislingName));

        if (client is null)
        {
            source.SendOrangeBarMessage("Aisling not found");

            return default;
        }

        var split = enumTypeAndValue.Split('.');

        if (split.Length != 2)
        {
            source.SendOrangeBarMessage("Invalid enum value. (TypeName.EnumValue)");

            return default;
        }

        var typeName = split[0];
        var enumValue = split[1];

        var type = TypeEx.LoadType(
            typeName,
            (asm, t) =>
            {
                if (!t.IsEnum)
                    return false;

                return asm.GetName()
                          .Name
                          ?.ContainsI("chaos")
                       ?? true;
            });

        if (type is null)
        {
            source.SendOrangeBarMessage("No such type exists.");

            return default;
        }

        var value = PrimitiveConverter.Convert(type, enumValue);

        if (type.IsFlagEnum())
            source.Trackers.Flags.AddFlag(type, (Enum)value);
        else
            source.Trackers.Enums.Set(type, (Enum)value);

        return default;
    }
}