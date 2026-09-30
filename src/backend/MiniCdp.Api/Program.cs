using System.Text.Json;
using System.Text.Json.Serialization;
using MiniCdp.Application.Abstractions;
using MiniCdp.Application.Analysis;
using MiniCdp.Application.Segments;
using MiniCdp.Application.Semantics;
using MiniCdp.Domain.Analysis;
using MiniCdp.Domain.Segments;
using MiniCdp.Infrastructure.Mock;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<ICustomerDataSource, MockCustomerDataSource>();
builder.Services.AddSingleton<IAnalysisDataSource, MockAnalysisDataSource>();
builder.Services.AddSingleton<ISemanticCatalog, SemanticCatalog>();
builder.Services.AddSingleton<ISegmentEngine, SegmentEngine>();
builder.Services.AddSingleton<IAnalysisService, AnalysisService>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "ok",
    dataSource = "mock",
    dataClassification = "DEMO / MOCK DATA"
}));

app.MapGet("/api/semantic-fields", (ISemanticCatalog catalog) => Results.Ok(new
{
    fields = catalog.Fields,
    metrics = catalog.Metrics,
    operators = catalog.Operators
}));

app.MapPost("/api/audiences/preview", async (
    SegmentGroup definition,
    ISegmentEngine engine,
    TimeProvider timeProvider,
    CancellationToken cancellationToken) =>
{
    try
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        return Results.Ok(await engine.PreviewAsync(definition, today, cancellationToken));
    }
    catch (SegmentValidationException exception)
    {
        return Results.BadRequest(new { error = exception.Message });
    }
    catch (Exception exception) when (exception is FormatException or InvalidOperationException)
    {
        return Results.BadRequest(new { error = "A condition contains an invalid value." });
    }
});

app.MapPost("/api/analyses/run", async (
    AnalysisDefinition definition,
    IAnalysisService service,
    CancellationToken cancellationToken) =>
{
    try
    {
        return Results.Ok(await service.RunAsync(definition, cancellationToken));
    }
    catch (Exception exception) when (exception is AnalysisValidationException or SemanticValidationException or SegmentValidationException)
    {
        return Results.BadRequest(new { error = exception.Message });
    }
    catch (Exception exception) when (exception is FormatException or InvalidOperationException)
    {
        return Results.BadRequest(new { error = "The analysis contains an invalid value." });
    }
});

app.Run();

public partial class Program;
