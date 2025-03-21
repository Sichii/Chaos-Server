# Loot Tables

Loot Tables in Chaos are used to determine the items that are dropped when a creature is killed. Loot tables can be
specified per-monster, or per-monster-per-mapInstance. Loot tables are not intended to be edited in code, only in json.

## How do I create them?

By default, loot tables should be created in `Data\Configuration\LootTables`. Configuration of how loot tables
are loaded can be found in `appsettings.json` at `Options:LootTableCacheOptions`.

Loot tables are a collection of loot drops

Loot tables are initially serialized into [LootTableSchema](<xref:Chaos.Schemas.Content.LootTableSchema>) before being
mapped to a [LootTable](<xref:Chaos.Collections.LootTable>). The schema object is mapped via
the [LootTableMapperProfile](<xref:Chaos.Services.MapperProfiles.LootTableMapperProfile>).

Loot drops are initially serialized into [LootDropSchema](<xref:Chaos.Schemas.Data.LootDropSchema>) before being mapped
to a [LootDrop](<xref:Chaos.Models.Data.LootDrop>). The schema object is mapped via
the [LootTableMapperProfile](<xref:Chaos.Services.MapperProfiles.LootTableMapperProfile>).

See [LootDropSchema](<xref:Chaos.Schemas.Data.LootDropSchema>) for a list of all configurable properties with
descriptions.

Keep in mind, loot tables are a collection of loot drops. This is expressed in json as a json array of loot drop
objects.

## How do I use them?

### Attach loot tables to monsters

Loot tables are not used directly in code. Instead, they reference by their key. You can attach loot tables to monsters
directly by adding the loot table key to the monster's `LootTableKeys` property in json.

#### Example monster json

[!code-json[](../../Data/Configuration/Templates/Monsters/common_rat.json)]

### Attach loot tables to monster spawns

Alternatively, you can attach loot tables to monsters on a per-mapInstance basis by adding the loot table key to the
monster spawn json.

#### Example monster spawn json

[!code-json[](../../Data/Configuration/MapInstances/test1/monsters.json)]

## Mode

Loot tables can be configured to drop items in one of two modes: `ChancePerItem` or `PickSingleOrDefault`.

### ChancePerItem

When in `ChancePerItem` mode, each item in the loot table has a chance to drop. This means that multiple items can drop
at the same time, and each roll is treated individually.

### PickSingleOrDefault

When in `PickSingleOrDefault` mode, at most only 1 item may drop. First, a check is made to determine if any item WOULD
drop if all drops were rolled individually. If this check succeeds, a single item is picked using the drop chances as
weights. If this check fails, no item is dropped.

### Mode combinations

Each loot table has it's own mode, but monsters can have multiple loot tables. Keep in mind that each loot table is
treated separately. If you have 5 loot table keys on a monster that are all in `PickSingleOrDefault` mode, you could
potentially drop 5 items at once.