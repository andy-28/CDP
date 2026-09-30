using System.Text.Json;
using MiniCdp.Application.Segments;
using MiniCdp.Application.Semantics;
using MiniCdp.Domain.Segments;
using MiniCdp.Infrastructure.Mock;

namespace MiniCdp.Tests;

public sealed class SegmentEngineTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 30, 0, 0, 0, TimeSpan.Zero);
    private readonly SegmentEngine _engine = new(
        new MockCustomerDataSource(new FixedTimeProvider(Now)),
        new SemanticCatalog());

    [Fact]
    public async Task And_segment_returns_only_customers_matching_all_rules()
    {
        var definition = new SegmentGroup(LogicalOperator.And,
        [
            Rule("purchase.brand", "equals", "\"NIKE\""),
            Rule("purchase.totalAmount", "greaterThanOrEqual", "5000")
        ]);

        var result = await _engine.PreviewAsync(definition, DateOnly.FromDateTime(Now.UtcDateTime));

        Assert.Equal(4, result.Count);
        Assert.All(result.Customers, customer => Assert.True(customer.TotalPurchase >= 5000));
    }

    [Fact]
    public async Task Nested_or_group_is_supported()
    {
        var definition = new SegmentGroup(LogicalOperator.And,
            [Rule("customer.memberLevel", "equals", "\"VIP\"")],
            [new SegmentGroup(LogicalOperator.Or,
            [
                Rule("customer.daysSinceLastPurchase", "greaterThanOrEqual", "100"),
                Rule("purchase.totalAmount", "greaterThanOrEqual", "10000")
            ])]);

        var result = await _engine.PreviewAsync(definition, DateOnly.FromDateTime(Now.UtcDateTime));

        Assert.Equal(3, result.Count);
        Assert.Equal(["DEMO-001", "DEMO-003", "DEMO-006"], result.Customers.Select(customer => customer.CustomerId));
    }

    [Fact]
    public async Task Unknown_semantic_field_is_rejected()
    {
        var definition = new SegmentGroup(LogicalOperator.And,
            [Rule("company.fakeColumn", "equals", "\"x\"")]);

        await Assert.ThrowsAsync<SegmentValidationException>(() =>
            _engine.PreviewAsync(definition, DateOnly.FromDateTime(Now.UtcDateTime)));
    }

    private static SegmentRule Rule(string field, string operation, string json) =>
        new(field, operation, JsonDocument.Parse(json).RootElement.Clone());

    private sealed class FixedTimeProvider(DateTimeOffset value) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => value;
    }
}
