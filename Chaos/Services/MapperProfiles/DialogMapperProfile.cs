using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Networking.Entities.Server;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class DialogMapperProfile : IMapperProfile<Dialog, DialogArgs>, IMapperProfile<Dialog, MenuArgs>
{
    private readonly ITypeMapper Mapper;
    public DialogMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    /// <inheritdoc />
    public Dialog Map(DialogArgs obj) => throw new NotImplementedException();

    /// <inheritdoc />
    DialogArgs IMapperProfile<Dialog, DialogArgs>.Map(Dialog obj) =>
        new()
        {
            DialogId = 0,
            DialogType = obj.Type.ToDialogType()!.Value,
            EntityType = obj.DialogSource.EntityType,
            HasNextButton = !string.IsNullOrWhiteSpace(obj.NextDialogKey),
            HasPreviousButton = !string.IsNullOrWhiteSpace(obj.PrevDialogKey),
            Name = obj.DialogSource.Name,
            Options = obj.Options.Select(o => o.OptionText).ToList(),
            PursuitId = 0,
            SourceId = obj.DialogSource.Id,
            Sprite = obj.DialogSource.Sprite,
            Color = obj.DialogSource.Color,
            Text = obj.Text.Replace("\r\n", "\n").TrimEnd('\n'),
            TextBoxLength = obj.TextBoxLength
        };

    /// <inheritdoc />
    public Dialog Map(MenuArgs obj) => throw new NotImplementedException();

    /// <inheritdoc />
    MenuArgs IMapperProfile<Dialog, MenuArgs>.Map(Dialog obj) =>
        new()
        {
            Args = obj.MenuArgs.LastOrDefault(),
            EntityType = obj.DialogSource.EntityType,
            Items = Mapper.MapMany<ItemInfo>(obj.Items).ToList(),
            MenuType = obj.Type.ToMenuType()!.Value,
            Name = obj.DialogSource.Name,
            Options = obj.Options.Select(op => op.OptionText).ToList(),
            PursuitId = 0,
            Skills = Mapper.MapMany<SkillInfo>(obj.Skills).ToList(),
            Spells = Mapper.MapMany<SpellInfo>(obj.Spells).ToList(),
            SourceId = obj.DialogSource.Id,
            Sprite = obj.DialogSource.Sprite,
            Color = obj.DialogSource.Color,
            Text = obj.Text.Replace("\r\n", "\n").TrimEnd('\n'),
            Slots = obj.Slots
        };
}