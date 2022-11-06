using Chaos.Objects.Panel.Abstractions;

namespace Chaos.Factories.Abstractions;

public interface IPanelObjectFactoryProvider
{
    IPanelObjectFactory<TPanelObject> GetPanelObjectFactory<TPanelObject>() where TPanelObject: PanelObjectBase;
}