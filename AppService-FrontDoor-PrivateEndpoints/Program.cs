using Azure.Storage.Blobs;
using Dapper;
using Microsoft.Data.SqlClient;
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

app.MapGet("/testsql", async () =>
{
    try
    {
        var mySetting = configuration.GetValue<string>("SQL_CONNECTIONSTRING");
        using var con = new SqlConnection(mySetting);
        con.Open();

        var version = await con.ExecuteScalarAsync<string>("SELECT @@VERSION");

        return version;
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

        return "Connectivity test successful!";
    }
    catch (Exception)
    {
        throw;
    }
})
.WithName("TestStorageConnection")
.WithOpenApi();

app.Run();