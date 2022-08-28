using Chaos.Objects.Panel.Abstractions;

namespace Chaos.Services.Factories.Interfaces;

public interface IPanelObjectFactoryProvider
{
    IPanelObjectFactory<TPanelObject> GetPanelObjectFactory<TPanelObject>() where TPanelObject: PanelObjectBase;
}