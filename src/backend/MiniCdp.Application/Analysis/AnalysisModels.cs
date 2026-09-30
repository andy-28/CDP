using MiniCdp.Domain.Analysis;

namespace MiniCdp.Application.Analysis;

public sealed record AnalysisRow(
    string Period,
    int? UniqueCustomers,
    decimal? AverageOrderValue);

public sealed record BucketResultRow(
    string Bucket,
    int CustomerCount,
    decimal TotalPurchaseAmount);

public sealed record AnalysisResult(
    IReadOnlyList<AnalysisRow> Rows,
    IReadOnlyList<BucketResultRow> Distribution,
    string DataClassification = "DEMO / MOCK DATA");

public interface IAnalysisService
{
    Task<AnalysisResult> RunAsync(AnalysisDefinition definition, CancellationToken cancellationToken = default);
}
