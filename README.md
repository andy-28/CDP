# EDA Mini CDP

An **experimental Mini Customer Data Platform** for exploring Audience, Analysis, Customer 360 foundations, and a shared semantic layer without exposing database details to users.

> This repository currently uses only clearly labelled **DEMO / MOCK DATA**. It contains no company schema, mapping, credentials, or customer data.

## Phase 0 vertical slice

The Phase 0 Audience flow is:

```text
Vue Audience Builder
  -> Segment Definition JSON
  -> ASP.NET Core API
  -> Segment Engine
  -> ICustomerDataSource
  -> MockCustomerDataSource
  -> Audience Preview
```

The UI starts with `Brand equals NIKE` and `Total Purchase >= 5000`. Conditions can be added or removed and combined using ALL (AND) or ANY (OR).

Phase 0.5 adds an Analysis workspace based on a validated, sanitized brand-spending use case:

```text
Analysis Definition
  -> Analysis Service + semantic validation
  -> IAnalysisDataSource
  -> MockAnalysisDataSource
  -> Monthly metrics + spending distribution
```

The analysis slice supports an inclusive time range, synthetic brands, grouping by month, unique customer count, average order value, and configurable customer-spend buckets.

## Technology

- Backend: ASP.NET Core 8 / C#
- Frontend: Vue 3 + Vite, plain JavaScript and CSS
- Data source: in-memory mock data
- Tests: xUnit

Vue was chosen because it fits the existing skill set and the Phase 0 UI does not need a larger state-management or component framework.

## Run locally

Requirements: .NET 8 SDK or newer and Node.js 20 or newer.

Terminal 1:

```bash
dotnet run --project src/backend/MiniCdp.Api --launch-profile http
```

Terminal 2:

```bash
cd src/frontend
npm install
npm run dev
```

Open <http://localhost:5173>. Vite proxies `/api` calls to <http://localhost:5072>.

## Verify

macOS/Linux shell:

```bash
dotnet test MiniCdp.sln
dotnet build MiniCdp.sln
cd src/frontend && npm run build
```

Windows PowerShell:

```powershell
dotnet test MiniCdp.sln
dotnet build MiniCdp.sln
Push-Location src/frontend
npm ci
npm run build
Pop-Location
```

## Repository map

```text
src/backend/MiniCdp.Domain          generic entities and segment definitions
src/backend/MiniCdp.Application     semantic catalog and segment evaluation
src/backend/MiniCdp.Infrastructure  mock audience and analysis adapters
src/backend/MiniCdp.Api             HTTP composition and endpoints
src/frontend                        audience and analysis workspaces
tests/MiniCdp.Tests                 audience and analysis tests
docs                                architecture and discovery documentation
```

See [architecture](docs/architecture.md), [semantic layer](docs/semantic-layer.md), [segment engine](docs/segment-engine.md), [validated use cases](docs/use-cases), and [roadmap](docs/roadmap.md).

## Current limits

- Data is in-memory and resets with the API process.
- Segments are previewed but not persisted.
- Purchase aggregation rules are deliberately simple and documented.
- Analysis currently supports one deliberately narrow query shape; one mock purchase record is treated as one order only inside its mock adapter.
- No Oracle adapter, export, authentication, audit trail, or Customer 360 screen exists yet.
- The engine evaluates in memory; database query compilation and pushdown are future work.

Company database work must begin with [the discovery template](docs/company-data-discovery.md); unknown relationships must never be treated as facts.
Validated SQL should be reviewed with [the validated-query template](docs/data-discovery/validated-query-review-template.md) and the [knowledge promotion rule](docs/data-knowledge-promotion.md).
