# Essential Development Commands

**NOTE: This project uses `.slnx` format (not `.sln`)**

## Build and Run Commands

### Build Solution
```powershell
# Build entire solution
dotnet build Chaos.slnx

# Build with code coverage support
dotnet build Chaos.slnx /p:CreateCoverageReport=true
```

### Run Server
```powershell
# Run server (default Development profile)
dotnet run --project Chaos/Chaos.csproj

# Run with specific configuration profile
dotnet run --project Chaos/Chaos.csproj --launch-profile "Chaos - Prod"
```

## Testing Commands

### Run Tests
```powershell
# Run all tests
dotnet test Chaos.slnx --nologo

# Run single test project
dotnet test Tests/Chaos.Common.Tests/Chaos.Common.Tests.csproj --nologo

# Run specific test by name
dotnet test Chaos.slnx --filter "FullyQualifiedName~TestName" --nologo

# Run tests from specific class
dotnet test Chaos.slnx --filter "FullyQualifiedName~ClassName" --nologo
```

### Code Coverage
```powershell
# Generate Cobertura XML coverage (output: **/TestResults/coverage.cobertura.xml)
dotnet build Chaos.slnx /p:CreateCoverageReport=true

# Generate HTML coverage report (output: Tests/Reports/CoverageReport)
Tests/Reports/generateCoverageReport-auto.bat
```

## Tool Commands

### Seq Configurator (Logging Setup)
```powershell
dotnet run --project Tools/SeqConfigurator/SeqConfigurator.csproj
```

### ChaosTool (WPF Content Editor)
```powershell
dotnet run --project Tools/ChaosTool/ChaosTool.csproj
```

## Windows System Commands

### File and Directory Operations
```powershell
# List directory contents
dir
# or
ls  # PowerShell alias

# Change directory
cd <path>

# Create directory
mkdir <directory_name>

# Remove directory
rmdir /s /q <directory_name>
# or in PowerShell
Remove-Item -Recurse -Force <directory_name>

# Copy files
copy <source> <destination>
# or in PowerShell
Copy-Item <source> <destination>

# Move/Rename files
move <source> <destination>
# or in PowerShell
Move-Item <source> <destination>

# Delete files
del <file>
# or in PowerShell
Remove-Item <file>
```

### Search and Find
```powershell
# Find files
dir /s /b *pattern*
# or in PowerShell
Get-ChildItem -Recurse -Filter "*pattern*"

# Search in files (PowerShell)
Select-String -Path "*.cs" -Pattern "search_term"

# Find string in files (cmd)
findstr /s /i "search_term" *.cs
```

### Git Commands
```powershell
# Status
git status

# Stage changes
git add .
git add <file>

# Commit
git commit -m "message"

# Push/Pull
git push
git pull

# Branch operations
git branch
git checkout <branch>
git checkout -b <new-branch>

# View logs
git log --oneline -n 10
```

## Configuration Notes
- Local configuration: Use `appsettings.local.json` for local overrides (gitignored)
- Production config: `appsettings.prod.json` is used in Release mode
- Seq logging: Enable with `"Logging:UseSeq": true` in config
- Debug logging: Enable packet logging in `Options:ChaosOptions`