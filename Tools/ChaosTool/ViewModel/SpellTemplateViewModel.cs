using Chaos.Common.Definitions;
using Chaos.Schemas.Templates;
using ChaosTool.ViewModel.Abstractions;
using ChaosTool.ViewModel.Observables;

namespace ChaosTool.ViewModel;

public sealed class SpellTemplateViewModel : PanelTemplateViewModelBase<SpellTemplateSchema>
{
    private byte _castLines;
    private string? _prompt;
    private SpellType _spellType;

    public byte CastLines
    {
        get => _castLines;
        set => SetField(ref _castLines, value);
    }

    public string? Prompt
    {
        get => _prompt;
        set => SetField(ref _prompt, value);
    }

    public SpellType SpellType
    {
        get => _spellType;
        set => SetField(ref _spellType, value);
    }

    public ObservableLearningRequirements LearningRequirements { get; } = new();

    public SpellTemplateViewModel() => LearningRequirements.PropertyChanged += (_, _) => OnPropertyChanged(nameof(LearningRequirements));
}