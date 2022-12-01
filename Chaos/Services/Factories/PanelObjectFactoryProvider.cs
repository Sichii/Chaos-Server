using Chaos.Objects.Panel.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Services.Factories;

public sealed class PanelObjectFactoryProvider : IPanelObjectFactoryProvider
{
    private readonly IServiceProvider ServiceProvider;

    public PanelObjectFactoryProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public IPanelObjectFactory<TPanelObject> GetPanelObjectFactory<TPanelObject>() where TPanelObject: PanelObjectBase =>
        ServiceProvider.GetRequiredService<IPanelObjectFactory<TPanelObject>>();
}