using Chaos.Formulae.Abstractions;
using Chaos.Formulae.Experience;

namespace Chaos.Formulae;

public static class ExperienceFormulae
{
    public static readonly IExperienceFormula Default = new DefaultExperienceFormula();
}