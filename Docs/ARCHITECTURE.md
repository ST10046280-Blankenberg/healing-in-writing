# Architecture Overview

<p align="center">
  <img src="https://img.shields.io/badge/Controllers-Orchestrate-blue?style=flat-square" alt="Controllers Orchestrate">
  <img src="https://img.shields.io/badge/Services-Enforce%20Rules-success?style=flat-square" alt="Services Enforce Rules">
  <img src="https://img.shields.io/badge/Repositories-Persist%20Data-orange?style=flat-square" alt="Repositories Persist Data">
</p>

> Our MVC stack keeps each layer focused: controllers orchestrate calls, services apply the real rules, repositories persist data without leaking infrastructure concerns.

---

## Layers at a Glance
| Layer | Responsibility | Notes |
| --- | --- | --- |
| Controllers | Receive requests, forward work | Never own business logic or data access. |
| Services | Apply policies, coordinate repositories | Map domain data to ViewModels before returning. |
| Repositories | Persist aggregates | Only infrastructure concerns live here. |
| Domain | Defines entities and value objects | Services rely on these to express rules. |

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

## Request Flow
1. Request hits a controller action.
2. Controller calls the matching service interface.
3. Service applies rules and asks repositories for data.
4. Repository reads or writes through the data provider.
5. Service maps the result to a ViewModel for the controller to return.

## Current Folder Layout
```
Controllers/
Services/
Repositories/
Domain/
Models/
Views/
Mapping/
Middlewares/
Filters/
Data/
wwwroot/
```

> Keep the separation strict: if logic creeps into controllers or repositories, move it back into services or domain types.
