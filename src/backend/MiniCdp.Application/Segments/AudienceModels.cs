namespace MiniCdp.Application.Segments;

public sealed record AudienceCustomer(
    string CustomerId,
    string Name,
    string MemberLevel,
    decimal TotalPurchase,
    int OrderCount,
    DateOnly? LastPurchaseDate,
    int? DaysSinceLastPurchase);

public sealed record AudienceResult(
    int Count,
    IReadOnlyList<AudienceCustomer> Customers,
    string DataClassification = "DEMO / MOCK DATA");
