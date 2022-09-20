// ReSharper disable InconsistentNaming

using System.Threading;

namespace Chaos.Data;

public record Attributes
{
    protected int _ac;
    protected int _atkSpeedPct;
    protected int _con;
    protected int _dex;
    protected int _dmg;
    protected int _hit;
    protected int _int;
    protected int _magicResistance;
    protected int _maximumHp;
    protected int _maximumMp;
    protected int _str;
    protected int _wis;

    public int Ac
    {
        get => _ac;
        init => _ac = value;
    }

    public int AtkSpeedPct
    {
        get => _atkSpeedPct;
        init => _atkSpeedPct = value;
    }

    public int Con
    {
        get => _con;
        init => _con = value;
    }

    public int Dex
    {
        get => _dex;
        init => _dex = value;
    }

    public int Dmg
    {
        get => _dmg;
        init => _dmg = value;
    }

    public int Hit
    {
        get => _hit;
        init => _hit = value;
    }

    public int Int
    {
        get => _int;
        init => _int = value;
    }

    public int MagicResistance
    {
        get => _magicResistance;
        init => _magicResistance = value;
    }

    public int MaximumHp
    {
        get => _maximumHp;
        init => _maximumHp = value;
    }

    public int MaximumMp
    {
        get => _maximumMp;
        init => _maximumMp = value;
    }

    public int Str
    {
        get => _str;
        init => _str = value;
    }

    public int Wis
    {
        get => _wis;
        init => _wis = value;
    }

    public virtual void Add(Attributes other)
    {
        Interlocked.Add(ref _ac, other.Ac);
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
        Interlocked.Add(ref _atkSpeedPct, other.AtkSpeedPct);
    }

    public virtual void Subtract(Attributes other)
    {
        Interlocked.Add(ref _ac, -other.Ac);
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
        Interlocked.Add(ref _atkSpeedPct, -other.AtkSpeedPct);
    }
}