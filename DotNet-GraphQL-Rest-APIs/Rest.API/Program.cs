using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

const string SERVICE_NAME = "sw-api";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(
            serviceName: SERVICE_NAME,
            serviceVersion: System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3)
            )
        .AddAttributes(new Dictionary<string, object>
            {
                { "host.name", Environment.MachineName }
            })
     )
    .WithTracing(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource(SERVICE_NAME)
        .AddOtlpExporter())
    .WithMetrics(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddPrometheusExporter()
        .AddOtlpExporter());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapHealthChecks("/health");

app.MapGet("/people", async () =>
{
    var data = new DataService();
    var people = await data.GetPeopleAsync();
    return people;
});

app.MapGet("/films", async () =>
{
    var data = new DataService();
    var films = await data.GetFilmsAsync();
    return films;
});

app.MapGet("/cast", async () =>
{
    var data = new DataService();
    var castList = await data.GetCastAsync();

    return castList;
});

app.Run();