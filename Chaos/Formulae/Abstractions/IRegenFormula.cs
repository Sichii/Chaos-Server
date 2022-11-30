using Chaos.Objects.World;

namespace Chaos.Formulae.Abstractions;

public interface IRegenFormula
{
    public int CalculateIntervalSecs(Aisling aisling);
    public void Regenerate(Aisling aisling);
}