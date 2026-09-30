# UC002 — Brand Spending Analysis

This is a generic, sanitized use case derived from a historical request that was implemented and delivered. All example brands in the running slice are synthetic (`BRAND_A`, `BRAND_B`, `BRAND_C`).

## Business Question

Within a selected period and set of brands, what are monthly unique customer count and average order value, and how are customer spending totals distributed across configurable ranges?

## Analysis Grain

- Monthly metrics: Calendar Month
- Distribution: Customer across the complete filtered period, then Spending Bucket

The distribution scope is explicit for Phase 0.5 and must be reconfirmed for a real request.

## Filters

- Inclusive analysis date range
- Brand `IN` a supplied list
- Transaction-validity rules: Unknown pending database discovery

## Dimensions

- Calendar month
- Spending bucket

## Metrics

- Unique customers
- Average order value
- Customer count by spending bucket
- Total purchase amount by spending bucket

## Aggregations

- Distinct count of customer identity per month
- Average of valid order amounts per month
- Sum of filtered spending per customer before bucketing
- Count of customer totals per bucket

## Buckets

Ordered, configurable, non-overlapping numeric ranges. Phase 0.5 uses mock thresholds solely to demonstrate the capability.

## Expected Output

Monthly result:

| Month | Unique Customers | Average Order Value |
| --- | ---: | ---: |
| YYYY-MM | Calculated | Calculated |

Distribution result:

| Spending Bucket | Customer Count | Total Purchase Amount |
| --- | ---: | ---: |
| Configurable | Calculated | Calculated |

## Semantic Concepts Required

- Purchase date and calendar month
- Customer identity
- Brand
- Valid order and order amount
- Unique customer count
- Average order value
- Customer period spend and spending bucket

## Database Knowledge Required

All physical database knowledge is **Unknown**: schemas, tables, columns, join keys, brand mapping, customer identifier, order grain, date/time-zone rules, amount/currency semantics, cancellation and refund rules, and duplicate prevention.

## Confirmed

- The business analysis was previously implemented in SQL and delivered to a business user.
- The request uses a date period and selected brands.
- Monthly unique customers, average order value, and a spending distribution are required outputs.

These are use-case facts, not confirmations about any database implementation.

## Inferred

- Brand is useful as a filterable and groupable semantic field.
- Purchase amount should advertise supported aggregations.
- Database-side aggregation will be necessary for realistic volume.

## Unknown

- Every physical database mapping and relationship
- Whether a source row is an order, order line, payment, or another grain
- Authoritative customer identity and brand hierarchy
- Exact AOV denominator and treatment of split orders
- Refund, cancellation, discount, tax, currency, and time-zone rules
- Whether distribution spend is monthly or across the entire requested period in each future request
