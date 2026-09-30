# Plan: Upgrade City Info API to .NET 10

## Context
`.claude/upgrade-to-net-10-requirements.md` asks to: upgrade to .NET 10, make all dependencies compatible, confirm .NET 10 is installed, follow .NET 10 best practices, and replace Swashbuckle/Swagger if it is no longer the default (use the OpenAPI default testing tools).

Findings:
- .NET SDK 10.0.401 and ASP.NET Core runtime 10.0.12 are already installed (requirement 3 is met; no `global.json`).
- All 5 csproj files (Data, Dtos, Service, Web, Test) target `net8.0`; packages pinned to 8.x.
- Swagger is used only in `CityInfoAPI.Web/Program.cs` (plus `launchUrl: "swagger"` in `launchSettings.json`, README links). Swashbuckle 6.5 is not the .NET 10 default; `Microsoft.AspNetCore.OpenApi` is (Microsoft.OpenApi v2, namespace `Microsoft.OpenApi`, not `.Models`).
- Decisions from the user: replace Swagger UI with **Scalar** (`Scalar.AspNetCore`); **keep AutoMapper 12.0.1** for now (note as follow-up).

## Approach
Work in small, separately verifiable tasks on branch `upgrade-net10`, one at a time, asking before each change. Tasks are listed below and will be written to `.claude/task.md` as the first step once approved (plan mode currently blocks writing it).

## Tasks (content for `.claude/task.md`)

1. [ ] **Baseline**: `dotnet build` + `dotnet test` on net8 to record current pass/fail state. Confirm SDK 10 present (`dotnet --list-sdks`); no install needed.
2. [ ] **Retarget projects**: change `TargetFramework` to `net10.0` in the 5 csproj files.
3. [ ] **Update packages** (check each against NuGet for a net10-compatible version; use context7/NuGet):
   - Microsoft.* (EF Core + SqlServer, JwtBearer, JsonPatch, Mvc.NewtonsoftJson, HealthChecks.EntityFrameworkCore) → 10.0.x
   - Asp.Versioning.Mvc / .ApiExplorer → latest compatible with net10
   - Serilog.AspNetCore and sinks → latest
   - System.Linq.Dynamic.Core → latest
   - Test: Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, coverlet, Moq → latest
   - AutoMapper.Extensions.Microsoft.DependencyInjection stays 12.0.1 (known advisory; follow-up)
   - Resolve restore/build warnings (`dotnet list package --outdated`, `--vulnerable`).
4. [ ] **Fix build breaks** after retarget (expected: Microsoft.OpenApi v2 types, any obsolete APIs), build the solution clean.
5. [ ] **Replace Swashbuckle with built-in OpenAPI + Scalar** in `Program.cs`:
   - Remove `Swashbuckle.AspNetCore`; add `Microsoft.AspNetCore.OpenApi` and `Scalar.AspNetCore`.
   - `AddOpenApi()` per API version (keeps Asp.Versioning `GroupNameFormat`), `MapOpenApi()`, `MapScalarApiReference()`.
   - Port bearer security scheme via a document transformer (replaces `AddSecurityDefinition/Requirement`).
   - XML doc comments: .NET 10 supports them through the build-time generator; verify the `CityInfoAPI*.xml` files from Dtos/Web are still surfaced, and drop the manual `EnumerateFiles` loop.
   - Remove the `BuildServiceProvider()` call (anti-pattern) used to get the version provider.
   - Update `launchSettings.json` `launchUrl` (`swagger` → `scalar/v1`) and the README swagger links.
6. [ ] **.NET 10 best-practice cleanup** in `Program.cs` (each item confirmed before applying):
   - Remove redundant `UseEndpoints` (use `app.MapControllers()`); remove duplicate `AddHealthChecks()` and the duplicated dev/non-dev Swagger branches; collapse the duplicated Serilog config.
   - Consider `AddOpenApi`/ProblemDetails/exception handler for non-dev, and moving DI registrations to extension methods (already in ToDo.md).
   - Review Newtonsoft usage (keep; JSON Patch depends on it) and the `AddXmlDataContractSerializerFormatters` setup.
   - Enable/verify nullable warnings; consider `<TreatWarningsAsErrors>` only if clean.
7. [ ] **Tests**: run `dotnet test`; fix failures; compare to the baseline from task 1.
8. [ ] **Manual verification**: run the API locally (needs `DbConnectionString` and `Authentication:*` settings), get a token, call endpoints via Scalar UI and the `.http`/Postman collection; check `/openapi/v1.json`, `/api/health`.
9. [ ] **Docs and deployment**: update README (platform versions, endpoints to OpenAPI/Scalar, release table entry e.g. 2.0.0), update `CLAUDE.md` (TFM, Swagger → Scalar), check Azure App Service/runtime setting supports .NET 10, delete stale `bin/net8.0` outputs only if the user asks.
10. [ ] **Follow-ups (not in scope)**: AutoMapper advisory/licensing, System.Text.Json migration.

## Critical files
- `CityInfoAPI/*/*.csproj` (5 files)
- `CityInfoAPI/CityInfoAPI.Web/Program.cs` (main rewrite)
- `CityInfoAPI/CityInfoAPI.Web/Properties/launchSettings.json`
- `README.md`, `.claude/CLAUDE.md`

## Verification
`dotnet build CityInfo.sln`, `dotnet test CityInfoAPI.Test`, `dotnet list package --vulnerable --outdated`, then run the web project and exercise auth + cities/POI endpoints through Scalar at `/scalar/v1`.
