using MiniCdp.Application.Analysis;
using MiniCdp.Domain.Analysis;

namespace MiniCdp.Application.Abstractions;

public interface IAnalysisDataSource
{
    Task<AnalysisResult> ExecuteAsync(AnalysisDefinition definition, CancellationToken cancellationToken = default);
}
