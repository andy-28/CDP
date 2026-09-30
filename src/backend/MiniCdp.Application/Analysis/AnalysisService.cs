using MiniCdp.Application.Abstractions;
using MiniCdp.Application.Semantics;
using MiniCdp.Domain.Analysis;

namespace MiniCdp.Application.Analysis;

public sealed class AnalysisService(IAnalysisDataSource dataSource, ISemanticCatalog catalog) : IAnalysisService
{
    public Task<AnalysisResult> RunAsync(AnalysisDefinition definition, CancellationToken cancellationToken = default)
    {
        Validate(definition);
        return dataSource.ExecuteAsync(definition, cancellationToken);
    }

    private void Validate(AnalysisDefinition definition)
    {
        if (definition.TimeRange.From > definition.TimeRange.To)
            throw new AnalysisValidationException("The analysis start date must be on or before the end date.");

        if (definition.TimeGrain != TimeGrain.Month)
            throw new AnalysisValidationException("The Phase 0.5 slice supports monthly time grain only.");

        if (definition.Dimensions.Count != 1 || definition.Dimensions[0] != "purchase.month")
            throw new AnalysisValidationException("The Phase 0.5 slice supports the purchase.month dimension only.");

        var dimension = catalog.GetField(definition.Dimensions[0]);
        if (!dimension.CanGroup)
            throw new AnalysisValidationException($"Field '{dimension.Key}' cannot be used as a dimension.");

        if (definition.Metrics.Count == 0)
            throw new AnalysisValidationException("Select at least one metric.");

        foreach (var metricKey in definition.Metrics) catalog.GetMetric(metricKey);

        foreach (var filter in definition.Filters)
        {
            var field = catalog.GetField(filter.Field);
            if (!field.CanFilter)
                throw new AnalysisValidationException($"Field '{field.Key}' cannot be filtered.");
            if (!field.SupportedOperators.Contains(filter.Operator))
                throw new AnalysisValidationException($"Operator '{filter.Operator}' is not supported for '{field.Key}'.");
            if (filter.Field != "purchase.brand" || filter.Operator is not ("in" or "equals"))
                throw new AnalysisValidationException("The Phase 0.5 analysis slice supports Brand IN or Brand equals filters only.");
        }

        foreach (var bucket in definition.Buckets ?? [])
        {
            var field = catalog.GetField(bucket.Field);
            if (!field.CanAggregate)
                throw new AnalysisValidationException($"Field '{field.Key}' cannot be bucketed.");
            if (bucket.Ranges.Count == 0)
                throw new AnalysisValidationException("A bucket definition must contain at least one range.");
            if (bucket.Ranges.Any(range => range.MinimumInclusive is not null &&
                                           range.MaximumExclusive is not null &&
                                           range.MinimumInclusive >= range.MaximumExclusive))
                throw new AnalysisValidationException("Each bucket minimum must be less than its maximum.");
            if (bucket.Ranges.Select(range => range.Label).Distinct(StringComparer.OrdinalIgnoreCase).Count() != bucket.Ranges.Count)
                throw new AnalysisValidationException("Bucket labels must be unique.");
            for (var index = 1; index < bucket.Ranges.Count; index++)
            {
                var previous = bucket.Ranges[index - 1];
                var current = bucket.Ranges[index];
                if (previous.MaximumExclusive is null || current.MinimumInclusive is null ||
                    current.MinimumInclusive < previous.MaximumExclusive)
                    throw new AnalysisValidationException("Bucket ranges must be ordered and cannot overlap.");
            }
        }
    }
}

public sealed class AnalysisValidationException(string message) : Exception(message);
