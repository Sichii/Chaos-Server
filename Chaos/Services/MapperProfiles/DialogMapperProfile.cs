using Chaos.Extensions.Common;
using Chaos.Networking.Entities.Server;
using Chaos.Objects.Menu;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class DialogMapperProfile : IMapperProfile<Dialog, DialogArgs>, IMapperProfile<Dialog, MenuArgs>
{
    private readonly ITypeMapper Mapper;
    public DialogMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    /// <inheritdoc />
    public Dialog Map(DialogArgs obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public Dialog Map(MenuArgs obj) => throw new NotImplementedException();

    /// <inheritdoc />
    MenuArgs IMapperProfile<Dialog, MenuArgs>.Map(Dialog obj) =>
        new()
        {
            Args = obj.MenuArgs.FirstOrDefault(),
            EntityType = obj.SourceEntity.EntityType,
            Items = Mapper.MapMany<ItemInfo>(obj.Items).ToList(),
            MenuType = obj.Type.ToMenuType()!.Value,
            Name = obj.SourceEntity.Name,
            Options = obj.Options.Select(op => op.OptionText).ToList(),
            PursuitId = 0,
            Skills = Mapper.MapMany<SkillInfo>(obj.Skills).ToList(),
            Spells = Mapper.MapMany<SpellInfo>(obj.Spells).ToList(),
            SourceId = obj.SourceEntity.Id,
            Sprite = obj.SourceEntity.Sprite,
            Color = obj.SourceEntity.Color,
            Text = obj.Text.Replace("\r\n", "\n").TrimEnd('\n'),
            Slots = obj.Slots
        };

    /// <inheritdoc />
    DialogArgs IMapperProfile<Dialog, DialogArgs>.Map(Dialog obj) =>
        new()
        {
            DialogId = 0,
            DialogType = obj.Type.ToDialogType()!.Value,
            EntityType = obj.SourceEntity.EntityType,
            HasNextButton = !string.IsNullOrWhiteSpace(obj.NextDialogKey),
            HasPreviousButton = !string.IsNullOrWhiteSpace(obj.PrevDialogKey),
            Name = obj.SourceEntity.Name,
            Options = obj.Options.Select(o => o.OptionText).ToList(),
            PursuitId = 0,
            SourceId = obj.SourceEntity.Id,
            Sprite = obj.SourceEntity.Sprite,
            Color = obj.SourceEntity.Color,
            Text = obj.Text.Replace("\r\n", "\n").TrimEnd('\n'),
            TextBoxLength = obj.TextBoxLength
        };
}