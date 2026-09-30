# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

City Info demo REST API (ASP.NET Core, SQL Server via EF Core) exposing USA cities, states, and their points of interest. The solution lives in `CityInfoAPI/` (`CityInfo.sln`); the repo root holds only `README.md` (endpoint docs, release table) and the PR template. The current branch `upgrade-net10` is for a .NET 10 upgrade, but the csproj files still target `net8.0` (old `bin/Debug/net8.0` and `net10.0` output folders both exist).

## Commands

Run from `CityInfoAPI/`:

```
dotnet build CityInfo.sln
dotnet run --project CityInfoAPI.Web          # Swagger at https://localhost:7024/swagger/index.html
dotnet test CityInfoAPI.Test
dotnet test CityInfoAPI.Test --filter "FullyQualifiedName~CityServiceTests"            # one class
dotnet test CityInfoAPI.Test --filter "FullyQualifiedName~CityServiceTests.SomeTest"   # one test
```

No linter is configured. There is no CI workflow (`.github/` only holds templates).

## Configuration

- The DB connection string is read from the `DbConnectionString` config key (env var or `launchSettings.json`/`appsettings.Development.json`), not from `ConnectionStrings`. Serilog's MSSqlServer sink uses the same key, and `Logs` table is auto-created.
- JWT settings come from `Authentication:Issuer`, `Authentication:Audience`, `Authentication:SecretForKey` (base64). Not in the committed `appsettings.json`; supply them locally.
- Other settings: `PointsOfInterestCityLimit` (max POIs per city, 20), `AppVersion` (default API version string).
- SQL scripts to create and seed tables are in `Docs/Sql-Scripts/`; a Postman collection is in `Docs/Postman/`. There are no EF migrations in use, so schema changes go through those scripts.

## Architecture

Four layered projects; dependencies flow Web → Service → Data, with Dtos shared:

- **CityInfoAPI.Data**: EF Core `CityInfoDbContext`, entities (`City`, `PointOfInterest`, `State`), repositories (`I*Repository` + implementations), and the `PropertyMapping` machinery. `CitiesMemoryRepository` (class `CityMemoryRepository`) plus `CityInfoTestEntityData` provide an in-memory implementation for tests.
- **CityInfoAPI.Dtos**: all request/response DTOs, `CityRequestParameters` (paging/filter/search/orderby/fields), and `PaginationMetaDataDto`. Entities are never returned directly.
- **CityInfoAPI.Service**: business logic (`CityService`, `PointsOfInterestService`, `StateService`, mail services). Controllers must go through services, never repositories. Services call repositories and map entity → DTO with AutoMapper.
- **CityInfoAPI.Web**: controllers, AutoMapper `Profiles/`, action filters, and all startup wiring in `Program.cs`.

Cross-cutting behavior that spans files:

- **Sorting**: `orderby` strings are translated from DTO property names to entity columns by `PropertyMappingProcessor`/`PropertyMapping` and applied with `IQueryableExtensions` (System.Linq.Dynamic.Core).
- **Response helpers** (`Web/Controllers/ResponseHelpers/`): `MetaDataUtility` and `UriLinkHelper` build the `X-CityParameters` pagination header and HATEOAS links; `HomeController` is the root links document.
- **Validation filter**: `CheckForExistingCityNameFilter` (registered as a scoped service, used via `ServiceFilter`) blocks creating a city whose name already exists in the same state.
- **Versioning**: Asp.Versioning with URL segment (`/v{version}/...`), default 1.0; Swagger docs are generated per API version, and every `CityInfoAPI*.xml` doc file in the output dir is included (so XML doc comments on controllers/DTOs matter; `CS1591` is suppressed in `Program.cs`).
- **Content negotiation**: Newtonsoft.Json is the JSON formatter, XML formatters are enabled, and unsupported `Accept` types return 406 (`ReturnHttpNotAcceptable`). Responses use the `5MinuteCacheProfile` with `UseResponseCaching`.
- **Auth**: all endpoints require a JWT bearer token, issued by `AuthenticationController` (hardcoded demo user validation).
- **Health check**: `/api/health` includes a DbContext check.
- `Program.cs` builds a temporary service provider to get `IApiVersionDescriptionProvider` before `AddSwaggerGen`; Swagger/UI and the developer exception page are enabled in every environment by design (demo).

## Tests

xUnit + Moq in `CityInfoAPI.Test/Tests/` (controller, service, object, parameter, and metadata tests). `SetUpAutoMapper.cs` provides the shared AutoMapper config. Tests run against mocks or in-memory data, so no database is needed.
