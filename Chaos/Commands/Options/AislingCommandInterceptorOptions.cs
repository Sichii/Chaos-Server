using Chaos.Messaging.Abstractions;

namespace Chaos.Commands.Options;

/// <inheritdoc />
public class AislingCommandInterceptorOptions : ICommandInterceptorOptions
{
    /// <inheritdoc />
    public required string Prefix { get; set; }
}