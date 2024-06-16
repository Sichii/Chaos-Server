using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command("setlanternsize", helpText: "<none, small, large")]
public class SetLanternSizeCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<LanternSize>(out var lanternSize))
            source.SetLanternSize(LanternSize.None);

        source.SetLanternSize(lanternSize);

        return default;
    }
}