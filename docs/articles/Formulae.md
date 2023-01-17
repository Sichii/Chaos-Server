# Formulae

Formulae are an underlying component to scripts. They are used to calculate the results of actions. For example, the final damage
calculation of an attack, taking into account AC and elemental modifiers. Formulae are found under `Chaos.Formulae`. There are several
default formulae, but developers are free to write their own.

> [!NOTE]
> Formulae are not scripts. They are not tied to any specific entity. They are simply a reusable way to calculate something  
> Formulae are often used in functional scripts, and their implementations are swappable

### DamageFormulae

Damage formulas are used to calculate the final damage of abilities. If some damage calculation must occur on a majority of damage
calculations, creating or modifying a damage formula is the way to do it.

> [!NOTE]
> The default damage formula is used by the default applyDamage functional script

### ExperienceFormulae

Experience formulas are used to calculate the amount of experience a monster gives. This calculation could include some variables like
monster level, player level, group size, and group level.

> [!NOTE]
> The default experience formula is used by the default experience distribution functional script

### LevelRangeFormulae

LevelRange formulas are used to calculate the upper and lower bounds of a level range. This is used to determine color on the world list, as
well as playing a part in experience distribution.

> [!NOTE]
> The default level range formula is used by the default experience formula

### LevelUpformulae

LevelUp formulas are used to calculate several aspects of leveling up, such as the next level's TNI, attribute increases, max weight
calculation, and more.

> [!NOTE]
> The default level up formula is used by the default level up functional script

### RegenFormulae

Regen formulas are used to calculate the amount of health and mana a creature naturally regenerates. A creature could be a player, monster,
merchant, etc...

> [!NOTE]
> The default regen formula is used by the default natural regeneration functional script