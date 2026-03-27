# Chaos-Server Project Overview

## Project Purpose
Chaos-Server is a configurable Dark Ages private server implementation. It's a .NET 10 monorepo that provides a complete game server solution with multiple server instances for different functionalities.

## Server Architecture
- **Main Server**: Runs on Kestrel (port 5000) with optional Razor Pages site
- **Multiple Server Instances**:
  - Lobby Server: Port 4200
  - Login Server: Port 4201  
  - World Server: Port 4202

## Tech Stack
- **Framework**: .NET 10.0 with C# 14 language features
- **Web Framework**: ASP.NET Core with Kestrel
- **Frontend**: Razor Pages with runtime compilation
- **Logging**: NLog with structured logging support
- **Testing**: TUnit test framework with FluentAssertions and Moq
- **Serialization**: System.Text.Json with custom converters
- **Code Coverage**: Cobertura format with ReportGenerator for HTML reports
- **Version Control**: Git with Nerdbank.GitVersioning

## Development Environment
- **Platform**: Windows (cross-platform .NET)
- **IDE**: Visual Studio 2022+ or compatible .NET IDE
- **SDK**: .NET 10.0 SDK required
- **Build System**: MSBuild with Directory.Build.props for shared configuration

## Key Features
- Configurable game server for Dark Ages
- Multi-server architecture with separate login, lobby, and world servers
- Built-in web interface for administration
- Comprehensive scripting system for game logic
- Content templating system with JSON schemas
- Real-time packet processing and networking
- Pathfinding and geometry systems
- Security and cryptography support