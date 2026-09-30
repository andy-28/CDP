using MiniCdp.Application.Abstractions;
using MiniCdp.Domain.Customers;

namespace MiniCdp.Infrastructure.Mock;

public sealed class MockCustomerDataSource(TimeProvider timeProvider) : ICustomerDataSource
{
    private static readonly IReadOnlyList<Customer> Customers =
    [
        new("DEMO-001", "Alex Chen", new DateOnly(1988, 4, 12), "VIP"),
        new("DEMO-002", "Jamie Lin", new DateOnly(1995, 9, 3), "Standard"),
        new("DEMO-003", "Taylor Wang", new DateOnly(1982, 1, 25), "VIP"),
        new("DEMO-004", "Morgan Lee", new DateOnly(1991, 11, 8), "Gold"),
        new("DEMO-005", "Casey Huang", new DateOnly(2000, 6, 19), "Standard"),
        new("DEMO-006", "Riley Wu", new DateOnly(1986, 2, 14), "VIP")
    ];

    public Task<IReadOnlyList<Customer>> GetCustomersAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Customers);

    public Task<IReadOnlyList<Purchase>> GetPurchasesAsync(CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        IReadOnlyList<Purchase> purchases =
        [
            new("DEMO-001", today.AddDays(-120), 8000m, "NIKE", "Air Zoom Demo"),
            new("DEMO-002", today.AddDays(-10), 3000m, "adidas", "Runner Demo"),
            new("DEMO-003", today.AddDays(-30), 12000m, "NIKE", "Pegasus Demo"),
            new("DEMO-003", today.AddDays(-180), 2000m, "LEGO", "Collector Demo"),
            new("DEMO-004", today.AddDays(-95), 6200m, "NIKE", "Trail Demo"),
            new("DEMO-005", today.AddDays(-5), 900m, "MUJI", "Daily Demo"),
            new("DEMO-006", today.AddDays(-210), 4500m, "NIKE", "Classic Demo"),
            new("DEMO-006", today.AddDays(-150), 2200m, "NIKE", "Training Demo")
        ];
        return Task.FromResult(purchases);
    }
}
