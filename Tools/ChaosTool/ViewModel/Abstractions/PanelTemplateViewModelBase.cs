using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using ChaosTool.Utility;

namespace ChaosTool.ViewModel.Abstractions;

public abstract class PanelTemplateViewModelBase<TSchema> : SchemaViewModelBase<TSchema> where TSchema: class
{
    private AdvClass? _advClass;
    private BaseClass? _class;
    private int? _cooldownMs;
    private string? _description;
    private int _level;
    private string _name = string.Empty;
    private ushort _panelSprite;
    private bool _requiresMaster;
    private string _templateKey = string.Empty;

    public AdvClass? AdvClass
    {
        get => _advClass;
        set => SetField(ref _advClass, value);
    }

    public BaseClass? Class
    {
        get => _class;
        set => SetField(ref _class, value);
    }

    public int? CooldownMs
    {
        get => _cooldownMs;
        set => SetField(ref _cooldownMs, value);
    }

    public string? Description
    {
        get => _description;
        set => SetField(ref _description, value);
    }

    public int Level
    {
        get => _level;
        set => SetField(ref _level, value);
    }

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public ushort PanelSprite
    {
        get => _panelSprite;
        set => SetField(ref _panelSprite, value);
    }

    public bool RequiresMaster
    {
        get => _requiresMaster;
        set => SetField(ref _requiresMaster, value);
    }

    public IDictionary<string, DynamicVars> ScriptVars { get; set; }
        = new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);

    public string TemplateKey
    {
        get => _templateKey;
        set => SetField(ref _templateKey, value);
    }

    public ObservingCollection<BindableString> ScriptKeys { get; } = [];

    protected PanelTemplateViewModelBase() => ScriptKeys.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ScriptKeys));
}