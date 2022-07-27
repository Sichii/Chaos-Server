using Chaos.Objects.Panel.Abstractions;
using Chaos.Services.Factories.Interfaces;

namespace Chaos.Services.Providers.Interfaces;

public interface IPanelObjectFactoryProvider
{
    IPanelObjectFactory<TPanelObject> GetPanelObjectFactory<TPanelObject>() where TPanelObject: PanelObjectBase;
}