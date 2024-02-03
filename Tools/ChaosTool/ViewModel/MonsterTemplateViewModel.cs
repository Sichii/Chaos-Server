using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Schemas.Templates;
using Chaos.Wpf.Collections.ObjectModel;
using Chaos.Wpf.Observables;
using ChaosTool.ViewModel.Abstractions;
using ChaosTool.ViewModel.Observables;

namespace ChaosTool.ViewModel;

public sealed class MonsterTemplateViewModel : SchemaViewModelBase<MonsterTemplateSchema>
{
    private int _aggroRange = -1;
    private int _assailIntervalMs;
    private int _expReward;
    private int _maxGoldDrop;
    private int _minGoldDrop;
    private int _moveIntervalMs;
    private string _name = string.Empty;
    private int _skillIntervalMs;
    private int _spellIntervalMs;
    private ushort _sprite;
    private string _templateKey = string.Empty;
    private CreatureType _type;
    private int _wanderIntervalMs;

    public int AggroRange
    {
        get => _aggroRange;
        set => SetField(ref _aggroRange, value);
    }

    public int AssailIntervalMs
    {
        get => _assailIntervalMs;
        set => SetField(ref _assailIntervalMs, value);
    }

    public int ExpReward
    {
        get => _expReward;
        set => SetField(ref _expReward, value);
    }

    public int MaxGoldDrop
    {
        get => _maxGoldDrop;
        set => SetField(ref _maxGoldDrop, value);
    }

    public int MinGoldDrop
    {
        get => _minGoldDrop;
        set => SetField(ref _minGoldDrop, value);
    }

    public int MoveIntervalMs
    {
        get => _moveIntervalMs;
        set => SetField(ref _moveIntervalMs, value);
    }

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public int SkillIntervalMs
    {
        get => _skillIntervalMs;
        set => SetField(ref _skillIntervalMs, value);
    }

    public int SpellIntervalMs
    {
        get => _spellIntervalMs;
        set => SetField(ref _spellIntervalMs, value);
    }

    public ushort Sprite
    {
        get => _sprite;
        set => SetField(ref _sprite, value);
    }

    public string TemplateKey
    {
        get => _templateKey;
        set => SetField(ref _templateKey, value);
    }

    public CreatureType Type
    {
        get => _type;
        set => SetField(ref _type, value);
    }

    public int WanderIntervalMs
    {
        get => _wanderIntervalMs;
        set => SetField(ref _wanderIntervalMs, value);
    }

    public ObservingCollection<BindableString> LootTableKeys { get; } = [];
    public ObservingCollection<BindableString> ScriptKeys { get; } = [];

    public IDictionary<string, DynamicVars> ScriptVars { get; } = new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);

    public ObservingCollection<BindableString> SkillTemplateKeys { get; } = [];
    public ObservingCollection<BindableString> SpellTemplateKeys { get; } = [];
    public ObservableStatSheet StatSheet { get; } = new();

    public MonsterTemplateViewModel()
    {
        LootTableKeys.CollectionChanged += (_, _) => OnPropertyChanged(nameof(LootTableKeys));
        ScriptKeys.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ScriptKeys));
        SkillTemplateKeys.CollectionChanged += (_, _) => OnPropertyChanged(nameof(SkillTemplateKeys));
        SpellTemplateKeys.CollectionChanged += (_, _) => OnPropertyChanged(nameof(SpellTemplateKeys));
        StatSheet.PropertyChanged += (_, _) => OnPropertyChanged(nameof(StatSheet));
    }
}