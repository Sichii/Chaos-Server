#region
using System.Text;
using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Messaging.Admin;

[Command("show", helpText: "<ids|keys|ips|mapinfo>")]
public class ShowCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var showWhat))
            return default;

        var mapInstance = source.MapInstance;

        //other people will not be able to see these message popups
        switch (showWhat.ToLower())
        {
            case "ids":
            {
                foreach (var creature in mapInstance.GetEntitiesWithinRange<Creature>(source))
                    if (!creature.Equals(source))
                        SelfReport(creature, creature.Id.ToString());

                foreach (var groundItem in mapInstance.GetEntitiesWithinRange<GroundItem>(source))
                    SelfReport(groundItem, groundItem.Id.ToString());

                break;
            }
            case "keys":
            {
                foreach (var creature in mapInstance.GetEntitiesWithinRange<Creature>(source))
                    if (!creature.Equals(source))
                        switch (creature)
                        {
                            case Merchant merchant:
                                SelfReport(merchant, merchant.Template.TemplateKey);

                                break;
                            case Monster monster:
                                SelfReport(monster, monster.Template.TemplateKey);

                                break;
                        }

                foreach (var groundItem in mapInstance.GetEntitiesWithinRange<GroundItem>(source))
                    SelfReport(groundItem, groundItem.Item.Template.TemplateKey);

                break;
            }
            case "ips":
            {
                foreach (var aisling in mapInstance.GetEntitiesWithinRange<Aisling>(source))
                    if (!aisling.Equals(source))
                        SelfReport(aisling, aisling.Client.RemoteIp.ToString());

                break;
            }
            case "mapinfo":
            {
                var builder = new StringBuilder();
                builder.AppendLine($"InstanceId: {mapInstance.InstanceId}");
                builder.AppendLine($"BaseInstanceId: {mapInstance.BaseInstanceId}");
                builder.AppendLine($"Name: {mapInstance.Name}");
                builder.AppendLine($"TemplateKey: {mapInstance.Template.TemplateKey}");
                builder.AppendLine($"Size: {mapInstance.Template.Width}x{mapInstance.Template.Height}");

                if (mapInstance.ShardingOptions is null)
                    builder.AppendLine("ShardingOptions: N/A");
                else
                {
                    builder.AppendLine("ShardingOptions:");
                    builder.AppendLine($"- Type: {mapInstance.ShardingOptions.ShardingType}");
                    builder.AppendLine($"- Limit: {mapInstance.ShardingOptions.Limit}");
                    builder.AppendLine($"- ExitLocation: {mapInstance.ShardingOptions.ExitLocation}");
                }

                builder.AppendLine($"Flags: {(byte)mapInstance.Flags}");

                //print flags each on new line
                if (mapInstance.Flags != MapFlags.None)
                    foreach (var flag in mapInstance.Flags.GetFlags())
                        if (flag != MapFlags.None)
                            builder.AppendLine($"- {flag}");

                builder.AppendLine("ScriptKeys:");

                foreach (var scriptKey in mapInstance.ScriptKeys)
                    builder.AppendLine($"- {scriptKey}");

                source.Client.SendServerMessage(
                    ServerMessageType.ScrollWindow,
                    builder.ToString()
                           .ReplaceLineEndings("\n"));

                break;
            }
        }

        return default;

        void SelfReport(NamedEntity entity, string message)
            => source.Client.SendDisplayPublicMessage(entity.Id, PublicMessageType.Normal, $"{entity.Name}: {message}");
    }
}