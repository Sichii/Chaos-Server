// ReSharper disable InconsistentNaming

namespace Chaos.Models.Data;

public record Stats
{
    protected int _con;
    protected int _dex;
    protected int _int;
    protected int _str;
    protected int _wis;

    public int Con
    {
        get => _con;
        set => _con = value;
    }

    public int Dex
    {
        get => _dex;
        set => _dex = value;
    }

    public int Int
    {
        get => _int;
        set => _int = value;
    }

    public int Str
    {
        get => _str;
        set => _str = value;
    }

    public int Wis
    {
        get => _wis;
        set => _wis = value;
    }
}