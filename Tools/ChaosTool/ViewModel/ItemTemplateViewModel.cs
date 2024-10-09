using Chaos.DarkAges.Definitions;
using Chaos.Schemas.Templates;
using ChaosTool.ViewModel.Abstractions;
using ChaosTool.ViewModel.Observables;

namespace ChaosTool.ViewModel;

public sealed class ItemTemplateViewModel : PanelTemplateViewModelBase<ItemTemplateSchema>
{
    private bool _accountBound;
    private int _buyCost;
    private string _category = null!;
    private DisplayColor _color;
    private ushort? _displaySprite;
    private EquipmentType? _equipmentType;
    private Gender? _gender;
    private bool _isDyeable;
    private bool _isModifiable;
    private int? _maxDurability;
    private int _maxStacks = 1;
    private bool _noTrade;
    private DisplayColor? _pantsColor;
    private int _sellValue;
    private byte _weight;

    public bool AccountBound
    {
        get => _accountBound;
        set => SetField(ref _accountBound, value);
    }

    public int BuyCost
    {
        get => _buyCost;
        set => SetField(ref _buyCost, value);
    }

    public string Category
    {
        get => _category;
        set => SetField(ref _category, value);
    }

    public DisplayColor Color
    {
        get => _color;
        set => SetField(ref _color, value);
    }

    public ushort? DisplaySprite
    {
        get => _displaySprite;
        set => SetField(ref _displaySprite, value);
    }

    public EquipmentType? EquipmentType
    {
        get => _equipmentType;
        set => SetField(ref _equipmentType, value);
    }

    public Gender? Gender
    {
        get => _gender;
        set => SetField(ref _gender, value);
    }

    public bool IsDyeable
    {
        get => _isDyeable;
        set => SetField(ref _isDyeable, value);
    }

    public bool IsModifiable
    {
        get => _isModifiable;
        set => SetField(ref _isModifiable, value);
    }

    public int? MaxDurability
    {
        get => _maxDurability;
        set => SetField(ref _maxDurability, value);
    }

    public int MaxStacks
    {
        get => _maxStacks;
        set => SetField(ref _maxStacks, value);
    }

    public bool NoTrade
    {
        get => _noTrade;
        set => SetField(ref _noTrade, value);
    }

    public DisplayColor? PantsColor
    {
        get => _pantsColor;
        set => SetField(ref _pantsColor, value);
    }

    public int SellValue
    {
        get => _sellValue;
        set => SetField(ref _sellValue, value);
    }

    public byte Weight
    {
        get => _weight;
        set => SetField(ref _weight, value);
    }

    public ObservableAttributes Modifiers { get; } = new();

    public ItemTemplateViewModel() => Modifiers.PropertyChanged += (_, _) => OnPropertyChanged(nameof(Modifiers));
}