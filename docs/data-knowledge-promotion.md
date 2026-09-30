# Data Knowledge Promotion Rule

Validated SQL is evidence, not automatically knowledge.

```text
Raw SQL
  -> validated against the intended source and requirement?
  -> result actually delivered to a business user?
  -> contains reusable, evidenced knowledge?
  -> extract and review individual semantic facts
  -> promote approved facts to Data Knowledge
```

Stop the promotion when any gate fails. Keep evidence status as CONFIRMED, INFERRED, or UNKNOWN for each individual claim.

## Usually request-local

- One-time date ranges
- Campaign or activity codes used for one delivery
- Temporary exception lists
- Ad hoc thresholds and bucket boundaries
- Output formatting and sorting chosen for one request

These may be preserved with the use case, but should not redefine global semantics.

## Potentially reusable

- Schema relationships and verified cardinality
- Customer identity and merge mapping
- Transaction-validity business rules
- Metric definitions and aggregation grain
- Cancellation and refund logic
- Reusable joins and mandatory predicates
- Semantic-field-to-source mapping
- Date, amount, currency, and duplicate-handling semantics

## Promotion checklist

1. State one atomic knowledge claim.
2. Attach its evidence and accountable owner.
3. Classify it as CONFIRMED, INFERRED, or UNKNOWN.
4. Separate reusable rules from parameters used by one request.
5. Test the claim against at least one controlled case and relevant edge cases.
6. Record scope, grain, null behavior, exclusions, and known pitfalls.
7. Promote only reviewed CONFIRMED claims; keep all others in discovery notes.

Never copy raw SQL, physical identifiers, credentials, or data samples into public/general documentation merely to preserve the evidence trail.
