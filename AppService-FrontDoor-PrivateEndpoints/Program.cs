using Azure.Data.AppConfiguration;
using Azure.Identity;
using Azure.Storage.Blobs;
using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder()    
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddOpenTelemetry()
    .WithTracing(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter())
    .WithMetrics(builder => builder        
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddConsoleExporter()
        .AddPrometheusExporter()
        .AddOtlpExporter());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapHealthChecks("/health");

const string connStringMessage = "Connection string not found.";

app.MapGet("/testhello", () => "What's up doc!").WithOpenApi();

app.MapGet("/testappconfig", async () =>
{
    var endpoint = builder.Configuration["APP_CONFIG_ENDPOINT"] ?? "";
    var key = builder.Configuration["APP_CONFIG_ENDPOINT_KEY"] ?? "";

    var value = Guid.NewGuid().ToString();

    var client = new ConfigurationClient(new Uri(endpoint), new DefaultAzureCredential());    
    await client.SetConfigurationSettingAsync($"{key}-{value}", value);

    return "Connection to Azure App Config successful. Config setting created.";
})
.WithName("TestAppConfigConnection")
.WithOpenApi();

app.MapGet("/testpostgres", async () =>
{
    try
    {
        var mySetting = configuration.GetValue<string>("POSTGRES_CONNECTIONSTRING");

        if (string.IsNullOrEmpty(mySetting))
            throw new ArgumentException(connStringMessage);

        await using var conn = new NpgsqlConnection(mySetting);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand("SELECT version()", conn);
        
        var reader = await cmd.ExecuteScalarAsync();
        
        return $"Connection to Azure Postgres successful. Version: {reader}";
    }
    catch (Exception)
    {
        throw;
    }
})
.WithName("TestPostresConnection")
.WithOpenApi();

app.MapGet("/testsql", async () =>
{
    try
    {
        var mySetting = configuration.GetValue<string>("SQL_CONNECTIONSTRING");

        if (string.IsNullOrEmpty(mySetting))
            throw new ArgumentException(connStringMessage);

        using var con = new SqlConnection(mySetting);
        await con.OpenAsync();

        var version = await con.ExecuteScalarAsync<string>("SELECT @@VERSION");
                
        return $"Connection to Azure SQL successful. Version: {version}";
    }
    catch (Exception)
    {
        throw;
    }
})
.WithName("TestSqlConnection")
.WithOpenApi();

app.MapGet("/testcache", async () =>
{
    try
    {
        var mySetting = configuration.GetValue<string>("CACHE_CONNECTIONSTRING");
        
        if (string.IsNullOrEmpty(mySetting))
            throw new ArgumentException(connStringMessage);
        
        var redis = await ConnectionMultiplexer.ConnectAsync(mySetting);

        var cache = redis.GetDatabase();

        var testValue = "TestValue";

        await cache.StringSetAsync("TestKey", testValue);

        return "Connection to Azure Redis Cache successful.";
    }
    catch (Exception)
    {
        throw;
    }
})
.WithName("TestCacheConnection")
.WithOpenApi();

app.MapGet("/teststorage", async () =>
{
    try
    {
        var mySetting = configuration.GetValue<string>("STORAGE_CONNECTIONSTRING");

        if (string.IsNullOrEmpty(mySetting))
            throw new ArgumentException(connStringMessage);

        var containerName = "test-container";

        BlobContainerClient container = new(mySetting, containerName);
        await container.CreateIfNotExistsAsync();

        return "Connection to Azure Storage successful. Blob container created.";
    }
    catch (Exception)
    {
        throw;
    }
})
.WithName("TestStorageConnection")
.WithOpenApi();

app.Run();