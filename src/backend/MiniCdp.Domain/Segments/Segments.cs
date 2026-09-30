using MiniCdp.Domain.Semantics;

namespace MiniCdp.Domain.Segments;

public enum LogicalOperator
{
    And,
    Or
}

public sealed record Segment(
    string? SegmentId,
    string Name,
    SegmentGroup Definition);

public sealed record SegmentGroup(
    LogicalOperator Operator,
    IReadOnlyList<FilterDefinition> Rules,
    IReadOnlyList<SegmentGroup>? Groups = null);

public sealed record SegmentRule(
    string Field,
    string Operator,
    System.Text.Json.JsonElement Value) : FilterDefinition(Field, Operator, Value);
