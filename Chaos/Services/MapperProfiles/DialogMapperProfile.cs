using Chaos.Common.Abstractions;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.Templates;
using Chaos.Networking.Entities.Server;
using Chaos.Schemas.Data;
using Chaos.Schemas.Templates;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class DialogMapperProfile(ITypeMapper mapper) : IMapperProfile<Dialog, DialogArgs>,
                                                       IMapperProfile<Dialog, MenuArgs>,
                                                       IMapperProfile<DialogTemplate, DialogTemplateSchema>,
                                                       IMapperProfile<DialogOption, DialogOptionSchema>
{
    private readonly ITypeMapper Mapper = mapper;

    /// <inheritdoc />
    public Dialog Map(DialogArgs obj) => throw new NotImplementedException();

    /// <inheritdoc />
    DialogArgs IMapperProfile<Dialog, DialogArgs>.Map(Dialog obj)
        => new()
        {
            DialogId = 0,
            DialogType = obj.Type.ToDialogType()!.Value,
            EntityType = obj.DialogSource.EntityType,
            HasNextButton = !string.IsNullOrWhiteSpace(obj.NextDialogKey),
            HasPreviousButton = !string.IsNullOrWhiteSpace(obj.PrevDialogKey),
            Name = obj.DialogSource.Name,
            Options = obj.Options
                         .Select(o => o.OptionText)
                         .ToList(),
            PursuitId = 0,
            SourceId = obj.DialogSource.Id,
            Sprite = obj.DialogSource.Sprite,
            Color = obj.DialogSource.Color,
            Text = obj.Text,
            TextBoxLength = obj.TextBoxLength,
            TextBoxPrompt = obj.TextBoxPrompt
        };

    /// <inheritdoc />
    public Dialog Map(MenuArgs obj) => throw new NotImplementedException();

    /// <inheritdoc />
    MenuArgs IMapperProfile<Dialog, MenuArgs>.Map(Dialog obj)
        => new()
        {
            Args = obj.MenuArgs.LastOrDefault(),
            EntityType = obj.DialogSource.EntityType,
            Items = Mapper.MapMany<ItemInfo>(obj.Items)
                          .ToList(),
            MenuType = obj.Type.ToMenuType()!.Value,
            Name = obj.DialogSource.Name,
            Options = obj.Options
                         .Select((op, index) => (op.OptionText, (ushort)(index + 1)))
                         .ToList(),
            PursuitId = 0,
            Skills = Mapper.MapMany<SkillInfo>(obj.Skills)
                           .ToList(),
            Spells = Mapper.MapMany<SpellInfo>(obj.Spells)
                           .ToList(),
            SourceId = obj.DialogSource.Id,
            Sprite = obj.DialogSource.Sprite,
            Color = obj.DialogSource.Color,
            Text = obj.Text
                      .Replace("\r\n", "\n")
                      .TrimEnd('\n'),
            Slots = obj.Slots
        };

    /// <inheritdoc />
    public DialogTemplate Map(DialogTemplateSchema obj)
        => new()
        {
            TemplateKey = obj.TemplateKey,
            NextDialogKey = obj.NextDialogKey,
            Options = Mapper.MapMany<DialogOption>(obj.Options)
                            .ToList(),
            PrevDialogKey = obj.PrevDialogKey,
            ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
            ScriptVars = new Dictionary<string, IScriptVars>(
                obj.ScriptVars.Select(kvp => new KeyValuePair<string, IScriptVars>(kvp.Key, kvp.Value)),
                StringComparer.OrdinalIgnoreCase),
            Text = obj.Text,
            TextBoxLength = obj.TextBoxLength,
            TextBoxPrompt = obj.TextBoxPrompt,
            Type = obj.Type,
            Contextual = obj.Contextual
        };

    /// <inheritdoc />
    public DialogTemplateSchema Map(DialogTemplate obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public DialogOption Map(DialogOptionSchema obj)
        => new()
        {
            DialogKey = obj.DialogKey,
            OptionText = obj.OptionText
        };

    /// <inheritdoc />
    public DialogOptionSchema Map(DialogOption obj) => throw new NotImplementedException();
}