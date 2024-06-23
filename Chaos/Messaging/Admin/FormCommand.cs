using Chaos.Collections.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command("form", helpText: "<formNumber?0>")]
public class FormCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<ushort>(out var formNumber))
        {
            if (source.Sprite != 0)
                source.SetSprite(0);

            return default;
        }

        source.SetSprite(formNumber);

        return default;
    }
}