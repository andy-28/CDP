using MiniCdp.Domain.Semantics;

namespace MiniCdp.Application.Semantics;

public interface ISemanticCatalog
{
    IReadOnlyList<FieldDefinition> Fields { get; }
    IReadOnlyList<MetricDefinition> Metrics { get; }
    IReadOnlyList<OperatorDefinition> Operators { get; }
    FieldDefinition GetField(string key);
    MetricDefinition GetMetric(string key);
}

public sealed class SemanticCatalog : ISemanticCatalog
{
    private static readonly string[] TextOperators = ["equals", "notEquals", "contains", "in"];
    private static readonly string[] ComparableOperators = ["equals", "notEquals", "greaterThan", "greaterThanOrEqual", "lessThan", "between"];

    public IReadOnlyList<FieldDefinition> Fields { get; } =
    [
        new("customer.customerId", "Customer ID", "Customer", SemanticDataType.String, TextOperators,
            "Generic mock customer identity; real identity mapping remains unknown.",
            CanAggregate: true,
            AvailableAggregations: [AggregationFunction.Count, AggregationFunction.DistinctCount],
            Category: SemanticCategory.Customer),
        new("customer.memberLevel", "Member Level", "Customer", SemanticDataType.String, TextOperators,
            CanGroup: true, Category: SemanticCategory.Customer),
        new("customer.birthday", "Birthday", "Customer", SemanticDataType.Date, ComparableOperators,
            Category: SemanticCategory.Customer),
        new("purchase.brand", "Brand", "Purchase", SemanticDataType.String, TextOperators,
            CanGroup: true, Category: SemanticCategory.Purchase),
        new("purchase.amount", "Purchase Amount", "Purchase", SemanticDataType.Number, ComparableOperators,
            "Amount of one mock purchase record; physical grain remains unknown.",
            CanAggregate: true,
            AvailableAggregations: [AggregationFunction.Sum, AggregationFunction.Average, AggregationFunction.Minimum, AggregationFunction.Maximum],
            Category: SemanticCategory.Purchase),
        new("purchase.month", "Purchase Month", "Purchase", SemanticDataType.Date, [],
            "Calendar month derived from the mock purchase date.",
            CanFilter: false, CanGroup: true, Category: SemanticCategory.Time),
        new("purchase.customerPeriodAmount", "Customer Period Purchase Amount", "Purchase", SemanticDataType.Number, [],
            "Customer purchase total after the analysis time range and filters are applied.",
            CanFilter: false, CanAggregate: true,
            AvailableAggregations: [AggregationFunction.Sum], Category: SemanticCategory.Metric),
        new("purchase.totalAmount", "Total Purchase", "Purchase", SemanticDataType.Number, ComparableOperators,
            "Sum of all purchases for the customer in the Phase 0 mock source.", Category: SemanticCategory.Purchase),
        new("purchase.orderCount", "Order Count", "Purchase", SemanticDataType.Number, ComparableOperators,
            Category: SemanticCategory.Purchase),
        new("purchase.lastPurchaseDate", "Last Purchase Date", "Purchase", SemanticDataType.Date, ComparableOperators,
            Category: SemanticCategory.Purchase),
        new("customer.daysSinceLastPurchase", "Days Since Last Purchase", "Customer", SemanticDataType.NumberOfDays, ComparableOperators,
            Category: SemanticCategory.Customer)
    ];

    public IReadOnlyList<MetricDefinition> Metrics { get; } =
    [
        new("customer.uniqueCount", "Unique Customers", SemanticDataType.Number,
            AggregationFunction.DistinctCount, "customer.customerId",
            "Distinct mock customer identifiers within each result group."),
        new("purchase.averageOrderValue", "Average Order Value", SemanticDataType.Number,
            AggregationFunction.Average, "purchase.amount",
            "Average amount per mock purchase record; an order-grain assumption only for the Phase 0.5 mock source.")
    ];

    public IReadOnlyList<OperatorDefinition> Operators { get; } =
    [
        new("equals", "equals"),
        new("notEquals", "does not equal"),
        new("greaterThan", ">"),
        new("greaterThanOrEqual", ">="),
        new("lessThan", "<"),
        new("contains", "contains"),
        new("in", "in"),
        new("between", "between", 2)
    ];

    public FieldDefinition GetField(string key) => Fields.FirstOrDefault(field => field.Key == key)
        ?? throw new SegmentValidationException($"Unknown semantic field '{key}'.");

    public MetricDefinition GetMetric(string key) => Metrics.FirstOrDefault(metric => metric.Key == key)
        ?? throw new SemanticValidationException($"Unknown semantic metric '{key}'.");
}

public sealed class SegmentValidationException(string message) : Exception(message);
public sealed class SemanticValidationException(string message) : Exception(message);
