namespace ChaosTool.ViewModel.Observables;

public sealed class ObservableStatSheet : ObservableAttributes
{
    private int _ability;
    private int _currentHp;
    private int _currentMp;
    private int _level;

    public int Ability
    {
        get => _ability;
        set => SetField(ref _ability, value);
    }

    public int CurrentHp
    {
        get => _currentHp;
        set => SetField(ref _currentHp, value);
    }

    public int CurrentMp
    {
        get => _currentMp;
        set => SetField(ref _currentMp, value);
    }

    public int Level
    {
        get => _level;
        set => SetField(ref _level, value);
    }
}