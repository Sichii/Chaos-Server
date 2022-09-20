using Chaos.Objects.World;

namespace Chaos.Formulae.Abstractions;

public interface IExpFormula
{
    long Calculate(Monster monster, params Aisling[] aislings);
}