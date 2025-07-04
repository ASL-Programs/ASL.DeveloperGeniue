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

```bash
dotnet run --project src/DeveloperGeniue.CLI -- build path/to/project.csproj
```

### Run tests

```bash
dotnet test DeveloperGeniue.sln
```

Or use the CLI:

```bash
dotnet run --project src/DeveloperGeniue.CLI -- test path/to/project.csproj
```

### 3D visualization

Render a simple Three.js view of a project:

```bash
dotnet run --project src/DeveloperGeniue.CLI -- viz path/to/project.csproj
```

### Augmented reality review

Start an AR code review session (placeholder implementation):

```bash
dotnet run --project src/DeveloperGeniue.CLI -- ar path/to/project.csproj
```

### Register commit on blockchain

```bash
dotnet run --project src/DeveloperGeniue.CLI -- provenance <commit-hash>
```

### Show evolution analytics

```bash
dotnet run --project src/DeveloperGeniue.CLI -- evolution
```

### Analyze quantum readiness

Generate a simple compatibility report:

```bash
dotnet run --project src/DeveloperGeniue.CLI -- quantum path/to/project.csproj
```

### Configure cloud AI provider

Set configuration values `CloudAIProvider`, `AzureAIEndpoint`/`AzureAIKey` or `AWSAIEndpoint`/`AWSAIKey` to use cloud intelligence services.

### Secure API key storage

API keys are encrypted when stored in configuration files or the local database. On Windows, [DPAPI](https://learn.microsoft.com/dotnet/api/system.security.cryptography.protecteddata) protects the values for the current user. On other platforms, an AES key derived from the `GENIUE_PASSPHRASE` environment variable is used. The CLI and web host read this variable to access your keys.

### Supported languages

Resource files now include English (`en-US`), Azerbaijani (`az-AZ`), Turkish (`tr-TR`) and Russian (`ru-RU`). Switch languages via:

```bash
dotnet run --project src/DeveloperGeniue.CLI --lang tr-TR
```

## Repository Structure

```
src/DeveloperGeniue.Core   - Class library for core logic
src/DeveloperGeniue.CLI    - Console entry point (early phase)
src/DeveloperGeniue.AI     - AI training and analytics components
src/DeveloperGeniue.SpeechDemo - Demo console for the speech interface
tests/DeveloperGeniue.Tests - xUnit tests
DeveloperGeniue.sln        - Solution file
```

