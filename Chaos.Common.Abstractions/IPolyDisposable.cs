namespace Chaos.Common.Abstractions;

/// <summary>
///     Defines the pattern for an object that is both <see cref="IDisposable"/> and <see cref="IAsyncDisposable"/>
/// </summary>
/// <implements><see cref="IAsyncDisposable"/>, <see cref="IDisposable"/></implements>
public interface IPolyDisposable : IAsyncDisposable, IDisposable { }