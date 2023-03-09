using Chaos.Collections.Common;
using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Commands;

[Command("form")]
public class FormCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<ushort>(out var formNumber))
        {
            if (source.Sprite != 0)
            {
                source.Sprite = 0;
                source.Refresh(true);
            }

            return default;
        }

        source.Sprite = formNumber;
        source.Refresh(true);

        return default;
    }
}