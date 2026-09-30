namespace MiniCdp.Domain.Customers;

// Generic concept model; real campaign semantics are intentionally deferred.
public sealed record Activity(
    string ActivityId,
    string CustomerId,
    string ActivityType,
    DateTimeOffset OccurredAt,
    string? CampaignId);
