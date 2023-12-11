using ChaosTool.Utility;
using ChaosTool.ViewModel.Abstractions;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

namespace ChaosTool.ViewModel.Observables;

public sealed class ObservableLearningRequirements : NotifyPropertyChangedBase
{
    private int? _requiredGold;

    public int? RequiredGold
    {
        get => _requiredGold;
        set => SetField(ref _requiredGold, value);
    }

    public ObservingCollection<ObservableItemRequirement> ItemRequirements { get; } = [];
    public ObservingCollection<ObservableAbilityRequirement> PrerequisiteSkills { get; } = [];
    public ObservingCollection<ObservableAbilityRequirement> PrerequisiteSpells { get; } = [];
    public ObservableStats RequiredStats { get; } = new();

    public ObservableLearningRequirements()
    {
        ItemRequirements.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ItemRequirements));
        PrerequisiteSkills.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrerequisiteSkills));
        PrerequisiteSpells.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrerequisiteSpells));
        RequiredStats.PropertyChanged += (_, _) => OnPropertyChanged(nameof(RequiredStats));
    }

    public bool Equals(ObservableLearningRequirements? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return (_requiredGold == other._requiredGold)
               && ItemRequirements.SequenceEqual(other.ItemRequirements)
               && PrerequisiteSkills.SequenceEqual(other.PrerequisiteSkills)
               && PrerequisiteSpells.SequenceEqual(other.PrerequisiteSpells)
               && RequiredStats.Equals(other.RequiredStats);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != GetType())
            return false;

        return Equals((ObservableLearningRequirements)obj);
    }
}