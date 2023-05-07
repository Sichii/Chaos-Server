using Chaos.Messaging.Abstractions;

namespace Chaos.Messaging.Options;

/// <summary>
///     Represents the configuration options for a <see cref="CommandInterceptor{T,TOptions}" /> that uses
///     <see cref="Chaos.Models.World.Aisling" />s as the target type
/// </summary>
public class AislingCommandInterceptorOptions : ICommandInterceptorOptions
{
    /// <inheritdoc />
    public required string Prefix { get; set; }
}