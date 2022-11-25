using Chaos.Objects.Menu;
using Chaos.Schemas.Content;
using Chaos.Schemas.Data;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public class DialogOptionsMapperProfile : IMapperProfile<DialogOption, DialogOptionSchema>
{
    /// <inheritdoc />
    public DialogOption Map(DialogOptionSchema obj) => new()
    {
        DialogKey = obj.DialogKey,
        OptionText = obj.OptionText
    };

    /// <inheritdoc />
    public DialogOptionSchema Map(DialogOption obj) => throw new NotImplementedException();
}