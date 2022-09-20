using Chaos.Formulae.Abstractions;
using Chaos.Formulae.Exp;

namespace Chaos.Formulae;

public static class ExpFormulae
{
    public static readonly IExpFormula Default = new DefaultExpFormula();
}