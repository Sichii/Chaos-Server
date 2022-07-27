// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;
using System.Threading;

namespace Chaos.Data;

public record UserStatSheet : StatSheet
{
    // ReSharper disable once UnassignedField.Global
    protected int _currentWeight;
    protected int _toNextAbility;
    protected int _toNextLevel;
    protected uint _totalAbility;
    protected uint _totalExp;
    protected int _unspentPoints;

    public AdvClass AdvClass { get; set; }
    public BaseClass BaseClass { get; set; }

    public int CurrentWeight
    {
        get => _currentWeight;
        private set => _currentWeight = value;
    }

    public bool Master { get; set; }
    public int MaxWeight { get; set; }

    [JsonInclude]
    public int ToNextAbility
    {
        get => _toNextAbility;
        set => _toNextAbility = value;
    }

    [JsonInclude]
    public int ToNextLevel
    {
        get => _toNextLevel;
        set => _toNextLevel = value;
    }

    [JsonInclude]
    public uint TotalAbility
    {
        get => _totalAbility;
        set => _totalAbility = value;
    }

    [JsonInclude]
    public uint TotalExp
    {
        get => _totalExp;
        set => _totalExp = value;
    }

    [JsonInclude]
    public int UnspentPoints
    {
        get => _unspentPoints;
        set => _unspentPoints = value;
    }

    public static UserStatSheet NewCharacter => new()
    {
        MaxWeight = 40,
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
        _level = 1
    };

    public void AddAbility(uint amount)
    {
        if (amount > 0)
        {
            if (amount > int.MaxValue)
            {
                amount -= int.MaxValue;
                Interlocked.Add(ref _toNextAbility, -int.MaxValue);
            }

            Interlocked.Add(ref _toNextAbility, -(int)amount);
        }

        Interlocked.Add(ref _totalAbility, amount);
    }

    public void AddExp(uint amount)
    {
        if (amount > 0)
        {
            if (amount > int.MaxValue)
            {
                amount -= int.MaxValue;
                Interlocked.Add(ref _toNextLevel, -int.MaxValue);
            }

            Interlocked.Add(ref _toNextLevel, -(int)amount);
        }

        Interlocked.Add(ref _totalExp, amount);
    }

    public void AddLevel()
    {
        Interlocked.Increment(ref _level);
        Interlocked.Increment(ref _unspentPoints);
    }

    public bool AddStat(Stat stat)
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

    public void AddWeight(int amount) => Interlocked.Add(ref _currentWeight, amount);
}