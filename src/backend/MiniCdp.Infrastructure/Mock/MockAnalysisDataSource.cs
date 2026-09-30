using MiniCdp.Application.Abstractions;
using MiniCdp.Application.Analysis;
using MiniCdp.Domain.Analysis;
using MiniCdp.Domain.Customers;
using MiniCdp.Domain.Semantics;

namespace MiniCdp.Infrastructure.Mock;

public sealed class MockAnalysisDataSource : IAnalysisDataSource
{
    // DEMO / MOCK DATA. For this adapter only, one Purchase record represents one order.
    private static readonly IReadOnlyList<Purchase> Purchases =
    [
        new("DEMO-A01", new DateOnly(2026, 1, 5), 1200m, "BRAND_A", "Mock Item 01"),
        new("DEMO-A01", new DateOnly(2026, 1, 18), 800m, "BRAND_B", "Mock Item 02"),
        new("DEMO-A02", new DateOnly(2026, 1, 22), 3000m, "BRAND_A", "Mock Item 03"),
        new("DEMO-A03", new DateOnly(2026, 2, 2), 5000m, "BRAND_B", "Mock Item 04"),
        new("DEMO-A01", new DateOnly(2026, 2, 14), 2000m, "BRAND_A", "Mock Item 05"),
        new("DEMO-A04", new DateOnly(2026, 3, 9), 7000m, "BRAND_C", "Mock Item 06"),
        new("DEMO-A02", new DateOnly(2026, 3, 27), 2500m, "BRAND_B", "Mock Item 07"),
        new("DEMO-A05", new DateOnly(2026, 4, 6), 900m, "BRAND_A", "Mock Item 08"),
        new("DEMO-A01", new DateOnly(2026, 4, 21), 4100m, "BRAND_C", "Mock Item 09"),
        new("DEMO-A03", new DateOnly(2026, 5, 16), 6200m, "BRAND_A", "Mock Item 10"),
        new("DEMO-A06", new DateOnly(2026, 6, 11), 1500m, "BRAND_B", "Mock Item 11"),
        new("DEMO-A02", new DateOnly(2026, 7, 24), 3500m, "BRAND_A", "Mock Item 12"),
        new("DEMO-A04", new DateOnly(2026, 8, 8), 8000m, "BRAND_B", "Mock Item 13"),
        new("DEMO-A05", new DateOnly(2026, 8, 19), 999m, "BRAND_D", "Mock Item 14")
    ];

    public Task<AnalysisResult> ExecuteAsync(AnalysisDefinition definition, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var filtered = Purchases
            .Where(purchase => purchase.PurchaseDate >= definition.TimeRange.From &&
                               purchase.PurchaseDate <= definition.TimeRange.To)
            .Where(purchase => definition.Filters.All(filter => Matches(filter, purchase)))
            .ToList();

        var wantsUniqueCustomers = definition.Metrics.Contains("customer.uniqueCount");
        var wantsAverageOrderValue = definition.Metrics.Contains("purchase.averageOrderValue");
        var rows = filtered
            .GroupBy(purchase => new { purchase.PurchaseDate.Year, purchase.PurchaseDate.Month })
            .OrderBy(group => group.Key.Year)
            .ThenBy(group => group.Key.Month)
            .Select(group => new AnalysisRow(
                $"{group.Key.Year:D4}-{group.Key.Month:D2}",
                wantsUniqueCustomers ? group.Select(purchase => purchase.CustomerId).Distinct().Count() : null,
                wantsAverageOrderValue ? decimal.Round(group.Average(purchase => purchase.Amount), 2) : null))
            .ToList();

        var customerTotals = filtered.GroupBy(purchase => purchase.CustomerId)
            .Select(group => new { CustomerId = group.Key, Total = group.Sum(purchase => purchase.Amount) })
            .ToList();
        var distribution = (definition.Buckets ?? [])
            .SelectMany(bucket => bucket.Ranges.Select(range =>
            {
                var matches = customerTotals.Where(item => InRange(item.Total, range)).ToList();
                return new BucketResultRow(range.Label, matches.Count, matches.Sum(item => item.Total));
            }))
            .ToList();

        return Task.FromResult(new AnalysisResult(rows, distribution));
    }

    private static bool Matches(FilterDefinition filter, Purchase purchase)
    {
        if (filter.Field != "purchase.brand") return false;
        return filter.Operator switch
        {
            "equals" => string.Equals(purchase.Brand, filter.Value.GetString(), StringComparison.OrdinalIgnoreCase),
            "in" => filter.Value.EnumerateArray().Any(value =>
                string.Equals(purchase.Brand, value.GetString(), StringComparison.OrdinalIgnoreCase)),
            _ => false
        };
    }

    private static bool InRange(decimal amount, BucketRange range) =>
        (range.MinimumInclusive is null || amount >= range.MinimumInclusive) &&
        (range.MaximumExclusive is null || amount < range.MaximumExclusive);
}
