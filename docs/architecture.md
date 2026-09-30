# Architecture

## Intent

Phase 0 follows **Domain First, Database Later**. The domain and application layers contain no Oracle types, SQL, schema names, or infrastructure assumptions.

```text
Frontend (Vue)
  -> CDP API (ASP.NET Core)
      -> Audience: Segment Engine -> ICustomerDataSource
      -> Analysis: Analysis Service -> IAnalysisDataSource
  -> Shared Semantic Catalog
  -> Infrastructure mock adapters
  -> In-memory DEMO / MOCK DATA
```

## Project boundaries

| Project | Responsibility | May depend on |
| --- | --- | --- |
| Domain | Customer, Purchase, concept models, segment structure, semantic definition types | Nothing else |
| Application | Use cases, semantic catalog, validation, evaluation, data-source port | Domain |
| Infrastructure | Adapter implementations and mock fixtures | Application, Domain |
| API | HTTP contracts and dependency composition | All layers |
| Frontend | Semantic-key-based UI and presentation | HTTP API only |

The frontend knows keys such as `purchase.brand`; it does not know physical storage. `ICustomerDataSource` is the seam where a future Oracle adapter can be introduced.

## Decisions

- The API exposes the semantic catalog so the builder is driven by backend-supported fields and operators.
- Segment definitions store semantic keys and values, never SQL.
- Dates use `DateOnly`; current-date calculations use `TimeProvider` so tests are deterministic.
- Mock data is generated relative to today and IDs are prefixed `DEMO-`.
- Phase 0 uses independent customer aggregates. Cross-rule event correlation is not inferred.

## ADR — Separate analysis query boundary

**Decision:** Keep `ICustomerDataSource` for customer/purchase detail used by the Phase 0 segment engine and add `IAnalysisDataSource` for aggregate analysis definitions and results.

**Why:** An analysis asks for grouped aggregates, not a customer list. Loading all detail through `ICustomerDataSource` would make the application own data-volume and query-pushdown problems. A future Oracle adapter should translate a validated `AnalysisDefinition` into a parameterized, source-appropriate aggregate query while keeping SQL inside Infrastructure.

**Constraint:** `AnalysisService` validates semantic capabilities before calling the adapter. The Phase 0.5 mock adapter supports only the UC002 slice. This is intentional and avoids pretending a generic SQL engine exists.

## Shared foundation review

```text
Audience      -> Who are the customers?
Analysis      -> What happened?
Customer 360  -> What do we know about one customer? (future)
```

All three use or will use the same Customer/Purchase concepts, semantic field keys, filter/operator definitions, and evidence-governed mappings. Audience produces customer profiles; Analysis produces grouped metrics; Customer 360 is not implemented.

## Future Oracle boundary

Oracle audience/detail access may implement `ICustomerDataSource`; aggregate analysis access may implement `IAnalysisDataSource`. Oracle SQL and physical mappings must remain inside Infrastructure. No adapter should be created until discovery evidence is documented as Confirmed, Inferred, or Unknown.
