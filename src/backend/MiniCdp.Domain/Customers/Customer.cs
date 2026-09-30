namespace MiniCdp.Domain.Customers;

public sealed record Customer(
    string CustomerId,
    string Name,
    DateOnly Birthday,
    string MemberLevel);
