# BigFlags

BigFlags is an unlimited-capacity flags system built on `BigInteger`. Standard C# `[Flags]` enums are limited to 64
flags
(using `long` or `ulong`). Use BigFlags when you need more than 64 flags or want extensibility. Fields are automatically
assigned sequential bit indices.

> [!TIP]
> BigFlags are implemented via [BigFlags\<TMarker\>](<xref:Chaos.Common.CustomTypes.BigFlags`1>)
> and [BigFlagsValue\<TMarker\>](<xref:Chaos.Common.Abstractions.IBigFlagsValue>)

## How do I create them?

Inherit from [BigFlags\<TMarker\>](<xref:Chaos.Common.CustomTypes.BigFlags`1>) and declare static fields of
type [BigFlagsValue\<TMarker\>](<xref:Chaos.Common.Abstractions.IBigFlagsValue>). Call `Initialize()` in a static
constructor
for automatic bit index assignment:

```csharp
public sealed class PlayerPermissions : BigFlags<PlayerPermissions>
{
    public static readonly BigFlagsValue<PlayerPermissions> CanTrade;
    public static readonly BigFlagsValue<PlayerPermissions> CanPvP;
    public static readonly BigFlagsValue<PlayerPermissions> CanUseMail;
    public static readonly BigFlagsValue<PlayerPermissions> IsAdmin;
    public static readonly BigFlagsValue<PlayerPermissions> IsModerator;

    static PlayerPermissions() => Initialize();
}
```

> [!NOTE]
> `Initialize()` automatically assigns sequential bit indices (0, 1, 2, ...) and
> respects [BitIndexAttribute](<xref:Chaos.Common.Attributes.BitIndexAttribute>).
> Without calling `Initialize()`, you must manually assign each field.

## How do I use them?

```csharp
// Combine flags with | operator
var permissions = PlayerPermissions.CanTrade | PlayerPermissions.CanPvP;

// Check if flag is set
if (permissions.HasFlag(PlayerPermissions.CanTrade))
    Console.WriteLine("Can trade!");

// Check multiple flags
if (permissions.HasAllFlags(PlayerPermissions.CanTrade, PlayerPermissions.CanPvP))
    Console.WriteLine("Can trade AND PvP!");

if (permissions.HasAnyFlag(PlayerPermissions.IsAdmin, PlayerPermissions.IsModerator))
    Console.WriteLine("Is staff member!");

// Add flags
permissions = permissions.SetFlag(PlayerPermissions.IsAdmin);

// Remove flags
permissions = permissions.ClearFlag(PlayerPermissions.CanPvP);

// Toggle flags
permissions = permissions.ToggleFlag(PlayerPermissions.CanUseMail);

// Check if empty
if (permissions.IsEmpty)
    Console.WriteLine("No permissions!");

// Bitwise operators
var combined = permissions | PlayerPermissions.CanUseMail;  // OR - combine
var shared = permissions & PlayerPermissions.CanTrade;      // AND - intersect
var different = permissions ^ PlayerPermissions.CanPvP;     // XOR - symmetric difference
var inverted = ~permissions;                                 // NOT - invert all bits

// Bit enumeration
foreach (int bitIndex in permissions.GetSetBitIndices())
    Console.WriteLine($"Bit {bitIndex} is set");

// Binary string representation (for debugging)
string binary = permissions.ToBinaryString();
```

## Explicit Bit Indices

Use [BitIndexAttribute](<xref:Chaos.Common.Attributes.BitIndexAttribute>) to assign explicit bit positions:

```csharp
public sealed class ItemFlags : BigFlags<ItemFlags>
{
    [BitIndex(0)]
    public static readonly BigFlagsValue<ItemFlags> Tradeable;

    [BitIndex(5)]
    public static readonly BigFlagsValue<ItemFlags> Stackable;

    [BitIndex(100)]
    public static readonly BigFlagsValue<ItemFlags> Legendary;

    // This automatically gets bit 101 (next after highest explicit index)
    public static readonly BigFlagsValue<ItemFlags> Unique;

    static ItemFlags() => Initialize();
}
```

Or assign values manually without calling `Initialize()`:

```csharp
public sealed class CustomFlags : BigFlags<CustomFlags>
{
    public static readonly BigFlagsValue<CustomFlags> Flag1 = new(0);
    public static readonly BigFlagsValue<CustomFlags> Flag2 = new(5);
    public static readonly BigFlagsValue<CustomFlags> Flag3 = new(100);
}
```

BigFlags serialize to JSON as comma-separated flag names.

## Enum Utility Equivalency

BigFlags provides utility methods equivalent to standard enum utilities. Methods are available on both the
[BigFlags\<TMarker\>](<xref:Chaos.Common.CustomTypes.BigFlags`1>) base class and the static
[BigFlags](<xref:Chaos.Common.CustomTypes.BigFlags>) utility class:

```csharp
// Get all defined flag names
IEnumerable<string> names = PlayerPermissions.GetNames();

// Get all defined flag values
IEnumerable<BigFlagsValue<PlayerPermissions>> values = PlayerPermissions.GetValues();

// Parse flag name to value
var admin = PlayerPermissions.Parse("IsAdmin");

// Try parse (returns false if not found)
if (PlayerPermissions.TryParse("UnknownFlag", out var flag))
    Console.WriteLine("Found it!");

// Check if flag name exists
bool exists = PlayerPermissions.IsDefined("CanTrade");

// Get name from value
string? name = PlayerPermissions.GetName(PlayerPermissions.CanTrade);
// Returns: "CanTrade"
```

## Best Practices

> [!TIP]
> Make BigFlags types sealed

- Use descriptive names (they appear in JSON)
- Group related flags together
- Use `[BitIndex]` for flags that shouldn't change position
- Use `None` for empty checks

## Example

```csharp
public sealed class QuestFlags : BigFlags<QuestFlags>
{
    public static readonly BigFlagsValue<QuestFlags> Started;
    public static readonly BigFlagsValue<QuestFlags> Completed;
    public static readonly BigFlagsValue<QuestFlags> Repeatable;
    public static readonly BigFlagsValue<QuestFlags> Daily;
    public static readonly BigFlagsValue<QuestFlags> Weekly;

    static QuestFlags() => Initialize();
}

// Add quest flags to an Aisling
aisling.Trackers.BigFlags.AddFlag(QuestFlags.Started);
aisling.Trackers.BigFlags.AddFlag(QuestFlags.Repeatable);
aisling.Trackers.BigFlags.AddFlag(QuestFlags.Daily);

// Check quest status - retrieve flags then check
var questFlags = aisling.Trackers.BigFlags.GetFlag<QuestFlags>();

if (questFlags.HasFlag(QuestFlags.Started) && !questFlags.HasFlag(QuestFlags.Completed))
    Console.WriteLine("Quest is active!");

// Or check directly on the collection
if (aisling.Trackers.BigFlags.HasFlag(QuestFlags.Repeatable))
    Console.WriteLine("Quest can be repeated!");
```