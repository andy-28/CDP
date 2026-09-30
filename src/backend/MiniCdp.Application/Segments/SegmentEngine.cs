using System.Globalization;
using System.Text.Json;
using MiniCdp.Application.Abstractions;
using MiniCdp.Application.Semantics;
using MiniCdp.Domain.Customers;
using MiniCdp.Domain.Semantics;
using MiniCdp.Domain.Segments;

namespace MiniCdp.Application.Segments;

public interface ISegmentEngine
{
    Task<AudienceResult> PreviewAsync(SegmentGroup definition, DateOnly asOfDate, CancellationToken cancellationToken = default);
}

public sealed class SegmentEngine(ICustomerDataSource dataSource, ISemanticCatalog catalog) : ISegmentEngine
{
    public async Task<AudienceResult> PreviewAsync(
        SegmentGroup definition,
        DateOnly asOfDate,
        CancellationToken cancellationToken = default)
    {
        ValidateGroup(definition);
        var customers = await dataSource.GetCustomersAsync(cancellationToken);
        var purchases = await dataSource.GetPurchasesAsync(cancellationToken);
        var purchasesByCustomer = purchases.GroupBy(p => p.CustomerId)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<Purchase>)group.ToList());

        var matches = customers
            .Select(customer => CreateContext(customer, purchasesByCustomer.GetValueOrDefault(customer.CustomerId) ?? [], asOfDate))
            .Where(context => EvaluateGroup(definition, context))
            .Select(context => new AudienceCustomer(
                context.Customer.CustomerId,
                context.Customer.Name,
                context.Customer.MemberLevel,
                context.TotalPurchase,
                context.Purchases.Count,
                context.LastPurchaseDate,
                context.DaysSinceLastPurchase))
            .ToList();

        return new AudienceResult(matches.Count, matches);
    }

    private void ValidateGroup(SegmentGroup group)
    {
        if (group.Rules.Count == 0 && (group.Groups is null || group.Groups.Count == 0))
            throw new SegmentValidationException("A segment group must contain at least one rule or nested group.");

        foreach (var rule in group.Rules)
        {
            var field = catalog.GetField(rule.Field);
            if (!field.SupportedOperators.Contains(rule.Operator))
                throw new SegmentValidationException($"Operator '{rule.Operator}' is not supported for '{rule.Field}'.");
        }

        foreach (var nested in group.Groups ?? []) ValidateGroup(nested);
    }

    private bool EvaluateGroup(SegmentGroup group, CustomerContext context)
    {
        var results = group.Rules.Select(rule => EvaluateRule(rule, context))
            .Concat((group.Groups ?? []).Select(nested => EvaluateGroup(nested, context)));
        return group.Operator == LogicalOperator.And ? results.All(value => value) : results.Any(value => value);
    }

    private static bool EvaluateRule(FilterDefinition rule, CustomerContext context)
    {
        object? actual = rule.Field switch
        {
            "customer.customerId" => context.Customer.CustomerId,
            "customer.memberLevel" => context.Customer.MemberLevel,
            "customer.birthday" => context.Customer.Birthday,
            "purchase.brand" => context.Purchases.Select(p => p.Brand).ToArray(),
            "purchase.amount" => context.Purchases.Select(p => p.Amount).ToArray(),
            "purchase.totalAmount" => context.TotalPurchase,
            "purchase.orderCount" => context.Purchases.Count,
            "purchase.lastPurchaseDate" => context.LastPurchaseDate,
            "customer.daysSinceLastPurchase" => context.DaysSinceLastPurchase,
            _ => null
        };

        return actual switch
        {
            string[] values => EvaluateCollection(values, rule.Operator, rule.Value),
            decimal[] values => EvaluateNumberCollection(values, rule.Operator, rule.Value),
            _ => EvaluateScalar(actual, rule.Operator, rule.Value)
        };
    }

    private static bool EvaluateCollection(IEnumerable<string> actualValues, string operation, JsonElement expected) => operation switch
    {
        "equals" => actualValues.Any(value => TextEquals(value, expected.GetString())),
        "notEquals" => actualValues.All(value => !TextEquals(value, expected.GetString())),
        "contains" => actualValues.Any(value => value.Contains(expected.GetString() ?? string.Empty, StringComparison.OrdinalIgnoreCase)),
        "in" => expected.EnumerateArray().Select(value => value.GetString()).Any(item => actualValues.Any(value => TextEquals(value, item))),
        _ => false
    };

    private static bool EvaluateNumberCollection(IEnumerable<decimal> actualValues, string operation, JsonElement expected)
    {
        if (operation == "between")
        {
            var bounds = expected.EnumerateArray().Select(value => value.GetDecimal()).ToArray();
            return bounds.Length == 2 && actualValues.Any(value => value >= bounds[0] && value <= bounds[1]);
        }

        var expectedNumber = expected.GetDecimal();
        return operation == "notEquals"
            ? actualValues.All(value => !Compare(value.CompareTo(expectedNumber), "equals"))
            : actualValues.Any(value => Compare(value.CompareTo(expectedNumber), operation));
    }

    private static bool EvaluateScalar(object? actual, string operation, JsonElement expected)
    {
        if (actual is null) return operation == "notEquals";

        if (actual is string text)
        {
            return operation switch
            {
                "equals" => TextEquals(text, expected.GetString()),
                "notEquals" => !TextEquals(text, expected.GetString()),
                "contains" => text.Contains(expected.GetString() ?? string.Empty, StringComparison.OrdinalIgnoreCase),
                "in" => expected.EnumerateArray().Any(item => TextEquals(text, item.GetString())),
                _ => false
            };
        }

        if (actual is DateOnly date)
        {
            if (operation == "between")
            {
                var bounds = expected.EnumerateArray().Select(ParseDate).ToArray();
                return bounds.Length == 2 && date >= bounds[0] && date <= bounds[1];
            }
            return Compare(date.CompareTo(ParseDate(expected)), operation);
        }

        var number = Convert.ToDecimal(actual, CultureInfo.InvariantCulture);
        if (operation == "between")
        {
            var bounds = expected.EnumerateArray().Select(value => value.GetDecimal()).ToArray();
            return bounds.Length == 2 && number >= bounds[0] && number <= bounds[1];
        }
        return Compare(number.CompareTo(expected.GetDecimal()), operation);
    }

    private static bool Compare(int comparison, string operation) => operation switch
    {
        "equals" => comparison == 0,
        "notEquals" => comparison != 0,
        "greaterThan" => comparison > 0,
        "greaterThanOrEqual" => comparison >= 0,
        "lessThan" => comparison < 0,
        _ => false
    };

    private static DateOnly ParseDate(JsonElement value) => DateOnly.Parse(value.GetString()!, CultureInfo.InvariantCulture);
    private static bool TextEquals(string left, string? right) => string.Equals(left, right, StringComparison.OrdinalIgnoreCase);

    private static CustomerContext CreateContext(Customer customer, IReadOnlyList<Purchase> purchases, DateOnly asOfDate)
    {
        DateOnly? lastPurchaseDate = purchases.Count == 0 ? null : purchases.Max(p => p.PurchaseDate);
        return new CustomerContext(
            customer,
            purchases,
            purchases.Sum(p => p.Amount),
            lastPurchaseDate,
            lastPurchaseDate is null ? null : asOfDate.DayNumber - lastPurchaseDate.Value.DayNumber);
    }

    private sealed record CustomerContext(
        Customer Customer,
        IReadOnlyList<Purchase> Purchases,
        decimal TotalPurchase,
        DateOnly? LastPurchaseDate,
        int? DaysSinceLastPurchase);
}
