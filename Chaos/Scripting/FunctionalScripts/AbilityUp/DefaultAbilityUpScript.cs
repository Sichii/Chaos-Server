#region
using Chaos.DarkAges.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Services.Servers.Options;
#endregion

namespace Chaos.Scripting.FunctionalScripts.AbilityUp;

public class DefaultAbilityUpScript : ScriptBase, IAbilityUpScript
{
    /// <inheritdoc />
    public IAbilityUpFormula AbilityUpFormula { get; set; } = AbilityUpFormulae.Default;

    public static string Key { get; } = GetScriptKey(typeof(DefaultAbilityUpScript));

    /// <inheritdoc />
    public void AbilityUp(Aisling aisling)
    {
        aisling.UserStatSheet.AddAbilityLevel();

        if (aisling.UserStatSheet.AbilityLevel < WorldOptions.Instance.MaxAbilityLevel)
        {
            var newTna = AbilityUpFormula.CalculateTna(aisling);
            aisling.UserStatSheet.AddTna(newTna);
        }

        aisling.Client.SendAttributes(StatUpdateType.Full);
    }

    public static IAbilityUpScript Create() => FunctionalScriptRegistry.Instance.Get<IAbilityUpScript>(Key);
}