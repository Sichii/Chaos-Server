# Code Style and Conventions

## C# Language Settings
- **Target Framework**: .NET 10.0
- **Language Version**: C# 14
- **Nullable Reference Types**: Enabled (`<Nullable>enable</Nullable>`)
- **Implicit Usings**: Enabled (`<ImplicitUsings>enable</ImplicitUsings>`)
- **Documentation**: XML documentation files generated for all projects

## Coding Standards

### General Principles
- Write high-verbosity code with descriptive names
- Use explicit types over var when it improves readability
- Handle edge cases first with early returns
- Keep comments concise - explain "why" not "what"
- Follow existing patterns in neighboring code

### Naming Conventions
- **Classes/Interfaces**: PascalCase (e.g., `MapInstance`, `ISocketClient`)
- **Methods**: PascalCase (e.g., `GetRequiredService`, `AddConfiguration`)
- **Properties**: PascalCase (e.g., `RemoteIp`, `TemplateKey`)
- **Private Fields**: camelCase or _camelCase
- **Parameters/Local Variables**: camelCase (e.g., `builder`, `serverCtx`)
- **Constants**: UPPER_CASE or PascalCase
- **Interfaces**: Prefix with 'I' (e.g., `IEffect`, `ISimpleCache`)

### Project Structure Conventions
- **Abstractions**: Separate projects for abstractions (e.g., `Chaos.*.Abstractions`)
- **Tests**: Mirror main project structure in Tests folder
- **Shared Configuration**: Use Directory.Build.props for common settings
- **Package Management**: Centralized package versions in Directory.Packages.props

### Script Development Conventions
- Script keys are class names without "Script" suffix
- Scripts implement game logic, attached via `scriptKeys` in templates
- Components provide reusable script logic implementing `IComponent` or `IConditionalComponent`
- Use `scriptVars` for script configuration

### Testing Conventions
- **Framework**: TUnit with FluentAssertions for assertions
- **Mocking**: Moq with helpers in `Tests/Chaos.Testing.Infrastructure`
- **Parameterized Tests**: Use `[Arguments]` attribute
- **Formatting**: Wrap attribute stacks with `//formatter:off` and `//formatter:on`
- **Focus**: Test one method at a time in large test files
- **File Naming**: Test files named as `{ClassName}Tests.cs`

### Best Practices
- Never assume a library is available - check neighboring files or package references
- When creating new components, examine existing ones for patterns
- Always follow security best practices - never expose or log secrets
- Never commit secrets or keys to the repository
- Prefer editing existing files over creating new ones
- When editing, preserve exact indentation (tabs/spaces)
- Use existing libraries and utilities rather than reimplementing

### Documentation
- All public APIs should have XML documentation comments
- Keep documentation focused and concise
- Update documentation when changing functionality
- Do not create documentation files unless explicitly requested

### Performance Considerations
- Server runs with `ServerGarbageCollection` enabled
- `ConcurrentGarbageCollection` enabled for better latency
- GC Latency Mode set to `SustainedLowLatency`
- Process priority set to `High` for server performance

## Guardrails

- Use PowerShell for commands; do not append `| cat` to commands
- Do not introduce interactive prompts in scripts or commands
- Do not add commentary inside code solely to explain actions
- Prefer semantic code search over full directory scans
- Respect package versions pinned in `Directory.Packages.props`

### Paths to Avoid Full Scans
- `Data/**` - Large game content directory
- `**/bin/**`, `**/obj/**` - Build outputs
- `docs/_site/**` - Generated documentation
- `Tests/Reports/**` - Generated coverage reports