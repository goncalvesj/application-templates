using Azure.Data.AppConfiguration;
using Azure.Identity;
using Azure.Storage.Blobs;
using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder()    
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGet("/testappconfig", async () =>
{
    var endpoint = builder.Configuration["APP_CONFIG_ENDPOINT"] ?? "";
    var key = builder.Configuration["CONFIG_NAME"] ?? "";

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
        using var con = new SqlConnection(mySetting);
        con.Open();

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
        var redis = ConnectionMultiplexer.Connect(mySetting);

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