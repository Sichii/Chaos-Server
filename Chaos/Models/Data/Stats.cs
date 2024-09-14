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
        init => _con = value;
    }

    public int Dex
    {
        get => _dex;
        init => _dex = value;
    }

    public int Int
    {
        get => _int;
        init => _int = value;
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
}