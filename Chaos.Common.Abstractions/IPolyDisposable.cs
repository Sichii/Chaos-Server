namespace Chaos.Common.Abstractions;

/// <summary>
///     Defines the pattern for an object that is both <see cref="System.IDisposable" /> and
///     <see cref="System.IAsyncDisposable" />
/// </summary>
public interface IPolyDisposable : IAsyncDisposable, IDisposable { }