# Validated Query Review Template

> Use this template only in an approved company environment. Do not commit credentials, internal addresses, confidential values, production exports, customer data, or raw company SQL to this repository.

## Evidence Status Rules

Every finding must have exactly one status:

- **CONFIRMED** — directly supported by the validated SQL plus authoritative metadata, test evidence, or a named domain owner.
- **INFERRED** — a reasoned hypothesis that still requires verification.
- **UNKNOWN** — missing or insufficient evidence.

“Present in SQL” does not automatically mean “reusable business truth.” Record the evidence and validation source.

## Review Context

- Sanitized use-case ID:
- Business requirement reference:
- Was the query executed successfully? UNKNOWN
- Was its output actually delivered to a business user? UNKNOWN
- Reviewer / domain owner:
- Review date:

## 1. Business Domain

- Finding:
- Status: UNKNOWN
- Evidence:
- Reusable knowledge candidate: Yes / No / Unknown

## 2. Query Grain

- Input/source grain:
- Intermediate aggregation grain:
- Final output grain:
- Status: UNKNOWN
- Evidence:

## 3. Schema

- Physical identifiers: Record only in an approved non-public location
- Purpose:
- Status: UNKNOWN
- Evidence:

## 4. Tables

| Logical Role | Physical Reference | Grain | Status | Evidence |
| --- | --- | --- | --- | --- |
| Unknown | Redacted here | Unknown | UNKNOWN | |

## 5. Columns

| Logical Meaning | Physical Reference | Type / Nullability | Transformation | Status | Evidence |
| --- | --- | --- | --- | --- | --- |
| Unknown | Redacted here | Unknown | Unknown | UNKNOWN | |

## 6. Join Keys

| Left Role | Right Role | Key Meaning | Cardinality | Required Predicates | Status | Evidence |
| --- | --- | --- | --- | --- | --- | --- |
| Unknown | Unknown | Unknown | Unknown | Unknown | UNKNOWN | |

## 7. Customer Identity

- Candidate identifier:
- Master / golden identifier:
- Duplicate and merge behavior:
- Anonymous / household behavior:
- Status: UNKNOWN
- Evidence:

## 8. Filters

| Filter | Business Purpose | Parameter or Reusable Rule | Status | Evidence |
| --- | --- | --- | --- | --- |
| Unknown | Unknown | Unknown | UNKNOWN | |

## 9. Business Rules

| Rule | Scope | Exceptions | Owner | Status | Evidence |
| --- | --- | --- | --- | --- | --- |
| Unknown | Unknown | Unknown | Unknown | UNKNOWN | |

## 10. Metrics

| Metric | Numerator | Denominator | Null / Zero Handling | Status | Evidence |
| --- | --- | --- | --- | --- | --- |
| Unknown | Unknown | Unknown | Unknown | UNKNOWN | |

## 11. Aggregations

| Stage | Grouping Keys | Function | Input Grain | Output Grain | Status |
| --- | --- | --- | --- | --- | --- |
| Unknown | Unknown | Unknown | Unknown | Unknown | UNKNOWN |

## 12. Dimensions

| Dimension | Derivation | Hierarchy / Sort | Status | Evidence |
| --- | --- | --- | --- | --- |
| Unknown | Unknown | Unknown | UNKNOWN | |

## 13. Date Semantics

- Business date used:
- Timestamp source:
- Time zone:
- Inclusive/exclusive boundaries:
- Calendar/fiscal rules:
- Status: UNKNOWN
- Evidence:

## 14. Amount Semantics

- Gross/net/paid amount:
- Tax, discount, shipping:
- Currency and conversion:
- Rounding:
- Status: UNKNOWN
- Evidence:

## 15. Cancellation / Refund Logic

- Cancellation exclusion:
- Partial/full refund treatment:
- Reversal timing:
- Status: UNKNOWN
- Evidence:

## 16. Duplicate Risk

- Join fan-out risks:
- Repeated-event risks:
- Deduplication key and ordering:
- Validation counts:
- Status: UNKNOWN
- Evidence:

## 17. Known Pitfalls

| Pitfall | Impact | Detection / Mitigation | Status | Evidence |
| --- | --- | --- | --- | --- |
| Unknown | Unknown | Unknown | UNKNOWN | |

## Promotion Decision

- Validated query? UNKNOWN
- Delivered to business user? UNKNOWN
- Contains reusable knowledge? UNKNOWN
- Promote which knowledge items, if any?
- Keep which items request-local?
- Required approver:
