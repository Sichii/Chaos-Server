// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace Chaos.Core.Data;

public record Attributes
{
    protected int _ac;
    protected int _attackSpeed;
    protected int _con;
    protected int _cooldownReduction;
    protected int _dex;
    protected int _dmg;
    protected int _hit;
    protected int _int;
    protected int _magicResistance;
    protected int _maximumHp;
    protected int _maximumMp;
    protected int _str;
    protected int _wis;

    [JsonInclude]
    public int Ac
    {
        get => _ac;
        private set => _ac = value;
    }

    [JsonInclude]
    public int AttackSpeed
    {
        get => _attackSpeed;
        private set => _attackSpeed = value;
    }

    [JsonInclude]
    public int Con
    {
        get => _con;
        private set => _con = value;
    }

    [JsonInclude]
    public int CooldownReduction
    {
        get => _cooldownReduction;
        private set => _cooldownReduction = value;
    }

    [JsonInclude]
    public int Dex
    {
        get => _dex;
        private set => _dex = value;
    }

    [JsonInclude]
    public int Dmg
    {
        get => _dmg;
        private set => _dmg = value;
    }

    [JsonInclude]
    public int Hit
    {
        get => _hit;
        private set => _hit = value;
    }

    [JsonInclude]
    public int Int
    {
        get => _int;
        private set => _int = value;
    }

    [JsonInclude]
    public int MagicResistance
    {
        get => _magicResistance;
        private set => _magicResistance = value;
    }

    [JsonInclude]
    public int MaximumHp
    {
        get => _maximumHp;
        private set => _maximumHp = value;
    }

    [JsonInclude]
    public int MaximumMp
    {
        get => _maximumMp;
        private set => _maximumMp = value;
    }

    [JsonInclude]
    public int Str
    {
        get => _str;
        private set => _str = value;
    }

    [JsonInclude]
    public int Wis
    {
        get => _wis;
        private set => _wis = value;
    }

    public virtual void Add(Attributes other)
    {
        Interlocked.Add(ref _ac, other.Ac);
        Interlocked.Add(ref _attackSpeed, other.AttackSpeed);
        Interlocked.Add(ref _cooldownReduction, other.CooldownReduction);
        Interlocked.Add(ref _dmg, other.Dmg);
        Interlocked.Add(ref _hit, other.Hit);
        Interlocked.Add(ref _str, other.Str);
        Interlocked.Add(ref _int, other.Int);
        Interlocked.Add(ref _wis, other.Wis);
        Interlocked.Add(ref _con, other.Con);
        Interlocked.Add(ref _dex, other.Dex);
        Interlocked.Add(ref _magicResistance, other.MagicResistance);
        Interlocked.Add(ref _maximumHp, other.MaximumHp);
        Interlocked.Add(ref _maximumMp, other.MaximumHp);
    }

    public virtual void Subtract(Attributes other)
    {
        Interlocked.Add(ref _ac, -other.Ac);
        Interlocked.Add(ref _attackSpeed, -other.AttackSpeed);
        Interlocked.Add(ref _cooldownReduction, -other.CooldownReduction);
        Interlocked.Add(ref _dmg, -other.Dmg);
        Interlocked.Add(ref _hit, -other.Hit);
        Interlocked.Add(ref _str, -other.Str);
        Interlocked.Add(ref _int, -other.Int);
        Interlocked.Add(ref _wis, -other.Wis);
        Interlocked.Add(ref _con, -other.Con);
        Interlocked.Add(ref _dex, -other.Dex);
        Interlocked.Add(ref _magicResistance, -other.MagicResistance);
        Interlocked.Add(ref _maximumHp, -other.MaximumHp);
        Interlocked.Add(ref _maximumMp, -other.MaximumHp);
    }
}