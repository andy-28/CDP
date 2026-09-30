# Company Data Discovery Template

> Template only. Do not add credentials, connection strings, internal IPs, production exports, customer data, or confidential values to this repository.

For every statement, assign exactly one evidence status:

- **Confirmed** — supported by authoritative documentation, database metadata, verified SQL behavior, or a named domain owner.
- **Inferred** — plausible from evidence but not yet verified.
- **Unknown** — unanswered or lacking sufficient evidence.

Never promote an Inferred or Unknown relationship to Confirmed without recording evidence.

## Domain

- Business concept:
- Owner / subject-matter expert:
- Scope:
- Evidence status: Unknown
- Evidence / source reference:

## Schema

- Schema purpose:
- Access mode:
- Environment:
- Evidence status: Unknown
- Evidence / source reference:

## Tables

| Logical purpose | Physical table | Grain | Evidence status | Evidence / source |
| --- | --- | --- | --- | --- |
| Unknown | Unknown | Unknown | Unknown | |

## Important Columns

| Table | Column | Logical meaning | Data type | Nullable | Example classification | Evidence status |
| --- | --- | --- | --- | --- | --- | --- |
| Unknown | Unknown | Unknown | Unknown | Unknown | Do not paste production values | Unknown |

## Relationships

| From | To | Join keys | Cardinality | Required filters | Evidence status | Evidence / source |
| --- | --- | --- | --- | --- | --- | --- |
| Unknown | Unknown | Unknown | Unknown | Unknown | Unknown | |

## Identity Mapping

- Candidate customer identifiers:
- Golden / master identifier:
- Duplicate handling:
- Household vs individual rules:
- Anonymous-to-known rules:
- Evidence status: Unknown
- Evidence / source reference:

## Business Rules

| Rule | Scope | Edge cases | Evidence status | Evidence / owner |
| --- | --- | --- | --- | --- |
| Unknown | Unknown | Unknown | Unknown | |

## Known Pitfalls

- Time zones:
- Currency:
- Returns / cancellations:
- Test accounts:
- Deleted / merged customers:
- Late-arriving data:
- Evidence status: Unknown

## Confirmed

Record only verified facts with their evidence.

## Inferred

Record hypotheses that require verification and the reason for each inference.

## Unknown

Record open questions, expected owner, and next verification action.

## Discovery Session Checklist

- [ ] Review representative existing SQL without copying confidential values
- [ ] Inspect metadata and keys
- [ ] Confirm table grain before joins
- [ ] Confirm customer identity rules
- [ ] Confirm purchase inclusion/exclusion rules
- [ ] Confirm dates, time zones, amount, currency, returns, and brand semantics
- [ ] Compare row counts and a safely controlled sample
- [ ] Classify every conclusion as Confirmed, Inferred, or Unknown
- [ ] Keep credentials and exports outside Git
