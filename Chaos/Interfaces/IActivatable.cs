using Chaos.DataObjects;

namespace Chaos.Interfaces;

public interface IActivatable
{
    void OnActivated(ActivationContext activationContext);
}