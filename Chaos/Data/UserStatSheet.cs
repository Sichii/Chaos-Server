// ReSharper disable InconsistentNaming

using Chaos.Common.Definitions;

namespace Chaos.Data;

public sealed record UserStatSheet : StatSheet
{
    private AdvClass _advClass;
    private BaseClass _baseClass;
    // ReSharper disable once UnassignedField.Global
    private int _currentWeight;
    private bool _master;
    private int _maxWeight;
    private long _toNextAbility;
    private long _toNextLevel;
    private long _totalAbility;
    private long _totalExp;
    private int _unspentPoints;

    public AdvClass AdvClass
    {
        get => _advClass;
        init => _advClass = value;
    }

    public BaseClass BaseClass
    {
        get => _baseClass;
        init => _baseClass = value;
    }

    public int CurrentWeight
    {
        get => _currentWeight;
        init => _currentWeight = value;
    }

    public bool Master
    {
        get => _master;
        init => _master = value;
    }

    public int MaxWeight
    {
        get => _maxWeight;
        init => _maxWeight = value;
    }

    public uint ToNextAbility
    {
        get => Convert.ToUInt32(_toNextAbility);
        init => _toNextAbility = value;
    }

    public uint ToNextLevel
    {
        get => Convert.ToUInt32(_toNextLevel);
        init => _toNextLevel = value;
    }

    public uint TotalAbility
    {
        get => Convert.ToUInt32(_totalAbility);
        init => _totalAbility = value;
    }

    public uint TotalExp
    {
        get => Convert.ToUInt32(_totalExp);
        init => _totalExp = value;
    }

    public int UnspentPoints
    {
        get => _unspentPoints;
        init => _unspentPoints = value;
    }

    public static UserStatSheet NewCharacter => new()
    {
        _ac = 100,
        _maxWeight = 40,
        _toNextLevel = 100,
        _str = 1,
        _int = 1,
        _wis = 1,
        _con = 1,
        _dex = 1,
        _currentHp = 100,
        _maximumHp = 100,
        _currentMp = 50,
        _maximumMp = 50,
        _level = 1,
        _master = false,
        _baseClass = BaseClass.Peasant,
        _advClass = AdvClass.None,
    };

    public void AddTNA(long amount) => Interlocked.Add(ref _toNextAbility, amount);

    public void AddTNL(long amount) => Interlocked.Add(ref _toNextLevel, amount);

    public void AddTotalAbility(long amount) => Interlocked.Add(ref _totalAbility, amount);

    public void AddTotalExp(long amount) => Interlocked.Add(ref _totalExp, amount);

    public void AddWeight(int amount) => Interlocked.Add(ref _currentWeight, amount);

    public void GivePoints(int amount) => Interlocked.Add(ref _unspentPoints, amount);

    public void IncrementLevel() => Interlocked.Increment(ref _level);

    public bool IncrementStat(Stat stat)
    {
        //if it's 0, do nothing
        if (_unspentPoints == 0)
            return false;

        if (Interlocked.Decrement(ref _unspentPoints) < 0)
        {
            _unspentPoints = 0;

            return false;
        }

        switch (stat)
        {
            case Stat.STR:
                Interlocked.Increment(ref _str);

                break;
            case Stat.INT:
                Interlocked.Increment(ref _int);

                break;
            case Stat.WIS:
                Interlocked.Increment(ref _wis);

                break;
            case Stat.CON:
                Interlocked.Increment(ref _con);

                break;
            case Stat.DEX:
                Interlocked.Increment(ref _dex);

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
        }

        return true;
    }

    public void RecalculateMaxWeight() => _maxWeight = 40 + _level + _str;

    public void SetAdvClass(AdvClass advClass) => _advClass = advClass;

    public void SetBaseClass(BaseClass baseClass) => _baseClass = baseClass;
}