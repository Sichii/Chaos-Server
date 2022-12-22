using Chaos.Formulae;
using Chaos.Scripts.RuntimeScripts.Abstractions;
using Chaos.Scripts.RuntimeScripts.ExperienceDistribution;

namespace Chaos.Scripts.RuntimeScripts;

public static class ExperienceDistributionScripts
{
    public static IExperienceDistributionScript Default { get; } = new DefaultExperienceDistributionScript(ExperienceFormulae.Default);
}