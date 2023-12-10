using Chaos.Collections.Common;
using Chaos.Schemas.Templates;
using ChaosTool.Utility;
using ChaosTool.ViewModel.Abstractions;
using ChaosTool.ViewModel.Observables;

namespace ChaosTool.ViewModel;

public sealed class MerchantTemplateViewModel : SchemaViewModelBase<MerchantTemplateSchema>
{
    private string _name = string.Empty;
    private int _restockIntervalHrs;
    private int _restockPct;
    private ushort _sprite;
    private string _templateKey = string.Empty;
    private int _wanderIntervalMs;

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public int RestockIntervalHrs
    {
        get => _restockIntervalHrs;
        set => SetField(ref _restockIntervalHrs, value);
    }

    public int RestockPct
    {
        get => _restockPct;
        set => SetField(ref _restockPct, value);
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

    public int WanderIntervalMs
    {
        get => _wanderIntervalMs;
        set => SetField(ref _wanderIntervalMs, value);
    }

    public ObservingCollection<ObservableItemDetails> ItemsForSale { get; } = [];
    public ObservingCollection<BindableString> ItemsToBuy { get; } = [];
    public ObservingCollection<BindableString> ScriptKeys { get; } = [];
    public IDictionary<string, DynamicVars> ScriptVars { get; } = new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);
    public ObservingCollection<BindableString> SkillsToTeach { get; } = [];
    public ObservingCollection<BindableString> SpellsToTeach { get; } = [];

    public MerchantTemplateViewModel()
    {
        ItemsForSale.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ItemsForSale));
        ItemsToBuy.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ItemsToBuy));
        ScriptKeys.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ScriptKeys));
        SkillsToTeach.CollectionChanged += (_, _) => OnPropertyChanged(nameof(SkillsToTeach));
        SpellsToTeach.CollectionChanged += (_, _) => OnPropertyChanged(nameof(SpellsToTeach));
    }
}