# AGENTS.md - ReforgedEngine Development Guidelines

## Build/Lint/Test Commands

### Building
```bash
# Build all projects
dotnet build

# Build specific project
dotnet build src/ReforgedEngine/ReforgedEngine.Core.csproj

# Build in Release mode
dotnet build --configuration Release
```

### Testing
```bash
# Run all tests
dotnet test

# Run tests for specific project
dotnet test tests/ReforgedEngine.Tests/

# Run single test method
dotnet test --filter "FullyQualifiedName=ReforgedEngine.Tests.ComponentMaskTests.EmptyMask_ContainsNothing"

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Linting/Code Analysis
```bash
# Run code analysis (if configured)
dotnet build /p:RunCodeAnalysis=true

# Format code (if dotnet-format is installed)
dotnet format
```

## Code Style Guidelines

### Language Features
- Target .NET 10.0
- Enable implicit usings (`<ImplicitUsings>enable</ImplicitUsings>`)
- Enable nullable reference types (`<Nullable>enable</Nullable>`)
- Use modern C# features (pattern matching, switch expressions, records where appropriate)

### Naming Conventions
- **Classes/Interfaces**: PascalCase (e.g., `ComponentMask`, `IComponent`)
- **Methods/Properties**: PascalCase (e.g., `CreateEntity`, `UpdateIso`)
- **Private fields**: camelCase with underscore prefix (e.g., `_nextEntityId`, `_entities`)
- **Constants**: PascalCase (e.g., `FloorHeight`)
- **Namespaces**: Match folder structure (e.g., `ReforgedEngine.Core.ECS.Components`)

### Code Structure
- Use XML documentation comments for public APIs
- Group related functionality in regions when files exceed 100 lines
- Prefer interfaces for component contracts (e.g., `IComponent`)
- Use enums for fixed sets of values (e.g., `RenderLayer`, `TiledLayerType`)

### Imports and Using Statements
- Group using statements by namespace hierarchy
- Use fully qualified names when there are conflicts
- Prefer `using static` for commonly used static classes

### Error Handling
- Use specific exception types (e.g., `FileNotFoundException`)
- Validate parameters and throw `ArgumentException`/`ArgumentNullException` for invalid inputs
- Use `TryParse` pattern for safe parsing operations
- Log errors using `System.Diagnostics.Debug.WriteLine` for development debugging

### Collections and LINQ
- Use generic collections (`List<T>`, `Dictionary<TKey, TValue>`)
- Prefer LINQ for complex queries and transformations
- Use `IReadOnlyList<T>` for exposing collections that shouldn't be modified externally

### Object Initialization
- Use object initializers for complex objects
- Prefer collection initializers for small collections
- Use named parameters for boolean flags and optional parameters

### Performance Considerations
- Avoid boxing/unboxing in hot paths
- Use `struct` for small, immutable data types (components)
- Consider memory allocation patterns in ECS systems
- Use `const` for compile-time constants

### Comments and Documentation
- Use `//` for implementation comments
- Use `///` for XML documentation on public members
- Comment complex algorithms and business logic
- Avoid redundant comments that restate what the code obviously does

### File Organization
- One class per file (except for small related classes)
- Group related files in appropriate folders
- Use consistent file naming matching class names</content>
<parameter name="filePath">D:\Projetos\MonoGame\ReforgedEngine\AGENTS.md