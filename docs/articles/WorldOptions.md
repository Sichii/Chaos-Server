# World Options

World options are changed in "appsettings.json" via section "Options:WorldOptions".  
This section is serialized into [IWorldOptions](<xref:Chaos.Servers.Options.IWorldOptions>)

## World-Server Settings

---

### Port

The port the server will listen on. Default is 4202, if this value is changed, the client will need to be edited to connect to a different
port.

### LoginRedirect

This is a redirect object that will be used to redirect players to the login server for this world when they log out.
This object consists of a hostname and port. These values should usually match up to the hostname and port specified for the LoginOptions
section.

## General Settings

---

### SaveIntervalMins

This is the amount of time in minutes between global character saves. This is on top of players automatically saving when they log out.

Do not set this value too low or it will become a burden on the server.  
A good range of values would be anywhere from 0.5 - 10

### UpdatesPerSecond

This is the number of times per second that the server will update the game state. .NET isn't great for this kind of workload due garbage
collection and JIT recompilation/OSR.

I suggest a hard cap of 60 with a good default value of 30  
If desired you can set this to 3 to emulate the original game

## Aisling Settings

---

### AislingAssailIntervalMs

All creatures have an AssailInterval. AssailInterval is essentially the cooldown in milliseconds for skills marked as assails. This value is
modified by the
AtkSpeedPct attribute and can be modified to be 3x faster or slower than the base value. This property sets the base interval for all
aislings.

A good starting value is 1500

### DropRange

This is the maximum distance from a player that they can drop items or gold on the ground.  
A value of -1 would effectively disable dropping items  
A value of 0 would only allow players to drop items directly beneath them  
A value of 13 would allow players to drop items anywhere in their viewport

### MaxHeldGold

This is the maximum amount of gold a player can hold in their inventory.

### MaximumAislingAc

This is the maximum amount of AC a player can have. damage formulas can be changed, but with the default damage formula, higher AC = more
damage taken.

### MaxLevel

This is the level cap for players. Level formulas can be changed, but with the default level formula, if you reach this level you will stop
gaining experience.

### MinimumAislingAc

This is the minimum amount of AC a player can have. damage formulas can be changed, but with the default damage formula, lower AC = less
damage taken.  
With the default damage formula, AC is a percentile, so -100 AC would make you invulnerable.

### PickupRange

This is the maximum distance from a player that they pick up items or money from the ground.
A value of -1 would effectively disable picking up items  
A value of 0 would only allow players to pick up items from directly beneath them  
A value of 13 would allow players to pick up items from anywhere in their viewport

### TradeRange

This is the maximum distance from a player that they can initiate a trade with another player  
A value of -1 would effectively disable trading  
A value of 0 would only allow players to only trade with players on the same tile, which is generally impossible    
A value of 13 would allow players to trade with anyone in their viewport

## Monster Settings

---

### MaximumMonsterAc

This is the maximum amount of AC a monster can have. damage formulas can be changed, but with the default damage formula, higher AC = more
damage taken.

### MinimumMonsterAc

This is the minimum amount of AC a monster can have. damage formulas can be changed, but with the default damage formula, lower AC = less
damage taken.
With the default damage formula, AC is a percentile, so -100 AC would make you invulnerable.

## Anti-Cheat Settings

---

### MaxActionsPerSecond

It would be bad to allow players to perform an infinite number of actions per second. Anything without a cooldown could become a huge burden
on the server. This value is used to control the maximum number of actions a player can take in a second. An action is defined as using any
spell, skill, or item. This includes equipping items.

A good range of values for this would be 4 - 10  
If desired, this value can be set to 3 to emulate the original game

### MaxChantTimeBurdenMs

When players cast spells, each spell line takes approximately 1000ms to chant. The amount of time a spell will take to cast can be predicted
to be 1000ms * (NumSpellLines).

Due to latency and jitter, players will often cast spells for slightly more or less than the expected amount of time. With big latency
spikes, the observed amount of time could be far off than the expected value. To be able to tolerate this while also prohibiting "speed
casting", the server will allow spell casts that occur too quickly and add up a time burden.

Each time a player casts a spell that completes faster than expected, the difference in time will be added to the time burden. This burden
will accumulate with every consecutive spell that occurs too quickly, and be subtracted from if a spell completes too slowly. The time
burden will also decrease while not casting spells.

If the time burden exceeds MaxChantTimeBurdenMs, the spell cast will be ignored.  
A good range of values for this setting would be 500 - 1500, with lower values being more strict.

### ProhibitF5Walk

When this is enabled it will prevent players from utilizing refreshing(F5) to walk faster.

### ProhibitItemSwitchWalk

When this is enabled it will prevent players from utilizing item switching to walk faster.

### ProhibitSpeedWalk

When this is enabled it will prevent players from utilizing more nefarious methods to walk faster.

### RefreshIntervalMs

This is the quickest interval in milliseconds that players will be allowed to refresh their client. This will not include refreshes utilized
by the server, such as for refreshing a player's position if they walk into a wall.  
A good value here would be 1000

## Example WorldOptions

```json
    "WorldOptions": {
      "Port": 4202,
      "LoginRedirect": {
        "HostName": "localhost",
        "Port": 4201
      },
      "RefreshIntervalMs": 1000,
      "TradeRange": 4,
      "DropRange": 4,
      "PickupRange": 4,
      "UpdatesPerSecond": 30,
      "SaveIntervalMins": 5,
      "MaxGoldHeld": 500000000,
      "MinimumMonsterAc": -99,
      "MaximumMonsterAc": 100,
      "MinimumAislingAc": -90,
      "MaximumAislingAc": 100,
      "MaxLevel": 30,
      "MaxActionsPerSecond": 5,
      "AislingAssailIntervalMs": 1500,
      "ProhibitF5Walk": true,
      "ProhibitItemSwitchWalk": true,
      "ProhibitSpeedWalk": true,
      "MaxChantTimeBurdenMs": 1500
    }
```