using System.Text.Json;

namespace MiniCdp.Domain.Semantics;

public enum SemanticDataType
{
    String,
    Number,
    Date,
    NumberOfDays
}

public enum SemanticCategory
{
    Customer,
    Purchase,
    Time,
    Metric
}

public enum AggregationFunction
{
    Sum,
    Average,
    Minimum,
    Maximum,
    Count,
    DistinctCount
}

public record FilterDefinition(
    string Field,
    string Operator,
    JsonElement Value);

public sealed record FieldDefinition(
    string Key,
    string Label,
    string Entity,
    SemanticDataType DataType,
    IReadOnlyList<string> SupportedOperators,
    string? Description = null,
    bool CanFilter = true,
    bool CanGroup = false,
    bool CanAggregate = false,
    IReadOnlyList<AggregationFunction>? AvailableAggregations = null,
    SemanticCategory Category = SemanticCategory.Purchase);

public sealed record MetricDefinition(
    string Key,
    string Label,
    SemanticDataType DataType,
    AggregationFunction Aggregation,
    string SourceField,
    string Description);

public sealed record OperatorDefinition(
    string Key,
    string Label,
    int ValueCount = 1);
