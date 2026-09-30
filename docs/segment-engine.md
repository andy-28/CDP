# Segment Engine

## Definition format

```json
{
  "operator": "and",
  "rules": [
    { "field": "purchase.brand", "operator": "equals", "value": "NIKE" },
    { "field": "purchase.totalAmount", "operator": "greaterThanOrEqual", "value": 5000 }
  ],
  "groups": []
}
```

Groups can contain nested groups and use `and` or `or`. The model stores no SQL.

## Operators

The Phase 0 engine implements `equals`, `notEquals`, `greaterThan`, `greaterThanOrEqual`, `lessThan`, `contains`, `in`, and `between`. The semantic catalog limits which operators are valid for each field.

For a collection field such as `purchase.brand`, `equals` means at least one value equals the requested brand; `notEquals` means no value equals it.

## Evaluation scope

Rules are evaluated independently on a customer profile. For example:

```text
purchase.brand = NIKE AND purchase.totalAmount >= 5000
```

means the customer has at least one NIKE purchase and the sum of all of that customer's purchases is at least 5000. It does **not** mean NIKE-only spend is at least 5000. Time windows, event correlation, currency, returns, cancellations, and order-line semantics are deliberately not assumed.

Production-scale query compilation, database pushdown, and performance controls are future work.
