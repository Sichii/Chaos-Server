using Chaos.Common.Definitions;

namespace Chaos.Time;

/// <summary>
///     A <see cref="System.DateTime"/> replacement that runs at 24x speed, to be used for in-game time measurement
/// </summary>
public readonly struct GameTime : IComparable, IComparable<GameTime>, IEquatable<GameTime>
{
    private readonly DateTime DateTime;

    public int Day => DateTime.Day;
    public int Hour => DateTime.Hour;
    public int Minute => DateTime.Minute;
    public int Month => DateTime.Month;

    /// <summary>
    ///     Gets the current ingame time.
    /// </summary>
    public static GameTime Now => FromDateTime(DateTime.UtcNow);
    public long Ticks => DateTime.Ticks;

    /// <summary>
    ///     Gets the appropriate level of light for the time of day.
    /// </summary>
    public LightLevel TimeOfDay => Hour switch
    {
        >= 10 and <= 14 => LightLevel.Lightest,
        >= 8 and <= 16  => LightLevel.Lighter,
        >= 6 and <= 18  => LightLevel.Light,
        >= 4 and <= 20  => LightLevel.Dark,
        >= 2 and <= 22  => LightLevel.Darker,
        _               => LightLevel.Darkest
    };

    public int Year => DateTime.Year;

    /// <summary>
    ///     Gets the proper suffix for a day, based on the number.
    /// </summary>
    private string GetDaySuffix => (Day % 10 == 1) && (Day != 11) ? "st" :
        (Day % 10 == 2) && (Day != 12)                            ? "nd" :
        (Day % 10 == 3) && (Day != 13)                            ? "rd" : "th";

    /// <summary>
    ///     Starting date of the server.
    /// </summary>
    private static DateTime Origin { get; } = new(2022, 11, 1);
    public static GameTime operator +(GameTime g, TimeSpan t) => new(g.DateTime + t);
    public static bool operator ==(GameTime d1, GameTime d2) => d1.DateTime.Ticks == d2.DateTime.Ticks;
    public static bool operator >(GameTime t1, GameTime t2) => t1.DateTime.Ticks > t2.DateTime.Ticks;
    public static bool operator >=(GameTime t1, GameTime t2) => t1.DateTime.Ticks >= t2.DateTime.Ticks;
    public static bool operator !=(GameTime d1, GameTime d2) => d1.DateTime.Ticks != d2.DateTime.Ticks;
    public static bool operator <(GameTime t1, GameTime t2) => t1.DateTime.Ticks < t2.DateTime.Ticks;
    public static bool operator <=(GameTime t1, GameTime t2) => t1.DateTime.Ticks <= t2.DateTime.Ticks;

    public static TimeSpan operator -(GameTime a, GameTime b) => a.DateTime - b.DateTime;
    public static GameTime operator -(GameTime g, TimeSpan t) => new(g.DateTime - t);

    public GameTime(long ticks)
        : this(new DateTime(ticks)) { }

    public GameTime(DateTime time) => DateTime = time;

    public int CompareTo(object? obj)
    {
        if (obj == null)
            return 1;

        if (obj is not GameTime gameTime)
            throw new ArgumentException();

        return CompareTo(gameTime);
    }

    public int CompareTo(GameTime other) => DateTime.CompareTo(other.DateTime);

    public bool Equals(GameTime other) => DateTime.Equals(other.DateTime);

    public override bool Equals(object? obj) => obj is GameTime time && Equals(time);

    /// <summary>
    ///     Converts a DateTime object to GameTime.
    /// </summary>
    /// <param name="dTime">DateTimeobject to be converted.</param>
    public static GameTime FromDateTime(DateTime dTime) => new(dTime.Subtract(Origin).Ticks * 24);

    public override int GetHashCode() => DateTime.GetHashCode();

    /// <summary>
    ///     Converts a GameTime object to DateTime.
    /// </summary>
    public DateTime ToDateTime() => new(DateTime.Ticks / 24 + Origin.Ticks);

    /// <summary>
    ///     Custom method that will print the current time like DateTime does.
    /// </summary>
    /// <param name="format">Optional string format guide.</param>
    public string ToString(string? format = null) =>
        $@"Year {(!string.IsNullOrEmpty(format) ? DateTime.ToString(format) : DateTime.ToString(@"y, MMM d"))}{GetDaySuffix}";
}