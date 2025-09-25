Controllers only orchestrate calls to services. No business logic and no data access.
ViewModels belong in /Models. Domain entities belong in /Domain.
Services enforce rules and coordinate repositories.
Repositories handle persistence only.
EF Core configurations live in /Data/Configurations. Migrations live in /Data/Migrations.
Mapping helpers live in /Mapping.
