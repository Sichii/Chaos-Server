using System.Collections;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using SkiaSharp;

namespace Chaos.Client.Controls;

public abstract class Control : IRectangle, IDisposable
{
    public SKColor BackgroundColor { get; set; }
    public SKImage? BackgroundImage { get; set; }

    /// <inheritdoc />
    public int Bottom { get; set; }

    public SKColor ForegroundColor { get; set; }

    /// <inheritdoc />
    public int Height
    {
        get => Bottom - Top;
        set => Bottom = Top + value;
    }

    /// <inheritdoc />
    public int Left { get; set; }

    /// <inheritdoc />
    public int Right { get; set; }

    /// <inheritdoc />
    public int Top { get; set; }

    /// <inheritdoc />
    public int Width
    {
        get => Bottom - Top;
        set => Right = Left + value;
    }

    /// <inheritdoc />
    int IRectangle.Area => Width * Height;

    /// <inheritdoc />
    IReadOnlyList<IPoint> IPolygon.Vertices
        => new List<IPoint>
        {
            new Point(Left, Top),
            new Point(Right, Top),
            new Point(Right, Bottom),
            new Point(Left, Bottom)
        };

    /// <inheritdoc />
    public virtual void Dispose()
    {
        BackgroundImage?.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public IEnumerator<IPoint> GetEnumerator() => ((IPolygon)this).Vertices.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}