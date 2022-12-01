using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Networking.Entities.Server;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Chaos.Utilities;

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
    MenuArgs IMapperProfile<Dialog, MenuArgs>.Map(Dialog obj)
    {
        var menuType = obj.Type.ToMenuType()!;
        var entityType = Helpers.GetEntityType(obj.SourceEntity)!;
        string name;
        ushort sprite;
        var color = DisplayColor.Default;
        uint sourceId;

        switch (obj.SourceEntity)
        {
            case Item item:
                name = item.DisplayName;
                sprite = item.Template.ItemSprite.OffsetPanelSprite;
                color = item.Color;
                sourceId = item.Id;

                break;
            case NamedEntity namedEntity:
                name = namedEntity.Name;
                sprite = namedEntity.Sprite;
                sourceId = namedEntity.Id;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(obj.SourceEntity));
        }

        return new MenuArgs
        {
            Args = obj.MenuArgs.FirstOrDefault(),
            EntityType = entityType.Value,
            Items = Mapper.MapMany<ItemInfo>(obj.Items).ToList(),
            MenuType = menuType.Value,
            Name = name,
            Options = obj.Options.Select(op => op.OptionText).ToList(),
            PursuitId = 0,
            Skills = Mapper.MapMany<SkillInfo>(obj.Skills).ToList(),
            Spells = Mapper.MapMany<SpellInfo>(obj.Spells).ToList(),
            SourceId = sourceId,
            Sprite = sprite,
            Color = color,
            Text = obj.Text.Replace("\r\n", "\n").TrimEnd('\n'),
            Slots = obj.Slots
        };
    }

    /// <inheritdoc />
    DialogArgs IMapperProfile<Dialog, DialogArgs>.Map(Dialog obj)
    {
        var dialogType = obj.Type.ToDialogType()!;
        var entityType = Helpers.GetEntityType(obj.SourceEntity)!;
        string name;
        ushort sprite;
        var color = DisplayColor.Default;
        uint sourceId;

        switch (obj.SourceEntity)
        {
            case Item item:
                name = item.DisplayName;
                sprite = item.Template.ItemSprite.OffsetPanelSprite;
                color = item.Color;
                sourceId = item.Id;

                break;
            case NamedEntity namedEntity:
                name = namedEntity.Name;
                sprite = namedEntity.Sprite;
                sourceId = namedEntity.Id;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(obj.SourceEntity));
        }

        return new DialogArgs
        {
            DialogId = 0,
            DialogType = dialogType.Value,
            EntityType = entityType.Value,
            HasNextButton = !string.IsNullOrWhiteSpace(obj.NextDialogKey),
            HasPreviousButton = !string.IsNullOrWhiteSpace(obj.PrevDialogKey),
            Name = name,
            Options = obj.Options.Select(o => o.OptionText).ToList(),
            PursuitId = 0,
            SourceId = sourceId,
            Sprite = sprite,
            Color = color,
            Text = obj.Text.Replace("\r\n", "\n").TrimEnd('\n'),
            TextBoxLength = obj.TextBoxLength
        };
    }
}