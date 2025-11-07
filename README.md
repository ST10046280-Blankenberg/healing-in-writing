# Healing-In-Writing

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-5C2D91?style=for-the-badge" alt=".NET 8.0">
  <img src="https://img.shields.io/badge/Architecture-Service%20Centric-1E90FF?style=for-the-badge" alt="Service-Centric Architecture">
  <img src="https://img.shields.io/badge/Controllers-Thin-green?style=for-the-badge" alt="Thin Controllers">
  <img src="https://img.shields.io/badge/Status-Scaffolding%20Ready-orange?style=for-the-badge" alt="Scaffolding Ready">
</p>

> Keep the experience empathetic, the architecture disciplined, and the code paths explicit.

---

## Contents
- [Architecture Principles](#architecture-principles)
- [Solution Layout](#solution-layout)
- [Workflow Checklist](#workflow-checklist)
- [Getting Started](#getting-started)
- [Contribution Notes](#contribution-notes)

<details>
<summary>Layer Responsibilities Snapshot</summary>

```
UI (Views)      -> presents ViewModels only
Controllers     -> orchestrate service calls, no business rules
Services        -> enforce policies, talk to repositories, trigger mapping
Repositories    -> persist aggregates via EF Core
Domain          -> entities, value objects, invariants
```
</details>

## Architecture Principles
- Controllers orchestrate services, never holding business logic or data access.
- Services enforce domain rules and coordinate repositories and mappings.
- Repositories encapsulate persistence concerns and stay infrastructure-only.
- ViewModels live in `Models` and mirror UI needs without exposing domain internals.
- Mapping helpers under `Mapping` translate between domain types and view models.

> NOTE: When in doubt, push logic down to the service or domain layer and keep controllers declarative.

## Solution Layout
| Path | Purpose |
| --- | --- |
| `Controllers/` | Orchestration entry points that delegate to services. |
| `Services/` | Interfaces and implementations enforcing use-case workflows. |
| `Repositories/` | Persistence contracts and EF Core implementations. |
| `Domain/` | Aggregates, value objects, and supporting enums. |
| `Models/` | ViewModels prepared exclusively for UI consumption. |
| `Views/` | Razor views, layouts, and shared partials. |
| `Data/` | EF Core context, configurations, migrations, and seed data. |
| `Mapping/` | AutoMapper profiles or manual mapping utilities. |
| `Middlewares/` | Cross-cutting HTTP pipeline components. |
| `Filters/` | MVC filters for validation and authorisation concerns. |
| `wwwroot/` | Static assets such as CSS, JavaScript, and images. |

### Visual Layout Map
```
┌─────────────────────┐
│  Presentation Layer │  Views, Layouts, Shared Partials
└──────────┬──────────┘
           │ binds ViewModels
┌──────────▼──────────┐
│     Controllers     │  Thin orchestrators
└──────────┬──────────┘
           │ invokes
┌──────────▼──────────┐
│      Services       │  Business rules + policies
└──────────┬──────────┘
           │ uses
┌──────────▼──────────┐
│    Repositories     │  Persistence boundaries
└──────────┬──────────┘
           │ maps to
┌──────────▼──────────┐
│       Domain        │  Entities and value objects
└─────────────────────┘
```

## Workflow Checklist
1. Shape domain rules inside services or domain objects.
2. Apply mappings before returning ViewModels or DTOs.
3. Persist state through repositories inside a unit of work.
4. Keep Razor views thin by binding to ViewModels only.
5. Update documentation when conventions evolve.

> TIP: Validate that each checklist item is satisfied before raising a pull request.

## Getting Started
- Restore packages: `dotnet restore`
- Run the app: `dotnet run`
- Execute tests (once available): `dotnet test`

<details>
<summary>Environment Hints</summary>

- Ensure EF Core tools are installed locally for migrations.
- Configure user secrets or appsettings overrides without checking sensitive data into source control.
- Use the mapping helpers to avoid duplicating projection logic in controllers.

### Setting the Google Books API key (local)

- Run `./scripts/setup-google-books-key.sh` from the repo root and paste your Google Books API key when prompted. The script stores the key in `.NET` user secrets (no angle brackets) so the Admin → Books import feature can call the Google Books API without touching committed config files.
- If you keep a custom `DOTNET_ROOT`, export it before running the script so it can invoke the correct `dotnet` binary.
- On startup, the app now logs a warning (and skips dev seeding) whenever `ApiKeys:GoogleBooks` is missing, so check console output if the import UI reports missing data.

### CI/CD verification

- Pipelines should call `./scripts/verify-google-books-key.sh` (with `ApiKeys__GoogleBooks` or `GOOGLE_BOOKS_API_KEY` supplied via secure variables). The script exits non-zero if the key is absent, preventing deployments that would break the ISBN import flow.

</details>

## Contribution Notes
- Prefer constructor injection for controllers, services, and middleware.
- Add TODO markers to placeholders; replace them once functionality is implemented.
- Keep PR descriptions aligned with the repo PR template for consistency.

> REMINDER: Surface any cross-cutting changes (middlewares, filters, mappings) early in code review to keep the architecture coherent.
