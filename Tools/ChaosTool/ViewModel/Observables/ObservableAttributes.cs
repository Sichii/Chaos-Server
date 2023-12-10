#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
namespace ChaosTool.ViewModel.Observables;

public class ObservableAttributes : ObservableStats
{
    private int _ac;
    private int _atkSpeedPct;
    private int _dmg;
    private int _flatSkillDamage;
    private int _flatSpellDamage;
    private int _hit;
    private int _magicResistance;
    private int _maximumHp;
    private int _maximumMp;
    private int _skillDamagePct;
    private int _spellDamagePct;

    public int Ac
    {
        get => _ac;
        set => SetField(ref _ac, value);
    }

    public int AtkSpeedPct
    {
        get => _atkSpeedPct;
        set => SetField(ref _atkSpeedPct, value);
    }

    public int Dmg
    {
        get => _dmg;
        set => SetField(ref _dmg, value);
    }

    public int FlatSkillDamage
    {
        get => _flatSkillDamage;
        set => SetField(ref _flatSkillDamage, value);
    }

    public int FlatSpellDamage
    {
        get => _flatSpellDamage;
        set => SetField(ref _flatSpellDamage, value);
    }

    public int Hit
    {
        get => _hit;
        set => SetField(ref _hit, value);
    }

    public int MagicResistance
    {
        get => _magicResistance;
        set => SetField(ref _magicResistance, value);
    }

    public int MaximumHp
    {
        get => _maximumHp;
        set => SetField(ref _maximumHp, value);
    }

    public int MaximumMp
    {
        get => _maximumMp;
        set => SetField(ref _maximumMp, value);
    }

    public int SkillDamagePct
    {
        get => _skillDamagePct;
        set => SetField(ref _skillDamagePct, value);
    }

    public int SpellDamagePct
    {
        get => _spellDamagePct;
        set => SetField(ref _spellDamagePct, value);
    }

    public bool Equals(ObservableAttributes? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return base.Equals(other)
               && (_ac == other._ac)
               && (_atkSpeedPct == other._atkSpeedPct)
               && (_dmg == other._dmg)
               && (_flatSkillDamage == other._flatSkillDamage)
               && (_flatSpellDamage == other._flatSpellDamage)
               && (_hit == other._hit)
               && (_magicResistance == other._magicResistance)
               && (_maximumHp == other._maximumHp)
               && (_maximumMp == other._maximumMp)
               && (_skillDamagePct == other._skillDamagePct)
               && (_spellDamagePct == other._spellDamagePct);
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

        return Equals((ObservableAttributes)obj);
    }
}