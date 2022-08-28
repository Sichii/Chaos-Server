// ReSharper disable InconsistentNaming

using System.Threading;
using Chaos.Entities.Schemas.World;

namespace Chaos.Data;

public record Attributes
{
    protected int _ac;
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

    public int Ac => _ac;

    public int Con => _con;

    public int Dex => _dex;

    public int Dmg => _dmg;

    public int Hit => _hit;

    public int Int => _int;

    public int MagicResistance => _magicResistance;

    public int MaximumHp => _maximumHp;

    public int MaximumMp => _maximumMp;

    public int Str => _str;

    public int Wis => _wis;

    public Attributes() { }

    public Attributes(AttributesSchema schema)
    {
        _ac = schema.Ac;
        _dmg = schema.Dmg;
        _hit = schema.Hit;
        _str = schema.Str;
        _int = schema.Int;
        _wis = schema.Wis;
        _con = schema.Con;
        _dex = schema.Dex;
        _magicResistance = schema.MagicResistance;
        _maximumHp = schema.MaximumHp;
        _maximumMp = schema.MaximumMp;
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
    }
}