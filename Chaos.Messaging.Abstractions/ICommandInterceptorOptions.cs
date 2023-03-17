namespace Chaos.Messaging.Abstractions;

/// <summary>
///     Stores the configuration of a command interceptor
/// </summary>
public interface ICommandInterceptorOptions
{
    /// <summary>
    ///     The prefix used to identify commands
    /// </summary>
    string Prefix { get; set; }
}