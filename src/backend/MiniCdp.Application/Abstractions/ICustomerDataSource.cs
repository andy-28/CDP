using MiniCdp.Domain.Customers;

namespace MiniCdp.Application.Abstractions;

public interface ICustomerDataSource
{
    Task<IReadOnlyList<Customer>> GetCustomersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Purchase>> GetPurchasesAsync(CancellationToken cancellationToken = default);
}
