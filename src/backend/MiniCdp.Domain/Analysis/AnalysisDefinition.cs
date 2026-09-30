using MiniCdp.Domain.Semantics;

namespace MiniCdp.Domain.Analysis;

public enum TimeGrain
{
    Day,
    Month
}

public sealed record TimeRange(DateOnly From, DateOnly To);

public sealed record BucketRange(
    string Label,
    decimal? MinimumInclusive,
    decimal? MaximumExclusive);

public sealed record BucketDefinition(
    string Field,
    IReadOnlyList<BucketRange> Ranges);

public sealed record AnalysisDefinition(
    TimeRange TimeRange,
    TimeGrain TimeGrain,
    IReadOnlyList<FilterDefinition> Filters,
    IReadOnlyList<string> Dimensions,
    IReadOnlyList<string> Metrics,
    IReadOnlyList<BucketDefinition>? Buckets = null);
