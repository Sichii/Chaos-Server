# FunctionalScripts

Functional scripts are a special kind of script that is handled slightly differently than other scripts. They are used
to handle specific
kinds of interactions, and are not tied to any single entity. These scripts can be found
under `Chaos.Scripting.FunctionalScripts`.

These scripts often use [Formulae](<Formulae.md>) for calculations as part of their actions. Any functional scripts that
utilize formulae
should be implemented in a way that the formula used is swappable for other implementations of that formula.

Let's use the [DefaultApplyDamageScript](<xref:Chaos.Scripting.FunctionalScripts.ApplyDamage.ApplyAttackDamageScript>)
functional script as
an example.

[!code-csharp[](../../Chaos/Scripting/FunctionalScripts/ApplyDamage/ApplyAttackDamageScript.cs)]

In this class, you can see that this script is used to apply damage to a target, and uses
the [DefaultDamageFormula](<xref:Chaos.Formulae.Damage.DefaultDamageFormula>) to calculate the damage before it applies
it. The formula is
stored by it's interface, making it swappable with other implementations of that interface.

Anything that deals damage should do it through this script, rather than do it directly. This creates a single place
where changes and logic
can be made for how damage is applied.

### Functional Script Construction

Functional scripts have a strange construction process. Because they generally deal with highly cross-cutting concerns,
they should
generally not be created through the IoC container. Instead, they should be created through a static factory method on
the implementation.
This static factory should reference the singleton instance of
the [FunctionalScriptRegistry](<xref:Chaos.Scripting.FunctionalScripts.FunctionalScriptRegistry>). This class is
responsible for creating
functional scripts and injecting any dependencies they may have.

> [!NOTE]
> The registry is configured at startup and automatically gathers type data for any implementations
> of [IFunctionalScript](<xref:Chaos.Scripting.FunctionalScripts.Abstractions.IFunctionalScript>).