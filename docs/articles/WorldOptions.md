# World Options

World options are changed in "appsettings.json" via section "Options:WorldOptions".  
This section is serialized into [WorldOptions](<xref:Chaos.Services.Servers.Options.WorldOptions>)

## World-Server Settings

---

### Port

The port the server will listen on. Default is 4202

### LoginRedirect

This is a redirect object that will be used to redirect players to the login server for this world when they log out.
This object consists of a hostname and port. These values should usually match up to the hostname and port specified for
the LoginOptions
section.

## General Settings

---

### MaxGroupSize

The maximum number of players that can be in a group together. If a group reaches this size, invites and invite accepts
will fail.

### SaveIntervalMins

This is the amount of time in minutes between global character saves. This is on top of players automatically saving
when they log out.

> [!WARNING]
> Do not set this value too low or it will become a burden on the server.

> [!TIP]
> A good range of values would be anywhere from 0.5 - 10

### UpdatesPerSecond

This is the number of times per second that the server will update the game state. .NET isn't great for this kind of
workload due garbage
collection and JIT recompilation/OSR.

> [!CAUTION]
> I strong suggest not going above 30, which is the default value

> [!TIP]
> If desired you can set this to 3 to emulate the original game

### F1MerchantTemplateKey

This is the template key of the merchant to display when a player presses F1. This is generally some kind of help npc

### LootDropsLockToRewardTargetSecs

This is the amount of time in seconds that loot drops will be locked to the reward target(s) of those loot drops. If
this value is not specified, loot drops will not lock at all.

### GroundItemDespawnTimeMins

This is the amount of time in minutes that ground items will remain on the ground before despawning. This applies to all
ground items, including items dropped by players.

## Channel Settings

---

A channel is a membership of players that can send messages to each other. Think global and trade chat in other games,
but
also group and guild chat in Dark Ages. These settings represent the parts of those systems that are configurable.

### DefaultChannels

This is a collection of channel names that new characters will join by default. These channels are also registered with
the channel service during startup.

| Name         | Type                                                           | Description                                  |
|--------------|----------------------------------------------------------------|----------------------------------------------|
| Name         | string                                                         | The name of the channel                      |
| MessageColor | [MessageColor](<xref:Chaos.DarkAges.Definitions.MessageColor>) | The color of messages sent from this channel |

> [!NOTE]
> Default channel names should also be added to `ChannelServiceOptions:ReservedChannelNames`

### GroupChatName

An alternative string that can be typed into the client to speak to the group chat. In Dark Ages this was "!!"

### GroupMessageColor

The color the message will be for the group chat. In Dark Ages this chat was sent over a specific channel, and that
channel had it's own color. To keep that color, specify "Default" as the color, so as not to use any color.

### GuildChatName

An alternative string that can be typed into the client to speak to the guild chat. In Dark Ages this was "!"

### GuildMessageColor

The color the message will be for the guild chat. In Dark Ages this chat was sent over a specific channel, and that
channel had it's own color. To keep that color, specify "Default" as the color, so as not to use any color.

## Aisling Settings

---

### AislingAssailIntervalMs

All creatures have an AssailInterval. AssailInterval is essentially the cooldown in milliseconds for skills marked as
assails. This value is modified by the AtkSpeedPct attribute and can be modified to be up to 5x faster or slower than
the base value. AtkSpeedPct will also scale the animation speed of the assail. This property sets the base interval for
all aislings.

> [!TIP]
> A good starting value is 1000-1500

### DropRange

This is the maximum distance from a player that they can drop items or gold on the ground.

> [!NOTE]
> A value of -1 would effectively disable dropping items  
> A value of 0 would only allow players to drop items directly beneath them  
> A value of 13 would allow players to drop items anywhere in their viewport

### MaxHeldGold

This is the maximum amount of gold a player can hold in their inventory.

### MaximumAislingAc

This is the maximum amount of AC a player can have. damage formulas can be changed, but with the default damage formula,
higher AC = more damage taken.

### MaxLevel

This is the level cap for players. Level formulas can be changed, but with the default level formula, if you reach this
level you will stop gaining experience.

### MaxAbilityLevel

This is the ability level cap for players. Ability level formulas can be changed, but with the default ability level
formula, if you reach this ability level you will stop gaining ability points. The ability system is a separate
progression system from experience levels that allows characters to continue advancing their power and learning new
skills/spells even after reaching the maximum character level.

### ClearOrangeBarTimerSecs

This is the amount of time in seconds after the last orange bar message was sent to an aisling where the server will
clear the orange bar.

### SleepAnimationTimerMins

This is the amount of time in minutes after an aisling's last action where the server will play the sleep animation.

### MinimumAislingAc

This is the minimum amount of AC a player can have. damage formulas can be changed, but with the default damage formula,
lower AC = less
damage taken.
> [!WARNING]
> With the default damage formula, AC is a percentile, so -100 AC would make you invulnerable.

### PickupRange

This is the maximum distance from a player that they pick up items or money from the ground.

> [!NOTE]
> A value of -1 would effectively disable picking up items  
> A value of 0 would only allow players to pick up items from directly beneath them  
> A value of 13 would allow players to pick up items from anywhere in their viewport

### TradeRange

This is the maximum distance from a player that they can initiate a trade with another player

> [!NOTE]
> A value of -1 would effectively disable trading  
> A value of 0 would only allow players to only trade with players on the same tile, which is generally impossible    
> A value of 13 would allow players to trade with anyone in their viewport

## Monster Settings

---

### MaximumMonsterAc

This is the maximum amount of AC a monster can have. damage formulas can be changed, but with the default damage
formula, higher AC = more damage taken.

### MinimumMonsterAc

This is the minimum amount of AC a monster can have. damage formulas can be changed, but with the default damage
formula, lower AC = less damage taken.

> [!WARNING]
> With the default damage formula, AC is a percentile, so -100 AC would make them invulnerable.

## Anti-Cheat Settings

---

### MaxActionsPerSecond

It would be bad to allow players to perform an infinite number of actions per second. Anything without a cooldown could
become a huge burden on the server. This value is used to control the maximum number of actions a player can take in a
second. An action is defined as using any spell, skill, or item. This includes equipping items.

> [!TIP]
> A good range of values for this would be 5 - 15

### MaxSkillsPerSecond

The maximum number of skills a player can use in a second. Assails do not count towards this value. This can be used to
disallow combos.

> [!TIP]
> Use a low value like 3 if you want to disallow combos, otherwise use a higher value like 30

### MaxSpellsPerSecond

The maximum number of spells a player can cast in a second.

> [!NOTE]
> This can be used to somewhat emulate the original game with a setting of 3, but keep in mind this server responds
> immediately, not in a 3updates per 1 second loop

### MaxItemsPerSecond

The maximum number of items a player can use in a second. Includes equipping items, consumables, trinkets, etc...

### MaxChantTimeBurdenMs

When players cast spells, each spell line takes approximately 1000ms to chant. The amount of time a spell will take to
cast can be predicted to be 1000ms * (NumSpellLines).

Due to latency and jitter, players will often cast spells for slightly more or less than the expected amount of time.
With big latency spikes, the observed amount of time could be far off from the expected value. To be able to tolerate
this while also prohibiting "speed casting", the server will allow spell casts that occur too quickly and add up a time
burden.

Each time a player casts a spell that completes faster than expected, the difference in time will be added to the time
burden. This burden will accumulate with every consecutive spell that occurs too quickly, and be subtracted from if a
spell completes too slowly. The time burden will also decrease while not casting spells.

If the time burden exceeds MaxChantTimeBurdenMs, the spell cast will be ignored.

> [!TIP]
> A good range of values for this setting would be 500 - 1500, with lower values being more strict.

### ProhibitF5Walk

When this is enabled it will prevent players from utilizing refreshing(F5) to walk faster.

### ProhibitItemSwitchWalk

When this is enabled it will prevent players from utilizing item switching to walk faster.

### ProhibitSpeedWalk

When this is enabled it will prevent players from utilizing more nefarious methods to walk faster.

### RefreshIntervalMs

This is the quickest interval in milliseconds that players will be allowed to refresh their client. This will not
include refreshes utilized by the server, such as for refreshing a player's position if they walk into a wall.

> [!TIP]
> A good value here would be 1000

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
  "SaveIntervalMins": 1,
  "MaxGroupSize": 6,
  "MaxGoldHeld": 500000000,
  "MinimumMonsterAc": -99,
  "MaximumMonsterAc": 100,
  "MinimumAislingAc": -90,
  "MaximumAislingAc": 100,
  "MaxLevel": 30,
  "MaxAbilityLevel": 30,
  "MaxActionsPerSecond": 10,
  "MaxSkillsPerSecond": 5,
  "MaxSpellsPerSecond": 3,
  "AislingAssailIntervalMs": 1500,
  "ProhibitF5Walk": true,
  "ProhibitItemSwitchWalk": true,
  "ProhibitSpeedWalk": true,
  "MaxChantTimeBurdenMs": 1500,
  "F1MerchantTemplateKey": "ackshually",
  "GroupChatName": "!!",
  "GroupMessageColor": "default",
  "DefaultChannels": [
    {
      "channelName": "!global",
      "messageColor": "yellow",
    },
    {
      "channelName": "!trade",
      "messageColor": "gainsboro",
    }
  ],
}
```