# Semantic Layer

The semantic layer is the stable vocabulary shared by Audience, Analysis, future Customer 360, and data adapters. It prevents physical database details from leaking into definitions or frontend code.

Each field now advertises data type, supported operators, filter/group/aggregate capabilities, available aggregations, and semantic category. Metrics separately identify their aggregation and source semantic field.

## Phase 0 catalog

| Semantic key | Type | Phase 0 meaning |
| --- | --- | --- |
| `customer.customerId` | String | Generic mock customer identity; real identity remains unknown |
| `customer.memberLevel` | String | Mock customer membership label |
| `customer.birthday` | Date | Mock customer birthday |
| `purchase.brand` | String collection | Brands appearing in any customer purchase |
| `purchase.amount` | Number | One mock purchase amount; filterable and aggregatable |
| `purchase.month` | Date | Calendar month derived from purchase date; groupable |
| `purchase.customerPeriodAmount` | Number | Customer total after the analysis period and filters; bucketable |
| `purchase.totalAmount` | Number | Sum of all mock purchases for the customer |
| `purchase.orderCount` | Number | Count of all mock purchases for the customer |
| `purchase.lastPurchaseDate` | Date | Most recent mock purchase date |
| `customer.daysSinceLastPurchase` | Number of days | Calendar days from last purchase to evaluation date |

These definitions are experimental, not statements about company data.

## Phase 0.5 metrics

| Metric key | Aggregation | Mock meaning |
| --- | --- | --- |
| `customer.uniqueCount` | Distinct count | Distinct mock customer IDs per result group |
| `purchase.averageOrderValue` | Average | Average amount per mock purchase record |

The AOV definition deliberately states its mock order-grain assumption. A real definition remains Unknown until source grain and business rules are confirmed.

## Mapping rule

Future physical mapping will follow:

```text
semantic key -> confirmed business definition -> adapter mapping -> physical source
```

Physical schema, table, column, joins, filters, null handling, and identity rules remain **Unknown** until company discovery provides evidence. They are intentionally absent from this repository.

## Adding a field

1. Define its stable semantic meaning and type.
2. Add supported operators to `SemanticCatalog`.
3. Add a resolver to `SegmentEngine` (Phase 0) or a query abstraction later.
4. Add tests for nulls, comparisons, and aggregation scope.
5. For company data, record the mapping evidence in the discovery document first.
