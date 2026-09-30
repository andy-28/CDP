# UC001 — Member Daily Spending Distribution

This is a generic, sanitized use case derived from a historical request that was implemented and delivered. It contains no source-system identifiers or customer data.

## Business Question

Within a selected period, how are customer-day purchase totals distributed across configurable spending ranges, and what are the customer-day count and total purchase amount in each range for each day?

## Analysis Grain

Two-stage grain:

1. Calculate purchase amount at **Customer × Calendar Day**.
2. Report at **Calendar Day × Spending Bucket**.

## Filters

- Inclusive analysis date range
- Transaction-validity rules: Unknown pending database discovery

## Dimensions

- Calendar day
- Spending bucket based on the customer-day total

## Metrics

- Customer-day count in each bucket
- Total purchase amount in each bucket

## Aggregations

- First stage: sum purchase amount by customer and calendar day
- Second stage: count customer-day records and sum their daily totals by day and bucket

## Buckets

Ordered, configurable, non-overlapping numeric ranges. Exact thresholds are request parameters and are not promoted as permanent business knowledge.

## Expected Output

| Date | Spending Bucket | Customer-Day Count | Total Purchase Amount |
| --- | --- | ---: | ---: |
| Configurable | Configurable | Calculated | Calculated |

## Semantic Concepts Required

- Purchase date and its calendar-day interpretation
- Customer identity
- Valid purchase amount
- Customer-day total purchase amount
- Spending bucket
- Count and sum aggregations

## Database Knowledge Required

All physical database knowledge is **Unknown**: schemas, tables, columns, join keys, customer identifier, purchase grain, date/time-zone rules, amount/currency semantics, cancellation and refund rules, and duplicate prevention.

## Confirmed

- The business analysis was previously implemented in SQL and delivered to a business user.
- The required logical grain is Customer × Day before bucketing.
- The output requires daily bucket counts and total purchase amount.

These are use-case facts, not confirmations about any database implementation.

## Inferred

- Bucket boundaries should be supplied with an analysis definition rather than embedded in a semantic field.
- A reusable engine needs explicit two-stage aggregation semantics.

## Unknown

- Every physical database mapping and relationship
- Authoritative customer identity
- Source transaction grain
- Inclusion/exclusion, refund, cancellation, tax, discount, currency, and time-zone rules
- Whether the count should be named customers, customer-days, or member occurrences in final business terminology
