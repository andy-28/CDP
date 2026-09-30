namespace MiniCdp.Domain.Customers;

public sealed record Purchase(
    string CustomerId,
    DateOnly PurchaseDate,
    decimal Amount,
    string Brand,
    string Product);
