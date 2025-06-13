# DeveloperGeniue

DeveloperGeniue aims to become an AI‑assisted code automation ecosystem inspired by the included specification "AI-Assisted Code Automation Agent - Complete Project Specification".  The long term vision is a hybrid desktop and web application that manages C# projects, integrates with multiple AI providers, and automates common tasks such as building, testing, refactoring and package management.

This repository currently contains a minimal .NET solution that will grow into that system.

## Project Goals

* Provide an extensible architecture for analyzing and managing C# projects
* Integrate with OpenAI/Claude APIs for code analysis, refactoring and documentation
* Support dynamic configuration with no hard coded settings
* Offer both CLI and UI front-ends (initial development starts with CLI)

## Planned Early Phase Features

The specification describes a very large system.  Early development will focus on:

1. **Command line tooling** for scanning solutions and invoking simple AI tasks
2. **Project discovery** and workspace persistence
3. Basic **OpenAI integration** with placeholder settings
4. Unit test project with xUnit

Later phases will expand to the full hybrid desktop/web application, advanced package management, multilingual UI, and learning systems described in the specification.

## Setup

1. Install the [.NET 8 SDK](https://dotnet.microsoft.com/download) or newer.
2. Clone this repository.
3. Restore dependencies:
   ```bash
   dotnet restore DeveloperGeniue.sln
   ```
4. Build the solution:
   ```bash
   dotnet build DeveloperGeniue.sln
   ```

## Running the Code

Run the command line application:

```bash
dotnet run --project src/DeveloperGeniue.CLI
```

### Discover projects

List all `.csproj` files under the current directory:

```bash
dotnet run --project src/DeveloperGeniue.CLI -- scan
```

You can provide a path to scan a specific folder:

```bash
dotnet run --project src/DeveloperGeniue.CLI -- scan ../some/path
```

### Build a project

### Manage configuration

Retrieve or store CLI settings:

```bash
dotnet run --project src/DeveloperGeniue.CLI -- config get SomeKey
dotnet run --project src/DeveloperGeniue.CLI -- config set SomeKey SomeValue
```

Run `dotnet build` through the CLI:

```bash
dotnet run --project src/DeveloperGeniue.CLI -- build path/to/project.csproj

```

### Run tests

Execute `dotnet test` on a project:

```bash
dotnet run --project src/DeveloperGeniue.CLI -- test path/to/project.csproj
```

## Repository Structure

```
src/DeveloperGeniue.Core   - Class library for core logic
src/DeveloperGeniue.CLI    - Console entry point (early phase)
tests/DeveloperGeniue.Tests - xUnit tests
DeveloperGeniue.sln        - Solution file
```

