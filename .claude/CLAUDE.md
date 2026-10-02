# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

City Info demo REST API (ASP.NET Core, SQL Server via EF Core) exposing USA cities, states, and their points of interest. The solution lives in `CityInfoAPI/` (`CityInfo.sln`); the repo root holds only `README.md` (endpoint docs, release table) and the PR template. All projects target `net10.0` (the upgrade was done on the `upgrade-net10` branch; stale `bin/Debug/net8.0` output folders may still exist locally).

## Commands

Run from `CityInfoAPI/`:

```
dotnet build CityInfo.sln
dotnet run --project CityInfoAPI.Web          # Scalar API reference at https://localhost:7024/scalar/v1 (OpenAPI doc at /openapi/v1.json)
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
- **Versioning**: Asp.Versioning with URL segment (`/v{version}/...`), default 1.0; there is one OpenAPI document (`v1`), registered with the core `builder.Services.AddOpenApi("v1", ...)` (not `AddApiVersioning().AddOpenApi()`) because only the core call is intercepted by the XML comment source generator, which picks up the XML doc comments on controllers and on DTOs in referenced projects (so those comments matter; `CS1591` is suppressed in `Program.cs`). The resulting Asp.Versioning analyzer warnings AV0029/AV0030 are suppressed on purpose via `NoWarn` in `CityInfoAPI.Web.csproj`. Adding an API v2 needs its own `AddOpenApi("v2", ...)` plus a matching `options.AddDocument(...)` in the Scalar setup.
- **Content negotiation**: Newtonsoft.Json is the JSON formatter, XML formatters are enabled, and unsupported `Accept` types return 406 (`ReturnHttpNotAcceptable`). Responses use the `5MinuteCacheProfile` with `UseResponseCaching`.
- **Auth**: all endpoints require a JWT bearer token, issued by `AuthenticationController` (hardcoded demo user validation).
- **Health check**: `/api/health` includes a DbContext check.
- `Program.cs` maps the OpenAPI document (`MapOpenApi`) and the Scalar UI (`MapScalarApiReference`), and the developer exception page and `UseStatusCodePages` (ProblemDetails bodies for 401/404 etc.) are enabled in every environment by design (demo).
- Known issue: with `Accept: application/xml`, cities with points of interest (`includePointsOfInterest=true`), `/pointsofinterest` and `/cities/fields` return an empty 500 (the DTO graph has a cycle that the XML DataContract serializer can't handle, and `/cities/fields` returns dynamic objects). JSON is unaffected.

## Tests

xUnit + Moq in `CityInfoAPI.Test/Tests/` (controller, service, object, parameter, and metadata tests). `SetUpAutoMapper.cs` provides the shared AutoMapper config. Tests run against mocks or in-memory data, so no database is needed.
