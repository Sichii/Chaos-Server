#region
using Chaos.Formulae.Abstractions;
using Chaos.Formulae.Experience;
#endregion

namespace Chaos.Formulae;

public static class ExperienceFormulae
{
    public static readonly DefaultExperienceFormula Default = new();
    public static readonly PureExperienceFormula Pure = new();
}