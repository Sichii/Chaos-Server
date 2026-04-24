# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Chaos-Server is a .NET 10 monorepo for a Dark Ages game server. **The game server is socket-based** and runs three TCP listeners: **Lobby (4200)**, **Login (4201)**, and **World (4202)** — clients connect directly to these. A Kestrel-hosted Razor Pages site (port 5000) is an *optional utility info-viewer* for browsing item/skill/spell/monster content from a browser; it is **not** the game server. Disabled by default; toggle via `Options:SiteOptions:EnableSite`.

## Repository Model

**This repo is a base/framework that downstream forks build their private servers on top of.** Production game content (scripts, templates, quests, monster behaviors, etc.) lives in fork repos, not here. Implications when working in this codebase:

- **"No internal callers" is not a deletion signal.** Public types under `Chaos.*` are extension surface consumed by forks you can't see. Treat them as load-bearing.
- **Public API stability matters.** Renaming, moving, or changing the signature of a public type in `Chaos.Scripting`, `Chaos.Utilities.SequenceScripter`, `Chaos.Networking`, `Chaos.Storage`, etc. is a breaking change for every fork. Prefer additive changes.
- **`Data/` in this repo is a minimal example/test set**, not a complete content drop. Forks supply their own templates, maps, scripts, and saves via the staging directory.
- **Recently-added "frameworks with no callers"** (e.g. the Quest framework in `Chaos/Scripting/Quests/`, `ScriptBuilder<T>` in `Chaos/Utilities/SequenceScripter/`) are intentionally fork-facing API, not in-progress unfinished work.

## Essential Commands

### Build and Run
```powershell
# Build solution
dotnet build Chaos.slnx

# Run server
dotnet run --project Chaos/Chaos.csproj

# Run with production configuration
dotnet run --project Chaos/Chaos.csproj --environment Production
```

### Testing
```powershell
# Test projects are TUnit + Microsoft Testing Platform — they are EXECUTABLES.
# Use `dotnet run`, NOT `dotnet test`. Filter with `--treenode-filter`, NOT `--filter`.

# Run a test project
dotnet run --project Tests/Chaos.Common.Tests/Chaos.Common.Tests.csproj -- --no-ansi

# Run a specific test class (treenode pattern: /Assembly/Namespace/Class/Method)
dotnet run --project Tests/Chaos.Tests/Chaos.Tests.csproj -- --treenode-filter "/*/*/ClassName/*"

# Run a specific test method
dotnet run --project Tests/Chaos.Tests/Chaos.Tests.csproj -- --treenode-filter "/*/*/ClassName/MethodName"

# Generate coverage on the fly for one project
dotnet run --project Tests/Chaos.Tests/Chaos.Tests.csproj -- --coverage --coverage-output coverage.cobertura.xml --coverage-output-format cobertura
```

### Code Coverage
```powershell
# Generate Cobertura XML coverage (output: **/TestResults/coverage.cobertura.xml)
dotnet build Chaos.slnx /p:CreateCoverageReport=true

# Generate HTML report (output: Tests/Reports/CoverageReport)
Tests/Reports/generateCoverageReport-auto.bat
```

### Tools
```powershell
# Seq configurator for logging setup
dotnet run --project Tools/SeqConfigurator/SeqConfigurator.csproj

# ChaosTool - WPF content editor for game data
dotnet run --project Tools/ChaosTool/ChaosTool.csproj
```

## Architecture & Structure

### Solution Layout
- **Chaos/** - Main server application (Program.cs entrypoint)
- **Chaos.*** - Core libraries and abstractions
- **Chaos.Extensions.Geometry/** - Geometry extension methods (separate from core)
- **Tests/** - TUnit test projects with FluentAssertions
- **Data/** - Game content and configuration (JSON templates, scripts)
- **Tools/** - Utility applications (SeqConfigurator, ChaosTool, SchemaGenerator, Benchmarks)
- **docs/** - DocFX documentation

### Key Architectural Components

#### Content System
Templates are JSON files in `Data/Configuration/Templates/` that define game content:
- Each template type has a Schema class (e.g., `ItemTemplateSchema`) and Factory interface (e.g., `IItemFactory`)
- Templates support `scriptKeys` (array of script class names without "Script" suffix) and `scriptVars` (configuration object)
- Content is created via factories, never directly instantiated

#### Scripting Architecture
- **Scripts** (`Chaos.Scripting`): Implement game logic, attached via `scriptKeys` in templates
- **Components** (`Chaos.Scripting.Components`): Reusable script logic implementing `IComponent` or `IConditionalComponent`
- **Functional Scripts**: Cross-cutting concerns accessed via `FunctionalScriptRegistry`
- **Formulae** (`Chaos.Formulae`): Swappable calculation logic for damage, experience, etc.
- **SequenceScripter** (`Chaos.Utilities.SequenceScripter`): Fluent builder DSL for composing entity behavior over time. `ScriptBuilder<T>` provides timed + conditional actions; `CreatureScriptBuilder<T>` adds HP-threshold actions on top. Forks use this to write monster AI and similar entity-driven behavior without hand-rolling tick logic.
- **Quest Framework** (`Chaos.Scripting.Quests`): Subclass `Quest<TStage>` (where `TStage` is your stage enum) and override `Configure(QuestBuilder<TStage> q)`. `QuestRegistry` auto-discovers all `Quest` subclasses via reflection at startup. Dialog handlers wired via `q.OnDialog(templateKey)` and a fluent `QuestStepBuilder<TStage>` chain.

#### Entity Hierarchy
```
WorldEntity
└── MapEntity
    └── InteractableEntity
        └── VisibleEntity
            ├── Door
            └── NamedEntity
                ├── GroundEntity
                │   ├── GroundItem
                │   └── Money
                └── Creature
                    ├── Aisling (Player)
                    ├── Monster
                    └── Merchant
```

#### Configuration System
Configuration files cascade based on environment:
- **Development**: `appsettings.json` → `appsettings.logging.json` → `appsettings.local.json`
- **Production**: `appsettings.json` → `appsettings.logging.json` → `appsettings.prod.json`
- **Seq Logging**: If `Logging:UseSeq=true`, also loads `appsettings.seq.json`

Key configuration sections in `appsettings.json`:
- `Options:ChaosOptions` - Core server settings, staging directory, debug flags
- `Options:LobbyOptions/LoginOptions/WorldOptions` - Server instance ports
- `Options:AccessManagerOptions` - Authentication rules, lockouts, whitelisting
- `Options:SiteOptions` - Built-in website configuration (`EnableSite`, `ShowItems`, `ShowSkills`, `ShowSpells`, `ShowMonsters`)
- `Options:AislingCommandInterceptorOptions` - In-game command settings

#### In-Game Commands
- Commands implement `ICommand<T>` with `[Command("name", requiresAdmin: bool, helpText: string?)]` attribute
- Command prefix configurable via `Options:AislingCommandInterceptorOptions:Prefix`
- Admin access: Set `"IsAdmin": true` in aisling's save JSON file

## Documentation

`docs/articles/` contains in-depth articles covering most systems. **Consult the relevant article before making non-trivial changes to a feature area** — they explain design intent, configuration knobs, and extension points that aren't always obvious from the code alone.

- **Setup & configuration:** `InitialSetup.md`, `GeneralConfiguration.md`, `LobbyOptions.md`, `LoginOptions.md`, `WorldOptions.md`, `AccessManager.md`, `Logging.md`, `JsonSchemas.md`
- **Scripting:** `Scripting.md`, `Components.md`, `FunctionalScripts.md`, `Formulae.md`
- **Game systems:** `Items.md`, `Skills.md`, `Spells.md`, `Maps.md`, `Monsters.md`, `Merchants.md`, `Dialogs.md`, `Commands.md`, `LootTables.md`, `BulletinBoards.md`, `ReactorTiles.md`, `BigFlags.md`, `Visibility.md`, `MetaData.md`
- **Tooling:** `ChaosTool.md`, `ChaosAssetManager.md`
- **Infrastructure:** `CachingAndSerialization.md`

## Testing Conventions

- Framework: TUnit with FluentAssertions
- Use `[Arguments]` for parameterized tests
- Wrap attribute stacks with `//formatter:off` and `//formatter:on`
- Focus on testing one method at a time in large test files

### Chaos.Testing.Infrastructure
- All mocks live in `Tests/Chaos.Testing.Infrastructure/Mocks/` — both Moq-based factory classes and concrete test implementations
- **Mock factories** follow a consistent pattern: static `Create()` methods returning `Mock<T>`, often with an optional `Action<Mock<T>>? setup` parameter (e.g., `MockLogger.Create<T>()`, `MockOptions.Create<T>(value)`, `MockChannelSubscriber.Create("name")`)
- **Concrete mocks** expose protected members or provide simple implementations for direct instantiation (e.g., `MockConfigurableScript`, `MockScriptBase`, `MockScripted`, `MockScriptVars`, `MockCache`, `MockStagingDirectory`)
- **MockLogger** has custom `VerifyLogEvent<T>()` and `VerifySimpleLog<T>()` extension methods that understand the project's structured `LogEvent` format — use these for log verification instead of raw Moq `Verify` calls
- **Test enums and flags** belong in `Definitions/Enums.cs` — reuse existing ones (`SampleEnum1/2`, `ColorEnum`, `SizeEnum`, `TestFeatures`, `TestPermissions`, etc.) rather than defining new ones per test
- **Mock `.Returns()` must use factory lambdas** when creating new objects — use `.Returns(() => new Mock<T>().Object)`, never `.Returns(new Mock<T>().Object)`. The non-lambda form evaluates once and returns the same singleton instance for every call, causing shared state pollution across tests

## Development Tips

- **Local Configuration**: Use `appsettings.local.json` for local overrides (gitignored)
- **Staging Directory**: Default `Data/` folder, override via `Options:ChaosOptions:StagingDirectory`
- **Debug Logging**: Enable packet logging with `LogRawPackets`, `LogReceivePacketCode`, `LogSendPacketCode` in `Options:ChaosOptions`
- **Coverage Reports**: HTML reports generate in `Tests/Reports/CoverageReport/`, XML in `**/TestResults/`
- **Script Development**: Script keys are class names without "Script" suffix, use `scriptVars` for configuration

## Important Paths to Avoid Full Scans

- `Data/**` - Large game content directory
- `**/bin/**`, `**/obj/**` - Build outputs
- `docs/_site/**` - Generated documentation
- `Tests/Reports/**` - Generated coverage reports
