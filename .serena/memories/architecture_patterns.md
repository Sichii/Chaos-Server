# Chaos-Server Architecture Patterns and Guidelines

## Content System Architecture

### Template System
The server uses a JSON-based template system for defining game content:

- **Templates Location**: `Data/Configuration/Templates/`
- **Schema Classes**: Each template type has a corresponding Schema class (e.g., `ItemTemplateSchema`)
- **Factory Pattern**: Templates are instantiated via Factory interfaces (e.g., `IItemFactory`)
- **Script Integration**: Templates support `scriptKeys` array for attaching behaviors
- **Configuration**: `scriptVars` object for script-specific configuration

### Script System Architecture

#### Script Types
1. **Regular Scripts** - Implement game logic, attached via `scriptKeys`
2. **Components** - Reusable logic implementing `IComponent` or `IConditionalComponent`  
3. **Functional Scripts** - Cross-cutting concerns via `FunctionalScriptRegistry`
4. **Formulae** - Swappable calculations for damage, experience, etc.

#### Script Naming Convention
- Script class names in code: `SomeFeatureScript`
- Reference in templates: `"SomeFeature"` (without "Script" suffix)

## Service Architecture

### Dependency Injection Structure
Services are registered with specific patterns in `Program.cs`:

```csharp
// Singleton services for application lifetime
builder.Services.AddSingleton<IService, Implementation>();

// Scoped services for request lifetime
builder.Services.AddScoped<IService, Implementation>();

// Extension methods for subsystems
builder.Services.AddChaosOptions();
builder.Services.AddScripting();
builder.Services.AddStorage();
```

### Server Instance Architecture
Three distinct server types with specific responsibilities:

1. **Lobby Server (Port 4200)**
   - Server selection
   - Initial client connection

2. **Login Server (Port 4201)**
   - Authentication
   - Character selection
   - Account management

3. **World Server (Port 4202)**
   - Game world simulation
   - Player interactions
   - Content delivery

## Data Access Patterns

### Storage Abstraction
- **ISimpleCache**: In-memory caching
- **IStorage**: Persistent storage interface
- **Factories**: Create instances from templates
- **Repositories**: Data access layer

### Configuration Cascade
Configuration files load in specific order:
1. `appsettings.json` - Base configuration
2. `appsettings.logging.json` - Logging setup
3. Environment-specific:
   - Development: `appsettings.local.json`
   - Production: `appsettings.prod.json`
4. Optional: `appsettings.seq.json` (if Seq logging enabled)

## Networking Architecture

### Packet Processing
- **Chaos.Packets**: Packet definitions
- **Chaos.Networking**: Network layer implementation
- **ISocketClient**: Client connection abstraction
- **Packet Serialization**: Custom binary protocol

### Client Types
- `IChaosLobbyClient` - Lobby connections
- `IChaosLoginClient` - Login connections
- `IChaosWorldClient` - World connections

## Logging Architecture

### Structured Logging
- NLog with structured logging transformations
- Custom object transformations for game entities
- Topics-based logging (e.g., `Topics.Actions.Disconnect`)
- Seq integration for centralized logging

### Logging Patterns
```csharp
logger.WithTopics(Topics.Category.Action)
      .LogInformation("Message", args);
```

## Entity System

### Entity Hierarchy
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

### Entity Management
- Unique IDs for all entities
- Creation timestamps
- Location tracking via `ILocation`
- Sprite and visual data

## Security Patterns

### Authentication Flow
1. Client connects to Lobby
2. Redirected to Login server
3. Authentication via `AccessManager`
4. Character selection
5. Transfer to World server with session tokens

### Security Features
- Password hashing and salting
- Session management
- IP-based restrictions
- Admin access control
- Lockout mechanisms

## Performance Optimizations

### Runtime Configuration
- Server GC enabled
- Concurrent GC for better latency
- Sustained low latency mode
- High process priority

### Caching Strategies
- In-memory caches for frequently accessed data
- Expiring caches for map instances
- Template caching at startup

## Extension Points

### Adding New Content
1. Create Schema class in `Chaos.Schemas`
2. Create Factory interface in appropriate project
3. Implement Factory in `Chaos`
4. Register Factory in DI container
5. Create JSON templates in `Data/Configuration/Templates/`

### Adding New Scripts
1. Create Script class in `Chaos.Scripting`
2. Implement required interfaces
3. Reference in templates via `scriptKeys`
4. Configure via `scriptVars` if needed

### Adding New Commands
1. Implement `ICommand<T>`
2. Add `[Command("name", isAdmin: bool)]` attribute
3. Command automatically discovered and registered