using System.Text.Json;
using MiniCdp.Application.Analysis;
using MiniCdp.Application.Semantics;
using MiniCdp.Domain.Analysis;
using MiniCdp.Domain.Semantics;
using MiniCdp.Infrastructure.Mock;

namespace MiniCdp.Tests;

public sealed class AnalysisServiceTests
{
    private readonly AnalysisService _service = new(new MockAnalysisDataSource(), new SemanticCatalog());

    [Fact]
    public async Task Brand_analysis_groups_unique_customers_and_average_order_value_by_month()
    {
        var result = await _service.RunAsync(CreateDefinition());

        Assert.Equal(8, result.Rows.Count);
        var january = Assert.Single(result.Rows, row => row.Period == "2026-01");
        Assert.Equal(2, january.UniqueCustomers);
        Assert.Equal(1666.67m, january.AverageOrderValue);
        Assert.Equal("DEMO / MOCK DATA", result.DataClassification);
    }

    [Fact]
    public async Task Spending_distribution_buckets_customer_period_totals()
    {
        var result = await _service.RunAsync(CreateDefinition());

        Assert.Equal([1, 1, 1, 3], result.Distribution.Select(row => row.CustomerCount));
        Assert.Equal(6, result.Distribution.Sum(row => row.CustomerCount));
    }

    [Fact]
    public async Task Invalid_time_range_is_rejected()
    {
        var definition = CreateDefinition() with
        {
            TimeRange = new TimeRange(new DateOnly(2026, 9, 1), new DateOnly(2026, 1, 1))
        };

        await Assert.ThrowsAsync<AnalysisValidationException>(() => _service.RunAsync(definition));
    }

    private static AnalysisDefinition CreateDefinition() => new(
        new TimeRange(new DateOnly(2026, 1, 1), new DateOnly(2026, 8, 31)),
        TimeGrain.Month,
        [new FilterDefinition("purchase.brand", "in", JsonDocument.Parse("[\"BRAND_A\",\"BRAND_B\"]").RootElement.Clone())],
        ["purchase.month"],
        ["customer.uniqueCount", "purchase.averageOrderValue"],
        [new BucketDefinition("purchase.customerPeriodAmount",
        [
            new("Under 1,000", null, 1000),
            new("1,000–2,999", 1000, 3000),
            new("3,000–5,999", 3000, 6000),
            new("6,000+", 6000, null)
        ])]);
}
