namespace MiniCdp.Domain.Customers;

// Concept model only in Phase 0. No redemption rules are assumed.
public sealed record Voucher(
    string VoucherId,
    string CustomerId,
    string Name,
    DateOnly? RedeemedDate);
