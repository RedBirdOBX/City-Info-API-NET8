# Upgrade to .NET 10: Tasks

Plan: `.claude/plan.md`
Requirements: `.claude/upgrade-to-net-10-requirements.md`

Decisions: replace Swagger UI with **Scalar**; keep **AutoMapper 12.0.1** for now.
Working rule: one task at a time, ask before making any change.

- [x] 1. **Baseline**: run `dotnet build` and `dotnet test` on net8 and record the result. SDK 10.0.401 is already installed, so no install is needed.
  - Result: build succeeded, 0 warnings, 0 errors. Tests: 18 passed, 0 failed, 0 skipped.
- [x] 2. **Retarget projects**: set `TargetFramework` to `net10.0` in the 5 csproj files (Data, Dtos, Service, Web, Test).
- [x] 3. **Update packages** (applied; build succeeds on net10, 0 errors. Only package warning left is AutoMapper NU1903, intentionally deferred) to net10-compatible versions:
  - Microsoft.* (EF Core, EF SqlServer, JwtBearer, JsonPatch, Mvc.NewtonsoftJson, HealthChecks.EntityFrameworkCore) to 10.0.x
  - Asp.Versioning.Mvc and .ApiExplorer
  - Serilog.AspNetCore and sinks
  - System.Linq.Dynamic.Core
  - Test packages (Test.Sdk, xunit, runner, coverlet, Moq)
  - Leave AutoMapper.Extensions.Microsoft.DependencyInjection at 12.0.1
  - Check `dotnet list package --outdated` and `--vulnerable`
- [ ] 4. **Fix build breaks** from the retarget (Microsoft.OpenApi v2 types, obsolete APIs) until the solution builds clean.
- [ ] 5. **Replace Swashbuckle with built-in OpenAPI + Scalar** in `Program.cs`:
  - Remove `Swashbuckle.AspNetCore`; add `Microsoft.AspNetCore.OpenApi` and `Scalar.AspNetCore`
  - `AddOpenApi()` per API version, `MapOpenApi()`, `MapScalarApiReference()`
  - Port the bearer security scheme via a document transformer
  - Verify XML doc comments still surface; drop the manual `EnumerateFiles` loop
  - Remove the `BuildServiceProvider()` call
  - Update `launchSettings.json` `launchUrl` and README swagger links
- [ ] 6. **.NET 10 best-practice cleanup** (confirm each item first):
  - `UseEndpoints` to `MapControllers()`
  - Remove the duplicate `AddHealthChecks()` and the duplicated dev/non-dev Swagger branches
  - Collapse the duplicated Serilog config
  - Review exception handling, ProblemDetails, Newtonsoft and XML formatter setup, nullable warnings
- [ ] 7. **Tests**: run `dotnet test`, fix failures, compare to the task 1 baseline.
- [ ] 8. **Manual verification**: run the API locally, get a token, exercise endpoints in Scalar, check `/openapi/v1.json` and `/api/health`.
- [ ] 9. **Docs and deployment**: update README (versions, OpenAPI/Scalar links, release entry), update `CLAUDE.md`, confirm Azure App Service supports .NET 10.
- [ ] 10. **Follow-ups (out of scope)**: AutoMapper advisory/licensing; System.Text.Json migration.
