using Chaos.Objects.Panel.Abstractions;

namespace Chaos.Services.Factories.Abstractions;

public interface IPanelObjectFactoryProvider
{
    IPanelObjectFactory<TPanelObject> GetPanelObjectFactory<TPanelObject>() where TPanelObject: PanelObjectBase;
}