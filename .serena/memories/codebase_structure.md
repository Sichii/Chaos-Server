# Codebase Structure

## Solution Organization

### Main Application
- **Chaos/** - Main server application
  - `Program.cs` - Entry point with top-level statements
  - `appsettings.*.json` - Configuration files
  - `Site/` - Razor Pages web interface
  - Server components and services

### Core Libraries
- **Chaos.Collections** - Custom collection implementations
- **Chaos.Common** - Common utilities and extensions
- **Chaos.Common.Abstractions** - Common interfaces and abstractions
- **Chaos.Cryptography** - Encryption and security algorithms
- **Chaos.Cryptography.Abstractions** - Cryptography interfaces
- **Chaos.DarkAges** - Game-specific implementations
- **Chaos.Extensions.Common** - Common extension methods
- **Chaos.Extensions.Geometry** - Geometry-related extensions
- **Chaos.Geometry** - Spatial and geometric calculations
- **Chaos.Geometry.Abstractions** - Geometry interfaces
- **Chaos.IO** - Input/Output operations
- **Chaos.Messaging** - Message handling and communication
- **Chaos.Messaging.Abstractions** - Messaging interfaces
- **Chaos.MetaData** - Metadata handling
- **Chaos.Networking** - Network communication layer
- **Chaos.Networking.Abstractions** - Networking interfaces
- **Chaos.NLog.Logging** - NLog logging integration
- **Chaos.Packets** - Packet definitions and handling
- **Chaos.Packets.Abstractions** - Packet interfaces
- **Chaos.Pathfinding** - Pathfinding algorithms
- **Chaos.Pathfinding.Abstractions** - Pathfinding interfaces
- **Chaos.Schemas** - JSON schema definitions for templates
- **Chaos.Scripting** - Game scripting engine
- **Chaos.Scripting.Abstractions** - Scripting interfaces
- **Chaos.Security** - Security and authentication
- **Chaos.Security.Abstractions** - Security interfaces
- **Chaos.Storage** - Data persistence layer
- **Chaos.Storage.Abstractions** - Storage interfaces
- **Chaos.Time** - Time-related utilities
- **Chaos.Time.Abstractions** - Time interfaces
- **Chaos.TypeMapper** - Type mapping utilities
- **Chaos.TypeMapper.Abstractions** - Type mapper interfaces
- **Chaos.Wpf** - WPF utilities (for tools)
- **Chaos.Client** - Client-related code

### Data Directory
- **Data/** - Game content and configuration
  - `Configuration/Templates/` - JSON templates for game content
  - Scripts and game data files
  - Default staging directory for server

### Testing
- **Tests/** - All test projects
  - Each main project has corresponding test project
  - `Chaos.Testing.Infrastructure` - Shared test utilities
  - `Reports/` - Coverage reports output
  - Test projects use TUnit framework

### Tools
- **Tools/** - Development and administration tools
  - `Benchmarks/` - Performance benchmarking
  - `ChaosTool/` - WPF content editor for game data
  - `SchemaGenerator/` - JSON schema generation
  - `SeqConfigurator/` - Seq logging configuration
  - `WaveFunctionCollapse/` - Procedural generation tool
  - `do-release.bat` - Release automation script

### Documentation
- **docs/** - DocFX documentation
  - `_site/` - Generated documentation (gitignored)
  - API and conceptual documentation

### Build Configuration
- **Directory.Build.props** - Root build configuration
- **Directory.Packages.props** - Centralized package versions
- **global.json** - .NET SDK version configuration
- **version.json** - Nerdbank.GitVersioning configuration

### Other Files
- **Patches/** - Game patches directory
- **nupkgs/** - NuGet packages output
- **.gitignore** - Git ignore configuration
- **LICENSE.md** - AGPL-3.0 license
- **README.md** - Project readme
- **CLAUDE.md** - Claude Code assistant instructions

## Key Architectural Patterns

### Dependency Injection
- Extensive use of DI throughout the application
- Services registered in `Program.cs`
- Custom service extensions for major subsystems

### Template System
- JSON templates define game content
- Schema classes validate template structure
- Factory pattern for creating instances from templates

### Scripting Architecture
- Scripts attached to game objects via `scriptKeys`
- Components provide reusable functionality
- Functional scripts for cross-cutting concerns

### Server Architecture
- Multiple server instances (Lobby, Login, World)
- Each server type has specific responsibilities
- Shared services and utilities across servers