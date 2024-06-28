using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Set up configuration sources.
var secretsPath = Environment.GetEnvironmentVariable("CONFIG_FILES_PATH");
IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    // Add secrets from a directory, for example from a Kubernetes secret mount.
    .AddKeyPerFile(directoryPath: $"{secretsPath}", optional: true)
    .Build();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddTypeActivatedCheck<ApiHealthCheck>("NodeApiHealth", args: new object[] { configuration, "NODEAPI_URL" })
    .AddTypeActivatedCheck<ApiHealthCheck>("PythonApiHealth", args: new object[] { configuration, "PYAPI_URL" });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
{
    var paths = new OpenApiPaths();
    var operation = new OpenApiOperation
    {
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Health" } },
        Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse { Description = "Healthy" },
            ["503"] = new OpenApiResponse { Description = "Unhealthy" }
        }
    };
    swaggerDoc.Paths.Add("/health", new OpenApiPathItem { Operations = { [OperationType.Get] = operation } });
});
});
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapHealthChecks("/health");


app.MapGet("api/data/{key}", async (string key) =>
{
    var client = new HttpClient();
    var result = await client.GetAsync($"{configuration.GetValue<string>("NODEAPI_URL")}/api/data/{key}");
    return await result.Content.ReadFromJsonAsync<Model>();
})
.WithOpenApi();

app.MapPost("api/data", async (Model data) =>
{
    try
    {
        var json = JsonSerializer.Serialize(data);

        // Some error
        string nullString = null;
        Console.WriteLine(nullString.Length);

        var client = new HttpClient();
        var response = await client.PostAsync($"{configuration.GetValue<string>("PYAPI_URL")}/api/data",
            new StringContent(json, null, "application/json"));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    catch (Exception ex)
    {
        throw;
    }
})
.WithOpenApi();

app.Run();

class ApiHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;
    private readonly string _configSetting;

    public ApiHealthCheck(IConfiguration configuration, string configSetting)
    {
        _configuration = configuration;
        _configSetting = configSetting;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var client = new HttpClient();
        var apiUrl = _configuration.GetValue<string>(_configSetting);
        var apiResponse = await client.GetAsync($"{apiUrl}/api/health", cancellationToken);

        return apiResponse.StatusCode == HttpStatusCode.OK ?
                       await Task.FromResult(new HealthCheckResult(
                             status: HealthStatus.Healthy,
                             description: "API is healthy.")) :
                       await Task.FromResult(new HealthCheckResult(
                             status: HealthStatus.Unhealthy,
                             description: "API is unhealthy."));
    }
}

class Model
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("age")]
    public int Age { get; set; }
    [JsonPropertyName("location")]
    public string? Location { get; set; }
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}
