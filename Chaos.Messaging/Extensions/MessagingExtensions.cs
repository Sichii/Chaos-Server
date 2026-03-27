#region
using System.Diagnostics.CodeAnalysis;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
#endregion

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     A class containing extension methods for the Chaos.Messaging library
/// </summary>
[ExcludeFromCodeCoverage]
public static class MessagingExtensions
{
    /// <param name="services">
    ///     The service collection to add the options object to
    /// </param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Adds a default implementation of <see cref="IChannelService" /> to the service collection
        /// </summary>
        /// <param name="optionsSubSection">
        ///     If the section is not at the root level, supply the subsection here
        /// </param>
        /// <param name="configure">
        ///     An action to configure the service before it is added to the container
        /// </param>
        public void AddChannelService(string? optionsSubSection = null, Action<IChannelService>? configure = null)
        {
            services.AddOptionsFromConfig<ChannelServiceOptions>(optionsSubSection);
            services.ConfigureOptions<ChannelServiceOptionsConfigurer>();

            services.AddSingleton<IChannelService, ChannelService>(sp =>
            {
                var channelService = ActivatorUtilities.CreateInstance<ChannelService>(sp);
                configure?.Invoke(channelService);

                return channelService;
            });
        }

        /// <summary>
        ///     Adds a default implementation of <see cref="ICommandInterceptor{T}" /> to the service collection
        /// </summary>
        /// <param name="optionsSubSection">
        ///     If the section is not at the root level, supply the subsection here
        /// </param>
        /// <typeparam name="T">
        ///     The type of the command subject
        /// </typeparam>
        /// <typeparam name="TOptions">
        ///     The type of the options object to use for this command interceptor
        /// </typeparam>
        public void AddCommandInterceptor<T, TOptions>(string? optionsSubSection = null) where T: ICommandSubject
                                                                                         where TOptions: class, ICommandInterceptorOptions
        {
            services.AddOptionsFromConfig<TOptions>(optionsSubSection);
            services.AddSingleton<ICommandInterceptor<T>, CommandInterceptor<T, TOptions>>();
        }
    }
}